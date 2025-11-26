"""Geotech research assistant endpoints backed by Azure AI Search."""

from __future__ import annotations

import logging
import os
from typing import Optional, List, Dict, Any

from fastapi import APIRouter, Body, Query, HTTPException

from ..core.azure_clients import (
    search_client,
    USE_VECTOR,
    SEMANTIC_CONFIGURATION_NAME,
)
from ..core.config import AOAI_ENDPOINT, AOAI_KEY, AOAI_API_VERSION
from ..models.search_models import SearchResponse, SearchResponseItem, AnswerRequest
from ..utils.filters import and_join, eq, build_bbox_filter, build_radius_filter
from ..utils.embeddings import embed_query

try:  # pragma: no cover - optional dependency for local dev
    from azure.search.documents.models import VectorizedQuery, QueryType
except ModuleNotFoundError:  # pragma: no cover - optional dependency for local dev
    VectorizedQuery = None  # type: ignore[assignment]
    QueryType = None  # type: ignore[assignment]

try:  # pragma: no cover - optional dependency for local dev
    from openai import AzureOpenAI
except ModuleNotFoundError:  # pragma: no cover - optional dependency for local dev
    AzureOpenAI = None  # type: ignore[assignment]


logger = logging.getLogger(__name__)

gen_client: AzureOpenAI | None = None
if AOAI_ENDPOINT and AOAI_KEY and AzureOpenAI:
    try:
        gen_client = AzureOpenAI(
            azure_endpoint=AOAI_ENDPOINT,
            api_key=AOAI_KEY,
            api_version=AOAI_API_VERSION,
        )
    except Exception as exc:  # pragma: no cover - optional dependency
        logger.warning("Failed to initialize Azure OpenAI generative client: %s", exc)
        gen_client = None

if AOAI_ENDPOINT and AOAI_KEY and not AzureOpenAI:
    logger.warning("OpenAI SDK is not installed. Answer generation will be disabled.")


router = APIRouter()


