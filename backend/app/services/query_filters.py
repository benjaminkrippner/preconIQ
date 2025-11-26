"""Helpers for constructing Azure Cognitive Search filter expressions.

Logic is unchanged; the module simply moved out of the utils namespace to
clarify its service role.
"""
from typing import List, Optional


def and_join(parts: List[Optional[str]]) -> Optional[str]:
    """Join non-empty filter fragments with 'and'."""
    combined = [p for p in parts if p]
    return " and ".join(combined) if combined else None


def eq(field: str, value: Optional[str]) -> Optional[str]:
    """Return an OData equality expression for a string field, with proper quoting."""
    if not value:
        return None
    escaped = value.replace("'", "''")
    return f"{field} eq '{escaped}'"


def build_bbox_filter(
    min_lat: Optional[float],
    max_lat: Optional[float],
    min_lon: Optional[float],
    max_lon: Optional[float],
) -> Optional[str]:
    """Create a bounding box filter on lat/lon if all values are present."""
    if None in (min_lat, max_lat, min_lon, max_lon):
        return None
    return f"lat ge {min_lat} and lat le {max_lat} and lon ge {min_lon} and lon le {max_lon}"


def build_radius_filter(
    lat: Optional[float],
    lon: Optional[float],
    radius_km: Optional[float],
) -> Optional[str]:
    """Create a geo.radius filter using the geography POINT and a radius in kilometers."""
    if None in (lat, lon, radius_km):
        return None
    return f"geo.distance(location, geography'POINT({lon} {lat})') le {radius_km}"
