import io
import json
import logging
import os
import tempfile
import zipfile
from dataclasses import dataclass
from typing import Optional, Dict, Any, List, Tuple

import azure.functions as func
from azure.core.exceptions import ResourceNotFoundError
from azure.storage.blob import (
    BlobServiceClient,
    BlobClient,
    BlobRequestConditions,
)
from azure.core.pipeline.policies import RetryPolicy

from openai import AzureOpenAI

# Text extractors
from pypdf import PdfReader
from openpyxl import load_workbook
from docx import Document  # python-docx
import xmltodict
from dbfread import DBF
import extract_msg


# ========= CONFIG =========
ENDPOINT     = os.environ["AZURE_OPENAI_API_ENDPOINT"]
API_KEY      = os.environ["AZURE_OPENAI_API_KEY"]
DEPLOYMENT   = os.environ["AZURE_OPENAI_DEPLOYMENT"]
API_VERSION  = os.getenv("AZURE_OPENAI_API_VERSION", "2024-08-01-preview")

STORAGE_CONN     = os.environ["AzureWebJobsStorage"]
INPUT_CONTAINER  = "input-files"
OUTPUT_CONTAINER = "output-files"

# NOTE: We ONLY read from this blob (the data dictionary). We DO NOT traverse that folder for documents.
DICT_BLOB_NAME    = "data_dictionary/Wind_BOD_Data_Dictionary_Draft.xlsx"

# The only file we will write as final, running aggregate of all values found.
MASTER_BLOB_NAME  = "_summary_master.json"

# ========= LIMITS / CONSTANTS =========
MAX_CHARS = 16000            # max chars sent to the LLM
MAX_GUIDANCE_CHARS = 24000   # cap dictionary guidance (generous)
MAX_ROWS_XLSX = 8            # per sheet
MAX_SHEETS_XLSX = 5
MAX_PAGES_PDF = 25
MAX_RECORDS_DBF = 50         # cap DBF records to avoid huge prompts
MAX_FILES_IN_ZIP = 20        # process first N supported files inside zip
MASTER_UPDATE_RETRIES = 5

# ========= CLIENTS =========
# Storage with resilient retries
blob_service = BlobServiceClient.from_connection_string(
    STORAGE_CONN,
    retry_policy=RetryPolicy(
        retry_total=6,
        retry_backoff_factor=0.8,
        retry_mode="exponential"
    ),
)
client = AzureOpenAI(
    azure_endpoint=ENDPOINT,
    api_key=API_KEY,
    api_version=API_VERSION,
)


# ========= SCHEMA =========
SUMMARY_SCHEMA: Dict[str, Any] = {
    "Summary": {
        "Owner": "",
        "Location": "",
        "MWac": "",
        "Interconnect_Voltage": "",
        "Number_of_Turbines": "",
        "Turbine_OEM": "",
        "Turbine_Model": "",
        "Turbine_MW": "",
        "Turbine_Hub_Height": "",
        "Turbine_Rotor_Diameter": "",
        "Owner_Supplied_Equipment": "",
        "Turbine_Locations": "",
        "Substation_Location": "",
        "Transmission_Route": "",
        "ALTA": "",
        "Land_Control": "",
        "Environmental_Constraints": "",
        "Geotech": "",
        "Topo_Survey_/_Surface_/_Lidar": "",
        "Interconnect_Agreement_(LGIA,_SGIA,_GIA)": "",
        "PPA": "",
        "Hydrology": "",
        "Wetland_Delineation": ""
    }
}


# ========= HELPERS =========
def _clip(text: str, limit: int = MAX_CHARS) -> str:
    """Clip text to `limit` chars, appending a truncation marker when applied."""
    if text is None:
        return ""
    return (text[:limit] + "\n[Truncated]") if len(text) > limit else text


def _safe_deepcopy(obj: Any) -> Any:
    """JSON round-trip deep copy (safe for dicts/lists of primitives)."""
    return json.loads(json.dumps(obj))


def _container_client(name: str):
    return blob_service.get_container_client(name)


def _blob_client(container: str, name: str) -> BlobClient:
    return _container_client(container).get_blob_client(name)


