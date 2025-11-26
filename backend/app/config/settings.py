"""Environment configuration for the Geotech API.

Centralizes env lookups so other modules can import constants from here.
No external dependency on pydantic-settings to keep the footprint minimal.
"""
import os
from pathlib import Path
from typing import List

from dotenv import load_dotenv

# -------------------------------------------------------------------
# Load environment variables from one level above the app directory
# -------------------------------------------------------------------
PROJECT_ROOT = Path(__file__).resolve().parents[2]
ENV_PATH = PROJECT_ROOT / ".env"

if ENV_PATH.exists():
    load_dotenv(dotenv_path=ENV_PATH, override=False)
    print(f"[OK] Loaded .env from: {ENV_PATH}")
    print(f"[DEBUG] AZURE_OPENAI_ENDPOINT: {os.getenv('AZURE_OPENAI_ENDPOINT', 'NOT SET')}")
    print(f"[DEBUG] AZURE_SEARCH_SERVICE_ENDPOINT: {os.getenv('AZURE_SEARCH_SERVICE_ENDPOINT', 'NOT SET')}")
    api_key = os.getenv('AZURE_OPENAI_API_KEY', '')
    print(f"[DEBUG] AZURE_OPENAI_API_KEY: {'***' + api_key[-4:] if api_key else 'NOT SET'}")
else:
    print(f"[ERROR] .env not found at expected location: {ENV_PATH}")


def _get_env(name: str, default: str | None = None, required: bool = False) -> str | None:
    """Fetch an environment variable, with optional required flag."""
    val = os.getenv(name, default)
    if required and not val:
        raise RuntimeError(f"Missing required environment variable: {name}")
    return val


# Azure AI Search (optional for local development)
SEARCH_ENDPOINT: str | None = _get_env("AZURE_SEARCH_SERVICE_ENDPOINT")
SEARCH_INDEX: str = _get_env("AZURE_SEARCH_INDEX_NAME", "geotech-index") or "geotech-index"
SEARCH_API_KEY: str | None = _get_env("AZURE_SEARCH_API_KEY")

# Azure OpenAI (optional for vector/hybrid search)
AOAI_ENDPOINT: str | None = _get_env("AZURE_OPENAI_ENDPOINT")
AOAI_KEY: str | None = _get_env("AZURE_OPENAI_API_KEY")
AOAI_DEPLOYMENT: str | None = _get_env("AZURE_OPENAI_EMBED_DEPLOYMENT")
AOAI_API_VERSION: str = _get_env("AZURE_OPENAI_API_VERSION", "2024-05-01-preview") or "2024-05-01-preview"

# Retrieval configuration
USE_VECTOR_DEFAULT: bool = bool(int(_get_env("USE_VECTOR", "1") or "1"))
SEMANTIC_CONFIG: str = _get_env("SEMANTIC_CONFIG", "geotech-semantic") or "geotech-semantic"


# CORS

def cors_origins() -> List[str]:
    """Return allowed CORS origins from env or wildcard."""
    raw = _get_env("CORS_ALLOW_ORIGINS", "*") or "*"
    return [origin.strip() for origin in raw.split(",") if origin.strip()]
