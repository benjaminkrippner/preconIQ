import os, json
from dataclasses import dataclass
from typing import List, Dict
from dotenv import load_dotenv
from pathlib import Path
from azure.search.documents.models import VectorizedQuery
from azure.core.credentials import AzureKeyCredential
from azure.search.documents import SearchClient
from openai import AzureOpenAI

# ---- Import dictionaries and output format ----
from wind.wind_output_format_full import WIND_FIELD_FORMAT
from wind.wind_dictionary_full import FIELD_DEFINITIONS as WIND_DICT

# ---- Environment setup ----
load_dotenv()

AZURE_OPENAI_ENDPOINT = os.environ["AZURE_OPENAI_ENDPOINT"]
AZURE_OPENAI_KEY = os.environ["AZURE_OPENAI_KEY"]
AZURE_OPENAI_API_VERSION = os.environ["AZURE_OPENAI_API_VERSION"]
EMBEDDING_MODEL = os.environ["AOAI_EMBEDDING_DEPLOYMENT"]
CHAT_MODEL = os.environ["AOAI_CHAT_DEPLOYMENT"]

SEARCH_ENDPOINT = os.environ["AZURE_SEARCH_ENDPOINT"]
SEARCH_KEY = os.environ["AZURE_SEARCH_KEY"]
SEARCH_INDEX = os.environ["AZURE_SEARCH_INDEX"]

# ---- Azure Clients ----
aoai = AzureOpenAI(
    api_key=AZURE_OPENAI_KEY,
    api_version=AZURE_OPENAI_API_VERSION,
    azure_endpoint=AZURE_OPENAI_ENDPOINT,
)
search_client = SearchClient(
    endpoint=SEARCH_ENDPOINT,
    index_name=SEARCH_INDEX,
    credential=AzureKeyCredential(SEARCH_KEY),
)

# ---- Index field names ----
KEY_FIELD = "chunk_id"
CONTENT_FIELD = "chunk"
SOURCE_FIELD = "title"
VECTOR_FIELD = "text_vector"
TOP_K = 15
# ---------- Data contracts ----------
@dataclass
class Passage:
    id: str
    score: float
    content: str
    source: str


# ---------- Embeddings ----------
def embed_text(text: str) -> List[float]:
    """Generate embeddings for a query."""
    return aoai.embeddings.create(input=text, model=EMBEDDING_MODEL).data[0].embedding


# ---------- Retrieval ----------
def retrieve(query: str, top_k: int = 10) -> List[Passage]:
    """Perform vector-based hybrid retrieval from Azure AI Search."""
    qvec = embed_text(query)
    vq = VectorizedQuery(vector=qvec, fields=VECTOR_FIELD, k_nearest_neighbors=top_k * 2)
    results = search_client.search(
        search_text=query,
        vector_queries=[vq],
        select=[KEY_FIELD, CONTENT_FIELD, SOURCE_FIELD],
        top= TOP_K,
    )

    # This is performing a ranking on the returned object.
    passages = [
        Passage(
            id=str(r[KEY_FIELD]),
            score=float(getattr(r, "@search.score", 0.0)),
            content=r[CONTENT_FIELD],
            source=r.get(SOURCE_FIELD, r[KEY_FIELD]),
        )
        for r in results
    ]
    passages.sort(key=lambda p: p.score, reverse=True)
    return passages


# ---------- Prompt Construction ----------
SYS_PROMPT = """\
You are a factual RAG assistant. Use only the given context.
If not found in context, reply exactly: "I don’t know."
You may combine relevant snippets.
"""

def make_context(passages: List[Passage], max_chars: int = 6500) -> str:
    """Concatenate top passages into a context block."""
    out, total = [], 0
    for p in passages:
        chunk = f"[{p.id}] {p.content}\n( title: {p.source} | score: {p.score:.3f} )\n---\n"
        if total + len(chunk) > max_chars:
            break
        out.append(chunk)
        total += len(chunk)
    return "".join(out)