# ========= DATA DICTIONARY LOADER =========
def _load_dictionary_guidance() -> str:
    """
    Loads the single, expected data dictionary workbook from DICT_BLOB_NAME.
    Adapts to a multi-sheet dictionary with variable header rows.

    Returns a human-readable guidance string suitable for prompt augmentation
    (soft-capped by MAX_GUIDANCE_CHARS).
    """
    cont = _container_client(INPUT_CONTAINER)
    try:
        data = cont.get_blob_client(DICT_BLOB_NAME).download_blob().readall()
    except ResourceNotFoundError:
        logging.warning("Data dictionary not found at '%s'. Proceeding without it.", DICT_BLOB_NAME)
        return ""
    except Exception:
        logging.exception("Failed to download data dictionary blob")
        return ""

    try:
        wb = load_workbook(io.BytesIO(data), read_only=True, data_only=True)
    except Exception:
        logging.exception("Failed to open data dictionary workbook")
        return ""

    def _norm(s) -> str:
        return str(s or "").strip().lower()

    def _find_header_row(ws, max_scan_rows: int = 100) -> Tuple[int, dict]:
        """
        Find the row containing headers and return (row_index, column_map).
        Required: field + definition/description
        Optional: synonym + example
        """
        for i, row in enumerate(ws.iter_rows(min_row=1, max_row=max_scan_rows, values_only=True), start=1):
            if not row:
                continue
            headers = [_norm(h) for h in row]
            col_field = next((idx for idx, h in enumerate(headers) if "field" in h), None)
            col_def = next((idx for idx, h in enumerate(headers) if "definition" in h or "description" in h), None)
            col_syn = next((idx for idx, h in enumerate(headers) if "synonym" in h), None)
            col_ex  = next((idx for idx, h in enumerate(headers) if "example" in h), None)
            if col_field is not None and col_def is not None:
                return i, {"field": col_field, "definition": col_def, "synonym": col_syn, "example": col_ex}
        return -1, {}

    lines: List[str] = []
    total_entries = 0

    for ws in wb.worksheets:
        header_row_idx, cmap = _find_header_row(ws)
        if header_row_idx < 0:
            logging.info("Skipped sheet '%s' (no recognizable header row).", ws.title)
            continue

        sheet_count = 0
        for row in ws.iter_rows(min_row=header_row_idx + 1, values_only=True):
            if not row:
                continue

            def _get(idx: Optional[int]) -> str:
                if idx is None:
                    return ""
                val = row[idx] if idx < len(row) else None
                return str(val).strip() if val is not None else ""

            field = _get(cmap["field"])
            definition = _get(cmap["definition"])
            synonyms = _get(cmap.get("synonym"))
            example = _get(cmap.get("example"))

            if not field and not definition and not synonyms and not example:
                continue
            if _norm(field) in {"field name", "field", "name"}:
                continue

            piece = f"- Category: {ws.title} | Field: {field}"
            if definition:
                piece += f" | Definition: {definition}"
            if synonyms:
                piece += f" | Synonyms: {synonyms}"
            if example:
                piece += f" | Example: {example}"

            lines.append(piece)
            total_entries += 1
            sheet_count += 1

        logging.info("Loaded %d entries from sheet '%s'.", sheet_count, ws.title)

    guidance = "DATA DICTIONARY:\n" + ("\n".join(lines) if lines else "(empty)")
    guidance = _clip(guidance, MAX_GUIDANCE_CHARS)
    logging.info(
        "Loaded data dictionary guidance: %d total entries across %d sheets (len=%d).",
        total_entries, len(wb.worksheets), len(guidance),
    )
    return guidance


