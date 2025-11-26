"""Pydantic models for /geotech search endpoints."""

from typing import Any, Dict, Iterable, List, Optional

from pydantic import BaseModel, Field, field_validator


class SearchResponseItem(BaseModel):
    """A single search hit from the geotech index."""
    id: str
    score: Optional[float] = None
    caption: Optional[str] = None
    project_number: Optional[str] = None
    project_name: Optional[str] = None
    owner: Optional[str] = None
    state: Optional[str] = None
    status: Optional[str] = None
    segment: Optional[str] = None
    lat: Optional[float] = None
    lon: Optional[float] = None
    page_start: Optional[int] = None
    page_end: Optional[int] = None
    snippet: Optional[str] = None
    link: Optional[str] = None
    sharepoint_url: Optional[str] = None


class SearchResponse(BaseModel):
    """The API response for a search request."""
    count: Optional[int] = None
    results: List[SearchResponseItem]
    facets: Optional[Dict[str, List[Dict[str, Any]]]] = None
    answer: Optional[str] = None
    next_skip: Optional[int] = None  # Client can pass back to request the next page


class AnswerRequest(BaseModel):
    question: str
    top: int = 10
    use_vector: bool = True
    use_semantic: bool = True
    include_answer: bool = False

    # UI toggles (same semantics as /search)
    limit_region: bool = False
    search_full_db: bool = False

    # Optional scope + metadata filters
    search_scope: Optional[str] = None
    selection_project_ids: List[str] = Field(default_factory=list)

    # Region (either center+radius or bbox)
    min_lat: float | None = None
    max_lat: float | None = None
    min_lon: float | None = None
    max_lon: float | None = None
    lat: float | None = None
    lon: float | None = None
    radius_km: float | None = None

    # Optional metadata filters
    state: str | None = None
    status: str | None = None
    segment: str | None = None
    owner: str | None = None
    project_number: str | None = None
    project_name: str | None = None

    @field_validator("search_scope")
    @classmethod
    def _normalize_search_scope(cls, value: Optional[str]) -> Optional[str]:
        if value is None:
            return None

        normalized = value.strip().lower()
        if normalized not in {"selection", "all"}:
            raise ValueError("search_scope must be 'selection' or 'all'")
        return normalized

    @field_validator("selection_project_ids", mode="before")
    @classmethod
    def _normalize_selection_ids(cls, value: Any) -> List[str]:
        if value is None:
            return []

        if isinstance(value, (str, bytes)):
            candidates: Iterable[Any] = [value]
        else:
            try:
                candidates = list(value)  # type: ignore[arg-type]
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