def build_messages(question: str, passages: List[Passage]) -> list[Dict[str, str]]:
    """Construct message payload for Azure OpenAI chat."""
    context = make_context(passages)
    user = f"Question: {question}\nContext:\n{context}\nFormat:\n1) Concise factual answer.\n2) 'Citations:' list."
    return [{"role": "system", "content": SYS_PROMPT}, {"role": "user", "content": user}]


# ---------- LLM Call ----------
def answer_with_rag(question: str, passages: List[Passage]) -> str:
    """Query the LLM with context-augmented question."""
    resp = aoai.chat.completions.create(
        model=CHAT_MODEL,
        temperature=0.1,
        max_tokens=600,
        messages=build_messages(question, passages),
    )
    return resp.choices[0].message.content.strip()


# ---------- Extraction Helpers ----------
def extract_value_only(text: str) -> str:
    """Strip citations and quotes from LLM output."""
    txt = text.split("\nCitations:")[0].strip()
    if (txt.startswith('"') and txt.endswith('"')) or (txt.startswith("'") and txt.endswith("'")):
        txt = txt[1:-1].strip()
    return txt.splitlines()[0].strip() if txt else "unknown"


# ---------- Dictionary / Format Utilities ----------
def find_definition(field_name: str, dictionary: dict) -> tuple[str | None, str | None]:
    """
    Look up a field's Definition and Example in any provided dictionary.
    Structure expected:
        { "Category": { "Subcategory": [ { "Field": "...", "Definition": "...", "Example": "..." }, ... ] } }
    """
    field_lower = field_name.strip().lower()
    for _, groups in dictionary.items():
        for _, items in groups.items():
            for it in items:
                if it.get("Field", "").strip().lower() == field_lower:
                    return it.get("Definition"), it.get("Example")
    return None, None


def build_retrieval_hint(field_name: str, dictionary: dict) -> str:
    """Generate a context-aware retrieval hint from a dictionary definition."""
    definition, example = find_definition(field_name, dictionary)
    example_str = f" Example: {example}." if example else ""
    if definition:
        return (
            f"Search for '{field_name}'. Definition: {definition}.{example_str} "
            "Prefer explicit specifications, BoD tables, or one-line summaries."
        )
    return (
        f"Search for '{field_name}'. "
        "Prefer explicit specifications, BoD tables, or one-line summaries."
    )


def build_extraction_prompt(field_name: str, format_dict: dict | None = None) -> str:
    """Build a precise extraction prompt with optional format hints."""
    format_hint = ""
    if format_dict and field_name in format_dict:
        format_hint = f" {format_dict[field_name]}"
    return (
        f"Using the retrieved documents, extract the value for '{field_name}'. "
        "Answer ONLY the value with no extra words. "
        "If unknown, answer exactly 'unknown'." + format_hint
    )


# ---------- Query Logic ----------
def query_field(field_name: str, dictionary: dict, format_dict: dict | None = None) -> tuple[str, str]:
    """
    Retrieve AI-generated value and source for a field using a provided dictionary and optional format.
    """
    retrieval_hint = build_retrieval_hint(field_name, dictionary)
    passages = retrieve(retrieval_hint, top_k=TOP_K)
    raw_answer = answer_with_rag(build_extraction_prompt(field_name, format_dict), passages)
    value = extract_value_only(raw_answer)
    source = passages[0].source if passages and value.lower() != "unknown" else ""
    return value, source


# ---------- Main ----------
if __name__ == "__main__":

    filled = WIND_FIELD_FORMAT

    for section, groups in filled.items():
        print(f"# {section}")

        for group_name, items in groups.items():

            if not isinstance(items, list):
                continue

            print(f"  - {group_name}")

            for item in items:
                fname = item.get("Field", "").strip()
                if not fname:
                    continue

                value, source = query_field(fname, dictionary=WIND_DICT)
                item["AI_Search_Finding"] = value or "unknown"
                item["Source"] = source 

                print(f"      • {fname}: {value}{' (src: ' + source + ')' if source else ''}")