# ========= EXTRACTORS =========
def _extract_text(b: bytes, blob_name: Optional[str]) -> str:
    """
    Returns a best-effort plaintext representation usable by the LLM.
    Supports: .txt .csv .json .md .log .pdf .xlsx .docx .xml .kml .kmz .dbf .msg .cpg .prj .zip
    Skips: .dwg .ctb .pc3 .sbn .sbx .shp .shx (no human text); image OCR not included here.
    """
    ext = (os.path.splitext(blob_name or "")[1] or "").lower()

    # ---------- Plain-ish text ----------
    if ext in {".txt", ".csv", ".json", ".md", ".log", ".cpg", ".prj"}:
        for enc in ("utf-8", "latin-1"):
            try:
                return b.decode(enc, errors="strict")
            except UnicodeDecodeError:
                continue
        return b.decode("utf-8", errors="ignore")

    # ---------- PDF (text-based only) ----------
    if ext == ".pdf":
        try:
            reader = PdfReader(io.BytesIO(b))
            pages: List[str] = []
            for i, p in enumerate(reader.pages):
                if i >= MAX_PAGES_PDF:
                    pages.append("\n[Truncated additional pages]")
                    break
                try:
                    pages.append(p.extract_text() or "")
                except Exception:
                    pages.append("")
            text = "\n".join(pages).strip()
            return text or "<pdf contained no extractable text>"
        except Exception as e:
            logging.exception("PDF extraction failed")
            return f"<pdf extraction failed: {e}>"

    # ---------- XLSX (sheet names + first rows) ----------
    if ext == ".xlsx":
        try:
            wb = load_workbook(io.BytesIO(b), read_only=True, data_only=True)
            parts: List[str] = []
            for ws in wb.worksheets[:MAX_SHEETS_XLSX]:
                parts.append(f"# Sheet: {ws.title}")
                rows = list(ws.iter_rows(min_row=1, max_row=MAX_ROWS_XLSX, values_only=True))
                for r in rows:
                    parts.append(" | ".join("" if c is None else str(c) for c in r))
            return "\n".join(parts) if parts else "<xlsx had no readable cells>"
        except Exception as e:
            logging.exception("XLSX extraction failed")
            return f"<xlsx extraction failed: {e}>"

    # ---------- DOCX (paragraphs + simple tables) ----------
    if ext == ".docx":
        try:
            doc = Document(io.BytesIO(b))
            parts: List[str] = []
            for para in doc.paragraphs:
                t = para.text.strip()
                if t:
                    parts.append(t)
            for tbl in doc.tables:
                for row in tbl.rows[:15]:
                    parts.append(" | ".join(cell.text.strip() for cell in row.cells))
            return "\n".join(parts) if parts else "<docx contained no text>"
        except Exception as e:
            logging.exception("DOCX extraction failed")
            return f"<docx extraction failed: {e}>"

    # ---------- XML/KML ----------
    if ext in {".xml", ".kml"}:
        try:
            data = xmltodict.parse(b)
            texts: List[str] = []

            def walk(node):
                if isinstance(node, dict):
                    for v in node.values():
                        walk(v)
                elif isinstance(node, list):
                    for v in node:
                        walk(v)
                else:
                    s = str(node).strip()
                    if s:
                        texts.append(s)

            walk(data)
            return _clip("\n".join(texts), 12000) if texts else "<xml/kml contained no text>"
        except Exception as e:
            logging.exception("XML/KML extraction failed")
            return f"<xml/kml extraction failed: {e}>"

    # ---------- KMZ (zip containing KML) ----------
    if ext == ".kmz":
        try:
            with zipfile.ZipFile(io.BytesIO(b)) as zf:
                texts = []
                for name in zf.namelist():
                    if name.lower().endswith(".kml"):
                        try:
                            kml_bytes = zf.read(name)
                            s = _extract_text(kml_bytes, blob_name="inner.kml")
                            if s:
                                texts.append(f"[{name}]\n{s}")
                        except Exception:
                            continue
                return "\n\n".join(texts) if texts else "<kmz had no kml text>"
        except Exception as e:
            logging.exception("KMZ extraction failed")
            return f"<kmz extraction failed: {e}>"

    # ---------- DBF ----------
    if ext == ".dbf":
        try:
            with tempfile.NamedTemporaryFile(suffix=".dbf", delete=True) as tmp:
                tmp.write(b)
                tmp.flush()
                table = DBF(tmp.name, load=True, ignore_missing_memofile=True)
                parts: List[str] = []
                for count, rec in enumerate(table):
                    if count >= MAX_RECORDS_DBF:
                        parts.append("[Truncated more DBF records]")
                        break
                    line = " | ".join(f"{k}={rec.get(k)}" for k in rec.keys())
                    parts.append(line)
            return "\n".join(parts) if parts else "<dbf had no records>"
        except Exception as e:
            logging.exception("DBF extraction failed")
            return f"<dbf extraction failed: {e}>"

    # ---------- MSG ----------
    if ext == ".msg":
        try:
            with tempfile.NamedTemporaryFile(suffix=".msg", delete=True) as tmp:
                tmp.write(b)
                tmp.flush()
                m = extract_msg.Message(tmp.name)
                subj = (m.subject or "").strip()
                body = (m.body or "").strip()
                sender = (m.sender or "").strip()
                date = str(m.date) if getattr(m, "date", None) else ""
                hdr = " | ".join(v for v in [f"Subject: {subj}", f"From: {sender}", f"Date: {date}"] if v)
                return (hdr + "\n\n" + body).strip() if (subj or body) else "<msg contained no text>"
        except Exception as e:
            logging.exception("MSG extraction failed")
            return f"<msg extraction failed: {e}>"

    # ---------- ZIP ----------
    if ext == ".zip":
        try:
            texts: List[str] = []
            with zipfile.ZipFile(io.BytesIO(b)) as zf:
                count = 0
                for name in zf.namelist():
                    if count >= MAX_FILES_IN_ZIP:
                        texts.append("[Truncated more files in zip]")
                        break
                    inner_ext = os.path.splitext(name)[1].lower()
                    if inner_ext in {
                        ".txt", ".csv", ".json", ".md", ".log",
                        ".pdf", ".xlsx", ".docx", ".xml", ".kml", ".kmz",
                        ".dbf", ".msg", ".cpg", ".prj"
                    }:
                        try:
                            s = _extract_text(zf.read(name), blob_name=name)
                            if s:
                                texts.append(f"[{name}]\n{s}")
                                count += 1
                        except Exception:
                            continue
            return "\n\n".join(texts) if texts else "<zip contained no supported files>"
        except Exception as e:
            logging.exception("ZIP extraction failed")
            return f"<zip extraction failed: {e}>"

    # ---------- Unsupported or images without OCR ----------
    if ext in {".jpg", ".jpeg", ".png"}:
        return "<image file; OCR not enabled>"

    if ext in {".dwg", ".ctb", ".pc3", ".sbn", ".sbx", ".shp", ".shx"}:
        return f"<{ext} not processed; no human-readable text>"

    # Fallback
    try:
        return b.decode("utf-8")
    except UnicodeDecodeError:
        return "<binary file not decoded>"