@router.get("/search", response_model=SearchResponse)
def search_geotech(
    q: str = Query("", description="Search text. If empty and use_vector=true, vector-only search will run."),
    top: int = Query(10, ge=1, le=50),
    skip: int = Query(0, ge=0),

    # Geo inputs from map
    min_lat: Optional[float] = Query(None),
    max_lat: Optional[float] = Query(None),
    min_lon: Optional[float] = Query(None),
    max_lon: Optional[float] = Query(None),

    lat: Optional[float] = Query(None),
    lon: Optional[float] = Query(None),
    radius_km: Optional[float] = Query(None, description="Radius in kilometers"),

    # UI toggles
    limit_region: bool = Query(False, description="If true, apply the provided bbox or radius as a hard filter."),
    search_full_db: bool = Query(False, description="If true, ignore map scope and query the full index."),

    # Metadata filters
    state: Optional[str] = Query(None),
    status: Optional[str] = Query(None),
    segment: Optional[str] = Query(None),
    owner: Optional[str] = Query(None),
    project_number: Optional[str] = Query(None),
    project_name: Optional[str] = Query(None),

    # Retrieval knobs
    use_vector: bool = Query(USE_VECTOR),
    use_semantic: bool = Query(True),
    include_facets: bool = Query(True),
    include_answer: bool = Query(False),
    orderby: Optional[str] = Query(None, description="e.g., 'last_modified desc' if available"),
) -> SearchResponse:
    if search_client is None:
        raise HTTPException(status_code=503, detail="Azure Cognitive Search client is not configured.")

    # Geo filter
    geo_filter = None
    if not search_full_db and limit_region:
        geo_filter = build_radius_filter(lat, lon, radius_km) or build_bbox_filter(min_lat, max_lat, min_lon, max_lon)
        if geo_filter is None:
            raise HTTPException(status_code=400, detail="limit_region is true but no valid lat/lon/radius or bbox was provided.")

    # Combined filter
    filters = [
        geo_filter,
        eq("state", state),
        eq("status", status),
        eq("segment", segment),
        eq("owner", owner),
        eq("project_number", project_number),
        eq("project_name", project_name),
    ]
    filter_str = and_join(filters)

    # Hybrid vector + lexical
    vector_queries = None
    search_text = q
    if use_vector and VectorizedQuery:
        vec = embed_query(q if q else "geotechnical context")
        if vec:
            vector_queries = [VectorizedQuery(vector=vec, k_nearest_neighbors=top, fields="content_vector")]
            if not q:
                search_text = ""  # vector-only
    elif use_vector and not VectorizedQuery:
        logger.debug("Vector search requested but azure-search-documents is not installed.")

    # Build query kwargs
    facet_fields = ["owner,count:50", "state,count:60", "segment,count:20", "status,count:20", "project_number,count:200"]
    query_kwargs: Dict[str, Any] = {
        "search_text": search_text,
        "filter": filter_str,
        "top": top,
        "skip": skip,
        "vector_queries": vector_queries,
        "include_total_count": True,
    }
    if include_facets:
        query_kwargs["facets"] = facet_fields
    if use_semantic and QueryType:
        query_kwargs["query_type"] = QueryType.SEMANTIC
        query_kwargs["semantic_configuration_name"] = SEMANTIC_CONFIGURATION_NAME
        query_kwargs["query_caption"] = "extractive|highlight-false"
        if include_answer:
            query_kwargs["query_answer"] = "extractive|count-1"
    elif use_semantic and not QueryType:
        logger.debug("Semantic search requested but azure-search-documents is not installed.")

    if orderby:
        query_kwargs["orderby"] = orderby

    try:
        results = search_client.search(**query_kwargs)

        items: List[SearchResponseItem] = []
        for r in results:
            sp_url = r.get("sharepoint_url") or ""
            page = r.get("page_start") or None
            link = f"{sp_url}#page={page}" if sp_url and page else (sp_url or None)

            caption = None
            captions = r.get("@search.captions")
            if captions:
                # SDK may return a single QueryCaptionResult or a list of them
                if isinstance(captions, list):
                    first_caption = captions[0]
                else:
                    first_caption = captions
                # Handle both object and dict cases safely
                if hasattr(first_caption, "text"):
                    caption = first_caption.text
                elif isinstance(first_caption, dict):
                    caption = first_caption.get("text")

            content = r.get("content") or ""
            snippet = content[:600] + ("…" if len(content) > 600 else "")

            items.append(
                SearchResponseItem(
                    id=r["id"],
                    score=r.get("@search.score"),
                    caption=caption,
                    project_number=r.get("project_number"),
                    project_name=r.get("project_name"),
                    owner=r.get("owner"),
                    state=r.get("state"),
                    status=r.get("status"),
                    segment=r.get("segment"),
                    lat=r.get("lat"),
                    lon=r.get("lon"),
                    page_start=r.get("page_start"),
                    page_end=r.get("page_end"),
                    snippet=snippet,
                    link=link,
                    sharepoint_url=sp_url,
                )
            )

        # Total, facets, answers
        try:
            total = results.get_count()
        except Exception:
            total = getattr(results, "total_count", None)

        facets = getattr(results, "facets", None)
        answer_text = None
        if include_answer:
            ans = getattr(results, "answers", None)
            if ans and isinstance(ans, list) and len(ans) > 0:
                answer_text = ans[0].get("text") or None

        next_skip = skip + top if (total is not None and skip + top < total) else None

        return SearchResponse(
            count=total,
            results=items,
            facets=facets,
            answer=answer_text,
            next_skip=next_skip,
        )
    except Exception as exc:
        raise HTTPException(status_code=500, detail=f"Search error: {type(exc).__name__}: {exc}") from exc


