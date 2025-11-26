"""Lightweight Geotech bot endpoints for local development.

The router still streams Server-Sent Events exactly like the legacy module; the
only change is the package location under `backend.app.api`.
"""

from __future__ import annotations

import json
import logging
import re
from collections.abc import Iterable
from concurrent.futures import ThreadPoolExecutor, TimeoutError as FuturesTimeoutError
from functools import lru_cache
from pathlib import Path
from typing import Any, Dict, List, Tuple
from uuid import uuid4

from fastapi import APIRouter, HTTPException
from fastapi.responses import StreamingResponse
from pydantic import AliasChoices, BaseModel, ConfigDict, Field, ValidationError, field_validator

from ...schemas.geotech import AnswerRequest
from . import geotech_research_assistant

router = APIRouter()
logger = logging.getLogger(__name__)

print("=" * 60)
print("GEOTECH_BOT MODULE LOADED - Router initialized")
print("=" * 60)


class GeotechAgentCitation(BaseModel):
    """Citation details returned to the frontend chat client."""
    title: str | None = None
    url: str | None = None
    snippet: str | None = None


class GeotechAgentMessageRequest(BaseModel):
    """Incoming chat message from the UI."""

    model_config = ConfigDict(populate_by_name=True)
    prompt: str = Field(..., min_length=1)
    session_id: str | None = Field(
        None,
        alias="sessionId",
        validation_alias=AliasChoices("sessionId", "session_id"),
    )
    search_scope: str | None = Field(
        default=None,
        alias="searchScope",
        validation_alias=AliasChoices("searchScope", "search_scope"),
    )
    selection_project_ids: List[str] = Field(
        default_factory=list,
        alias="selectionProjectIds",
        validation_alias=AliasChoices("selectionProjectIds", "selection_project_ids"),
    )

    @field_validator("search_scope")
    @classmethod
    def _normalize_search_scope(cls, value: str | None) -> str | None:
        if value is None:
            return None

        normalized = value.strip().lower()
        if normalized not in {"selection", "all"}:
            raise ValueError("search_scope must be 'selection' or 'all'")
        return normalized

    @field_validator("selection_project_ids", mode="before")
    @classmethod
    def _normalize_selection_ids(cls, value: object) -> List[str]:
        if value is None:
            return []

        if isinstance(value, (str, bytes)):
            candidates = [value]
        else:
            try:
                candidates = list(value)
            except TypeError:
                candidates = [value]

        unique: List[str] = []
        seen = set()
        for candidate in candidates:
            if not isinstance(candidate, str):
                continue

            trimmed = candidate.strip()
            if not trimmed or trimmed in seen:
                continue

            unique.append(trimmed)
            seen.add(trimmed)

        return unique


class GeotechAgentMessageResponse(BaseModel):
    """Response payload delivered back to the UI."""
    model_config = ConfigDict(populate_by_name=True)
    session_id: str = Field(..., alias="sessionId")
    message: str
    citations: List[GeotechAgentCitation] = Field(default_factory=list)


PROJECT_ROOT = Path(__file__).resolve().parents[3]
DATA_PATH = PROJECT_ROOT / "public" / "data" / "geotech-projects.json"
HANDSHAKE_KEYWORDS = ("connection check", "handshake", "ping")


def _truncate(value: str, limit: int = 240) -> str:
    if len(value) <= limit:
        return value
    truncated = value[: limit - 1].rsplit(" ", 1)[0]
    return f"{truncated}…"


@lru_cache(maxsize=1)
def _load_projects() -> List[Dict[str, Any]]:
    if not DATA_PATH.exists():
        logger.warning(f"Demo data not found at: {DATA_PATH}")
        return []
    try:
        with DATA_PATH.open("r", encoding="utf-8") as handle:
            data = json.load(handle)
        return [entry for entry in data if isinstance(entry, dict)]
    except Exception as e:
        logger.error(f"Error loading projects: {e}")
        return []


def _tokenize(query: str) -> List[str]:
    tokens = re.findall(r"[A-Za-z0-9]+", query.lower())
    return [token for token in tokens if len(token) > 2]


def _score_project(project: Dict[str, Any], tokens: List[str]) -> int:
    haystack_parts = [
        project.get("projectName", ""),
        project.get("projectNumber", ""),
        project.get("owner", ""),
        project.get("segment", ""),
        project.get("location", ""),
        project.get("overview", ""),
    ]
    haystack = " ".join(filter(None, haystack_parts)).lower()
    score = 0
    for token in tokens:
        if not token:
            continue
        occurrences = haystack.count(token)
        if occurrences:
            score += occurrences * 5
        elif token in haystack:
            score += 1
    return score