# ========= MASTER STATE (CONCURRENCY-SAFE) =========
def _load_master_state_with_etag() -> Tuple[Dict[str, Any], Optional[str]]:
    """
    Download master JSON and return (dict, etag). If not found, return (fresh copy, None).
    """
    bc = _blob_client(OUTPUT_CONTAINER, MASTER_BLOB_NAME)
    try:
        downloader = bc.download_blob()
        etag = downloader.properties.etag  # current ETag for optimistic concurrency
        data = downloader.readall()
        return json.loads(data.decode("utf-8")), etag
    except Exception:
        # Not found or first run: start fresh
        return _safe_deepcopy(SUMMARY_SCHEMA), None


def _merge_no_overwrite(master: Dict[str, Any], new_result: Dict[str, Any]) -> Dict[str, Any]:
    """
    Merge strategy: keep existing non-empty values in the master; only fill blanks.
    """
    merged = _safe_deepcopy(master)
    for section, fields in new_result.items():
        merged.setdefault(section, {})
        for k, v in fields.items():
            if v and not merged[section].get(k):
                merged[section][k] = v
    return merged


def _save_master_state_concurrent_safe(merged: Dict[str, Any], etag: Optional[str]) -> None:
    """
    Upload with optimistic concurrency. If an ETag is provided, enforce If-Match.
    Retry a few times in case of races.
    """
    bc = _blob_client(OUTPUT_CONTAINER, MASTER_BLOB_NAME)

    for attempt in range(1, MASTER_UPDATE_RETRIES + 1):
        try:
            if etag:
                conditions = BlobRequestConditions(if_match=etag)
            else:
                # If blob doesn't exist, require it to not exist (If-None-Match: "*")
                conditions = BlobRequestConditions(if_none_match="*")

            bc.upload_blob(
                json.dumps(merged, indent=2),
                overwrite=True,  # required by SDK; conditions still enforce ETag/None-Match
                conditions=conditions,
            )
            return
        except Exception as e:
            # ETag mismatch or race: re-download, re-merge, and retry
            logging.warning("Master upload race detected (attempt %d): %s", attempt, e)
            current, current_etag = _load_master_state_with_etag()
            merged = _merge_no_overwrite(current, merged)  # preserve 'first non-empty wins'
            etag = current_etag
    # Final attempt without retries raised
    raise RuntimeError("Failed to update master after concurrent retries")


