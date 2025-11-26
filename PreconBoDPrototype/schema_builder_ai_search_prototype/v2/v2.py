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
from wind.wind_output_format import WIND_FIELD_FORMAT
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

@dataclass
class Passage:
    id: str
    score: float
    content: str
    source: str

def embed_text(text: str) -> List[float]:
    return aoai.embeddings.create(input=text, model=EMBEDDING_MODEL).data[0].embedding

def retrieve(query: str, top_k: int = 10) -> List[Passage]:
    qvec = embed_text(query)
    vq = VectorizedQuery(vector=qvec, fields=VECTOR_FIELD, k_nearest_neighbors=top_k * 2)
    results = search_client.search(
        search_text=query,
        vector_queries=[vq],
        select=[KEY_FIELD, CONTENT_FIELD, SOURCE_FIELD],
        top=TOP_K,
    )

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

def make_context(passages: List[Passage], max_chars: int = 6000) -> str:
    out, total = [], 0
    for p in passages:
        chunk = f"[{p.id}] {p.content}\n(title: {p.source} | score: {p.score:.3f})\n---\n"
        if total + len(chunk) > max_chars:
            break
        out.append(chunk)
        total += len(chunk)
    return "".join(out)

if __name__ == "__main__":

    print("\nRunning AI Search autofill for empty fields...\n")

    for section, groups in WIND_FIELD_FORMAT.items():
        print(f"# {section}")

        for group_name, fields in groups.items():
            print(f"  - {group_name}")

            for field_name, field_value in fields.items():
                if field_value.strip() != "":
                    continue

                definition = None
                for sect_def in WIND_DICT.values():
                    if isinstance(sect_def, dict):
                        for group_def in sect_def.values():
                            if isinstance(group_def, dict) and field_name in group_def:
                                definition = group_def[field_name]
                                break
                        if definition:
                            break

                if not definition:
                    print(f"      • {field_name}: (no definition found)")
                    continue

                retrieval_hint = f"Search for '{field_name}'. Definition: {definition}. Prefer explicit specifications, BoD tables, or one-line summaries."
                passages = retrieve(retrieval_hint, top_k=TOP_K)

                context = make_context(passages)
                extraction_prompt = f"""
You are a precise engineering assistant.
Field: "{field_name}"
Definition: {definition}

Context:
{context}

Return only a <5-word factual value from retrieved document.
If unknown, return exactly 'unknown'.
"""

                response = aoai.chat.completions.create(
                    model="gpt-4o-mini",
                    temperature=0.2,
                    max_tokens=50,
                    messages=[{"role": "user", "content": extraction_prompt}],
                )

                result = response.choices[0].message.content.strip()
                source = passages[0].source if passages else "N/A"

                print(f"      • {field_name}: {result}  (source: {source})")