def generate_rag_answer(req: AnswerRequest) -> Dict[str, Any]:
    """Generate a synthesized answer using Azure Search + Azure OpenAI with optional regional + metadata filters."""
    if search_client is None:
        raise HTTPException(status_code=503, detail="Azure Cognitive Search client is not configured.")
    if gen_client is None:
        raise HTTPException(status_code=503, detail="Azure OpenAI client is not configured.")
    # Build geo filter per toggles
    geo_filter = None
    if not req.search_full_db and req.limit_region:
        geo_filter = (
            build_radius_filter(req.lat, req.lon, req.radius_km)
            or build_bbox_filter(req.min_lat, req.max_lat, req.min_lon, req.max_lon)
        )
        if geo_filter is None:
            raise HTTPException(
                status_code=400,
                detail="limit_region is true but no valid lat/lon/radius or bbox was provided."
            )

    # Combine filters for Azure AI Search index
    filter_str = and_join([
        geo_filter,
        eq("state", req.state),
        eq("status", req.status),
        eq("segment", req.segment),
        eq("owner", req.owner),
        eq("project_number", req.project_number),
        eq("project_name", req.project_name),
    ])

    # Optional hybrid vector retrieval
    vector_queries = None
    if req.use_vector and VectorizedQuery:
        vec = embed_query(req.question if req.question else "geotechnical context")
        if vec:
            vector_queries = [VectorizedQuery(vector=vec, k_nearest_neighbors=req.top, fields="content_vector")]
    elif req.use_vector and not VectorizedQuery:
        logger.debug("Vector answer request but azure-search-documents is not installed.")

    # Search kwargs
    query_kwargs: Dict[str, Any] = {
        "search_text": (req.question or "*"),
        "top": req.top,
        "include_total_count": False,
        "vector_queries": vector_queries,
        "filter": filter_str,
    }
    if req.use_semantic:
        query_kwargs["query_type"] = "semantic"
        query_kwargs["semantic_configuration_name"] = SEMANTIC_CONFIGURATION_NAME
        query_kwargs["query_caption"] = "extractive|highlight-false"
        if req.include_answer:
            query_kwargs["query_answer"] = "extractive|count-1"

    # Retrieve the relavent chunks
    try:
        results = search_client.search(**query_kwargs)
    except Exception as exc:  # pragma: no cover
        raise HTTPException(status_code=500, detail=f"Search error: {type(exc).__name__}: {exc}") from exc

    # Collect chunks + citations
    chunks: List[str] = []
    citations: List[Dict[str, Any]] = []

    for r in results:
        # caption-first, fallback to content
        text = None
        if req.use_semantic:
            caps = r.get("@search.captions")
            if caps:
                first = caps[0] if isinstance(caps, list) else caps
                if hasattr(first, "text"):
                    text = first.text
                elif isinstance(first, dict):
                    text = first.get("text")
        if not text:
            text = (r.get("content") or "")[:1200]

        proj = r.get("project_name") or r.get("project_number") or "Unknown Project"
        where = f"{proj} | {r.get('owner') or ''} | {r.get('state') or ''}".strip(" |")
        page = r.get("page_start")
        ref = f"[{r.get('project_number') or ''}:{page or '?'}]"

        chunks.append(f"{ref} {where}\n{text}")
        citations.append({
            "id": r.get("id"),
            "project_number": r.get("project_number"),
            "project_name": r.get("project_name"),
            "state": r.get("state"),
            "owner": r.get("owner"),
            "page_start": page,
            "page_end": r.get("page_end"),
            "link": (f"{(r.get('sharepoint_url') or '')}#page={page}" if r.get('sharepoint_url') and page else r.get('sharepoint_url')),
            "score": r.get("@search.score"),
            "snippet": text,
        })

    if not chunks:
        return {"answer": "No relevant passages found in the geotechnical index.", "citations": []}

    # Combine the context from retrieved chunks
    context = ("\n\n---\n\n").join(chunks)

    # Synthesize answer with Azure OpenAI
    try:
        system_prompt = (
            "You are a geotechnical research assistant for utility-scale renewables. "
            "Answer concisely and numerically where possible. "
            "Only use the provided context. If unsure, say so. "
            "Include short inline refs like [projectName:page] when citing."
        )
        user_prompt = f"Question: {req.question}\n\nContext:\n{context}"

        completion = gen_client.chat.completions.create(
            model=os.getenv("AZURE_OPENAI_CHAT_DEPLOYMENT", "gpt-4o-mini"),
            messages=[
                {"role": "system", "content": system_prompt},
                {"role": "user", "content": user_prompt},
            ],
            temperature=0.2,
            max_tokens=3000,
        )
        answer_text = completion.choices[0].message.content
    except Exception as exc:  # pragma: no cover
        raise HTTPException(status_code=500, detail=f"Generation error: {type(exc).__name__}: {exc}") from exc

    return {"answer": answer_text, "citations": citations}


@router.post("/answer")
def rag_answer(req: AnswerRequest = Body(...)) -> Dict[str, Any]:
    """Generate a synthesized answer using Azure Search + Azure OpenAI with optional regional + metadata filters."""
    return generate_rag_answer(req)
@router.get("/suggest")
def suggest(
    term: str = Query(..., min_length=1),
    top: int = Query(8, ge=1, le=20),
) -> Dict[str, List[str]]:
    """Type-ahead suggestions over project_name/owner/state/segment (requires a suggester named 'sg')."""
    if search_client is None:
        raise HTTPException(status_code=503, detail="Azure Cognitive Search client is not configured.")
    try:
        res = search_client.suggest(search_text=term, suggester_name="sg", top=top, use_fuzzy_matching=True)
        return {"suggestions": [r["text"] for r in res]}
    except Exception as exc:  # pragma: no cover
        raise HTTPException(status_code=500, detail=f"Suggest error: {type(exc).__name__}: {exc}") from exc