# ========= LLM FILL =========
def _llm_fill_summary(text: str, dictionary_guidance: str) -> Dict[str, Any]:
    """
    Calls Azure OpenAI with JSON-mode to fill the SUMMARY_SCHEMA fields.
    Missing values MUST be empty strings.
    """
    system = (
        "You extract structured data for a wind farm Basis of Design. "
        "Return STRICT JSON matching the provided schema keys and structure. "
        "If a value is not present, set it to an empty string ''. Do not invent values."
    )
    user = (
        "Use the data dictionary to map document terms to schema fields.\n\n"
        f"{(dictionary_guidance or '(No data dictionary provided.)')}\n\n"
        "DOCUMENT TEXT (truncated if large):\n"
        f"{_clip(text, MAX_CHARS)}\n\n"
        "SCHEMA (fill only these fields under 'Summary'):\n"
        f"{json.dumps(SUMMARY_SCHEMA, indent=2)}\n\n"
        "Return ONLY the JSON object. No commentary."
    )

    resp = client.chat.completions.create(
        model=DEPLOYMENT,
        messages=[{"role": "system", "content": system},
                  {"role": "user", "content": user}],
        temperature=0.1,
        max_tokens=1000,
        response_format={"type": "json_object"},
    )
    raw = (resp.choices[0].message.content or "").strip()
    try:
        return json.loads(raw)
    except Exception:
        logging.warning("Failed to parse model JSON despite JSON mode; returning empty Summary schema.")
        return _safe_deepcopy(SUMMARY_SCHEMA)


# ========= ENTRYPOINT =========
def main(inputblob: func.InputStream, summaryjson: func.Out[str]):
    """
    Trigger: new blob in input-files/{name}

    Outputs:
      - ONLY: output-files/_summary_master.json (running aggregate)
        (We no longer write per-file {name}-summary.json.)

    Behavior changes:
      - If the triggered blob resides under `input-files/data_dictionary/`, we DO NOT process it
        as a document. That folder is reserved solely for loading field definitions.

    Note:
      - `summaryjson` output binding is intentionally unused; remove from function.json
        if you want to drop the binding entirely.
    """
    logging.info("Triggered by blob: %s (%s bytes)", inputblob.name, inputblob.length)

    # --------- Skip dictionary files ----------
    rel_path = inputblob.name
    if rel_path.startswith(f"{INPUT_CONTAINER}/"):
        rel_path = rel_path[len(f"{INPUT_CONTAINER}/"):]
    if rel_path.startswith("data_dictionary/"):
        logging.info("Skipping document processing for dictionary file: %s", inputblob.name)
        return

    # 1) Extract text from the incoming file
    try:
        raw = inputblob.read()
    except Exception:
        logging.exception("Failed reading input blob stream")
        return
    doc_text = _extract_text(raw, blob_name=inputblob.name)

    # 2) Load data dictionary guidance (reads ONLY DICT_BLOB_NAME)
    dictionary_guidance = _load_dictionary_guidance()

    logging.info(_load_dictionary_guidance)

    # 3) LLM proposal for this file
    try:
        proposal = _llm_fill_summary(doc_text, dictionary_guidance)
    except Exception as e:
        logging.exception("LLM call failed")
        proposal = _safe_deepcopy(SUMMARY_SCHEMA)
        proposal["error"] = str(e)

    # 4) Merge into master (concurrency safe, no overwrite of non-empty)
    try:
        current_master, etag = _load_master_state_with_etag()
        merged = _merge_no_overwrite(current_master, proposal)
        _save_master_state_concurrent_safe(merged, etag)
        logging.info("Updated master schema at %s/%s", OUTPUT_CONTAINER, MASTER_BLOB_NAME)
    except Exception:
        logging.exception("Failed to update the master summary file")