def _rank_projects(query: str, limit: int = 3) -> List[Dict[str, Any]]:
    projects = _load_projects()
    if not projects:
        return []
    tokens = _tokenize(query)
    if not tokens:
        return projects[:limit]
    ranked: List[Tuple[int, Dict[str, Any]]] = []
    for project in projects:
        score = _score_project(project, tokens)
        if score:
            ranked.append((score, project))
    ranked.sort(key=lambda item: item[0], reverse=True)
    return [project for _, project in ranked[:limit]]


def _format_project_line(project: Dict[str, Any]) -> str:
    project_name = project.get("projectName") or project.get("project_number") or "Unknown project"
    owner = project.get("owner") or "Owner TBD"
    location = project.get("location") or project.get("state") or "Location TBD"
    status = project.get("status") or "status unknown"
    return f"• {project_name} — {owner} | {location} ({status})"


def _build_citation(project: Dict[str, Any]) -> GeotechAgentCitation:
    title = project.get("projectName") or project.get("project_number")
    url = project.get("sharepointPath") or project.get("sharepoint_url") or None
    if isinstance(url, str):
        url = url.strip() or None
    overview = project.get("overview") or project.get("snippet") or ""
    snippet = _truncate(str(overview).strip()) if overview else None
    return GeotechAgentCitation(title=title, url=url, snippet=snippet)


def _build_project_response(prompt: str) -> Tuple[str, List[GeotechAgentCitation]]:
    ranked = _rank_projects(prompt)
    if not ranked:
        message = "No matching projects found in the demo dataset. Try searching for owner names, locations, or project types."
        return message, []
    lines = [_format_project_line(project) for project in ranked]
    message = (
        "Here are relevant geotechnical reports from the demo dataset:\n"
        + "\n".join(lines)
        + "\n\nAsk about owner, location, or status to refine."
    )
    citations = [_build_citation(project) for project in ranked]
    return message, citations


def _build_research_citation(entry: Dict[str, Any]) -> GeotechAgentCitation | None:
    """Build citation from Azure Search result."""
    title = entry.get("project_name") or entry.get("project_number") or entry.get("title")
    if isinstance(title, str):
        title = title.strip() or None
    else:
        title = None

    url = entry.get("link") or entry.get("url")
    if isinstance(url, str):
        url = url.strip() or None
    else:
        url = None

    snippet = entry.get("snippet")
    if isinstance(snippet, str):
        snippet = snippet.strip() or None
    else:
        snippet = None

    if not snippet:
        details: List[str] = []
        owner = entry.get("owner")
        state = entry.get("state")
        page = entry.get("page_start")
        if isinstance(owner, str) and owner.strip():
            details.append(owner.strip())
        if isinstance(state, str) and state.strip():
            details.append(state.strip())
        if isinstance(page, (int, float)):
            page_number = int(page)
            details.append(f"Page {page_number}")
        snippet = " • ".join(details) if details else None

    if not any([title, url, snippet]):
        return None

    return GeotechAgentCitation(title=title, url=url, snippet=snippet)


def _format_sse_event(data: Dict[str, Any], event: str | None = None) -> str:
    payload = json.dumps(data, ensure_ascii=False)
    if event:
        return f"event: {event}\ndata: {payload}\n\n"
    return f"data: {payload}\n\n"


def _serialize_citations(citations: Iterable[GeotechAgentCitation]) -> List[Dict[str, Any]]:
    serialized: List[Dict[str, Any]] = []
    for citation in citations:
        try:
            serialized.append(citation.model_dump())
        except Exception:
            serialized.append({
                "title": citation.title,
                "url": citation.url,
                "snippet": citation.snippet,
            })
    return serialized


def _build_answer_request(
    payload: GeotechAgentMessageRequest,
    prompt: str,
) -> AnswerRequest:
    """Normalize incoming payload into an ``AnswerRequest`` for Azure Search."""

    selection_ids: List[str] = []
    if payload.search_scope == "selection":
        selection_ids = payload.selection_project_ids

    try:
        return AnswerRequest(
            question=prompt,
            top=6,
            include_answer=True,
            search_scope=payload.search_scope,
            selection_project_ids=selection_ids,
        )
    except ValidationError as exc:
        logger.error("Invalid request: %s", exc)
        raise HTTPException(status_code=400, detail="Invalid request format") from exc


