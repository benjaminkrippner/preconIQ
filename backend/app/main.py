"""FastAPI application entry point.

Routers are imported from the reorganized api package. The inclusion order and
prefixes mirror the legacy implementation so HTTP behavior remains unchanged.
"""
from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware

from .api.routers import geotech_bot, geotech_research_assistant

app = FastAPI(title="PreconIQ APIs", version="1.0")

# CORS (loosen for development; restrict in production)
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # set env-driven list later if needed
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# Mount routers
app.include_router(
    geotech_research_assistant.router,
    prefix="/geotech",
    tags=["Geotech Research"],
)
app.include_router(
    geotech_bot.router,
    prefix="/geotech",
    tags=["Geotech Assistant"],
)


@app.get("/", tags=["Health"])
def root() -> dict:
    """Basic root that confirms the API is alive."""
    return {"message": "Precon API suite is running"}


@app.get("/healthz", tags=["Health"])
def health() -> dict:
    """K8s-style health endpoint."""
    return {"status": "ok"}