def _query_research_assistant_with_timeout(
    payload: GeotechAgentMessageRequest,
    prompt: str,
    timeout: int = 30,
) -> Tuple[str, List[GeotechAgentCitation]]:
    """Query Azure with timeout - Windows-compatible version."""

    # Check if Azure clients are available
    if not hasattr(geotech_research_assistant, 'search_client') or geotech_research_assistant.search_client is None:
        raise HTTPException(status_code=503, detail="Azure Search client not configured")
    
    if not hasattr(geotech_research_assistant, 'gen_client') or geotech_research_assistant.gen_client is None:
        raise HTTPException(status_code=503, detail="Azure OpenAI client not configured")
    
    # Create request
    request = _build_answer_request(payload, prompt)
    
    # Call Azure with timeout using ThreadPoolExecutor (Windows-compatible)
    def _call_azure():
        return geotech_research_assistant.generate_rag_answer(request)
    
    with ThreadPoolExecutor(max_workers=1) as executor:
        future = executor.submit(_call_azure)
        try:
            result = future.result(timeout=timeout)
        except FuturesTimeoutError:
            logger.warning(f"Azure call timed out after {timeout}s")
            raise TimeoutError(f"Azure research assistant timed out after {timeout} seconds")
        except Exception as e:
            logger.error(f"Azure call failed: {e}")
            raise
    
    # Process result
    answer = result.get("answer", "")
    message = answer.strip() if isinstance(answer, str) else ""
    
    citations_payload = result.get("citations", [])
    citations: List[GeotechAgentCitation] = []
    if isinstance(citations_payload, list):
        for item in citations_payload:
            if isinstance(item, dict):
                citation = _build_research_citation(item)
                if citation:
                    citations.append(citation)
    
    if not message:
        message = "No relevant passages found in the geotechnical index."
    
    return message, citations


def _stream_research_assistant(
    payload: GeotechAgentMessageRequest,
    prompt: str,
) -> Tuple[Iterable[str], List[GeotechAgentCitation]]:
    if not hasattr(geotech_research_assistant, 'search_client') or geotech_research_assistant.search_client is None:
        raise HTTPException(status_code=503, detail="Azure Search client not configured")

    if not hasattr(geotech_research_assistant, 'gen_client') or geotech_research_assistant.gen_client is None:
        raise HTTPException(status_code=503, detail="Azure OpenAI client not configured")

    request = _build_answer_request(payload, prompt)

    stream_iterable, citations_payload = geotech_research_assistant.generate_rag_answer_stream(request)

    citations: List[GeotechAgentCitation] = []
    if isinstance(citations_payload, list):
        for item in citations_payload:
            if isinstance(item, dict):
                citation = _build_research_citation(item)
                if citation:
                    citations.append(citation)

    return stream_iterable, citations


class _SessionStore:
    """In-memory chat session history."""
    def __init__(self) -> None:
        self._sessions: Dict[str, List[Dict[str, str]]] = {}

    def get_or_create(self, session_id: str | None) -> str:
        if session_id and session_id in self._sessions:
            return session_id
        new_session = uuid4().hex
        self._sessions[new_session] = []
        return new_session

    def append(self, session_id: str, role: str, content: str) -> None:
        history = self._sessions.setdefault(session_id, [])
        history.append({"role": role, "content": content})
        if len(history) > 20:
            del history[:-20]


session_store = _SessionStore()


def _build_handshake_message() -> str:
    return "Geotech assistant online! Ask any question about our Geotechnical data:"


@router.post("/messages/stream")
def chat_with_geotech_bot_stream(payload: GeotechAgentMessageRequest) -> StreamingResponse:
    logger.info("=" * 50)
    logger.info(f"STREAM REQUEST: {payload.prompt}")
    logger.info("=" * 50)

    prompt = payload.prompt.strip()
    if not prompt:
        raise HTTPException(status_code=400, detail="Prompt cannot be empty.")

    session_id = session_store.get_or_create(payload.session_id)
    session_store.append(session_id, "user", prompt)

    def _generator() -> Iterable[str]:
        yield _format_sse_event({"sessionId": session_id}, "session")

        lowered = prompt.lower()
        if any(keyword in lowered for keyword in HANDSHAKE_KEYWORDS):
            message = _build_handshake_message()
            session_store.append(session_id, "assistant", message)
            yield _format_sse_event({"content": message}, "delta")
            yield _format_sse_event(
                {"sessionId": session_id, "message": message, "citations": []},
                "final",
            )
            return

        try:
            stream_iterable, citations = _stream_research_assistant(payload, prompt)
            message_parts: List[str] = []
            for chunk in stream_iterable:
                if not chunk:
                    continue
                message_parts.append(chunk)
                yield _format_sse_event({"content": chunk}, "delta")

            final_message = "".join(message_parts).strip()
            if not final_message:
                final_message = "No relevant passages found in the geotechnical index."

            session_store.append(session_id, "assistant", final_message)
            yield _format_sse_event(
                {
                    "sessionId": session_id,
                    "message": final_message,
                    "citations": _serialize_citations(citations),
                },
                "final",
            )
        except TimeoutError as exc:
            logger.warning(f"Azure timed out: {exc}. Falling back to demo data.")
            message, citations = _build_project_response(prompt)
            session_store.append(session_id, "assistant", message)
            yield _format_sse_event({"content": message}, "delta")
            yield _format_sse_event(
                {
                    "sessionId": session_id,
                    "message": message,
                    "citations": _serialize_citations(citations),
                },
                "final",
            )
        except HTTPException as exc:
            if exc.status_code >= 500 or exc.status_code == 503:
                logger.warning(f"Azure unavailable ({exc.status_code}). Falling back to demo data.")
                message, citations = _build_project_response(prompt)
                session_store.append(session_id, "assistant", message)
                yield _format_sse_event({"content": message}, "delta")
                yield _format_sse_event(
                    {
                        "sessionId": session_id,
                        "message": message,
                        "citations": _serialize_citations(citations),
                    },
                    "final",
                )
            else:
                logger.error("Azure returned HTTP %s: %s", exc.status_code, exc.detail)
                yield _format_sse_event(
                    {
                        "sessionId": session_id,
                        "error": exc.detail or "Request failed.",
                    },
                    "error",
                )
                return
        except Exception as exc:  # pragma: no cover
            logger.exception(f"Azure error: {exc}. Falling back to demo data.")
            message, citations = _build_project_response(prompt)
            session_store.append(session_id, "assistant", message)
            yield _format_sse_event({"content": message}, "delta")
            yield _format_sse_event(
                {
                    "sessionId": session_id,
                    "message": message,
                    "citations": _serialize_citations(citations),
                },
                "final",
            )

    return StreamingResponse(_generator(), media_type="text/event-stream")


@router.post("/messages", response_model=GeotechAgentMessageResponse)
def chat_with_geotech_bot(payload: GeotechAgentMessageRequest) -> GeotechAgentMessageResponse:
    """Chat endpoint with Azure OpenAI + fallback to demo data."""
    
    logger.info("=" * 50)
    logger.info(f"RECEIVED REQUEST: {payload.prompt}")
    logger.info("=" * 50)

    prompt = payload.prompt.strip()
    if not prompt:
        raise HTTPException(status_code=400, detail="Prompt cannot be empty.")

    session_id = session_store.get_or_create(payload.session_id)
    session_store.append(session_id, "user", prompt)

    # Check for handshake
    lowered = prompt.lower()
    if any(keyword in lowered for keyword in HANDSHAKE_KEYWORDS):
        message = _build_handshake_message()
        session_store.append(session_id, "assistant", message)
        return GeotechAgentMessageResponse(session_id=session_id, message=message, citations=[])

    # Try Azure first with 30-second timeout
    try:
        logger.info("Attempting Azure OpenAI query...")
        message, citations = _query_research_assistant_with_timeout(payload, prompt, timeout=30)
        logger.info(f"Azure responded successfully with {len(citations)} citations")
    except TimeoutError as e:
        logger.warning(f"Azure timed out: {e}. Falling back to demo data.")
        message, citations = _build_project_response(prompt)
    except HTTPException as e:
        if e.status_code >= 500 or e.status_code == 503:
            logger.warning(f"Azure unavailable ({e.status_code}). Falling back to demo data.")
            message, citations = _build_project_response(prompt)
        else:
            raise
    except Exception as e:
        logger.exception(f"Azure error: {e}. Falling back to demo data.")
        message, citations = _build_project_response(prompt)
    
    session_store.append(session_id, "assistant", message)
    logger.info(f"Responding with {len(citations)} citations")
    return GeotechAgentMessageResponse(session_id=session_id, message=message, citations=citations)