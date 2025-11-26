"""Singleton Azure clients for Search and Azure OpenAI embeddings.

Module moved under the infra namespace; imports updated to pull from the new
config.settings location while keeping runtime semantics identical.
"""

from __future__ import annotations

import logging
from typing import Optional

try:  # pragma: no cover - optional dependency for local dev
    from azure.core.credentials import AzureKeyCredential
    from azure.search.documents import SearchClient
except ModuleNotFoundError:  # pragma: no cover - optional dependency for local dev
    AzureKeyCredential = None  # type: ignore[assignment]
    SearchClient = None  # type: ignore[assignment]

try:  # pragma: no cover - optional dependency for local dev
    from openai import AzureOpenAI
except ModuleNotFoundError:  # pragma: no cover - optional dependency for local dev
    AzureOpenAI = None  # type: ignore[assignment]

from ..config.settings import (
    SEARCH_ENDPOINT,
    SEARCH_INDEX,
    SEARCH_API_KEY,
    AOAI_ENDPOINT,
    AOAI_KEY,
    AOAI_DEPLOYMENT,
    AOAI_API_VERSION,
    USE_VECTOR_DEFAULT,
    SEMANTIC_CONFIG,
)

logger = logging.getLogger(__name__)

# Exported constants (used by routers/utils)
USE_VECTOR = USE_VECTOR_DEFAULT
SEMANTIC_CONFIGURATION_NAME = SEMANTIC_CONFIG
AOAI_EMBED_DEPLOYMENT = AOAI_DEPLOYMENT

# Search client (single instance)
search_client: Optional["SearchClient"] = None
if SEARCH_ENDPOINT and SEARCH_API_KEY and SearchClient and AzureKeyCredential:
    try:
        search_client = SearchClient(
            endpoint=SEARCH_ENDPOINT,
            index_name=SEARCH_INDEX,
            credential=AzureKeyCredential(SEARCH_API_KEY),
        )
        logger.info("[OK] Azure Search client initialized successfully")
    except Exception as exc:  # pragma: no cover - optional dependency
        logger.warning("Failed to initialize Azure Search client: %s", exc)
        search_client = None
elif SEARCH_ENDPOINT and SEARCH_API_KEY:
    logger.warning(
        "Azure Search dependencies are not installed. Search endpoints will be disabled.",
    )

# Azure OpenAI embeddings client (optional)
embed_client: Optional["AzureOpenAI"] = None
if AOAI_ENDPOINT and AOAI_KEY and AOAI_DEPLOYMENT and AzureOpenAI:
    try:
        embed_client = AzureOpenAI(
            azure_endpoint=AOAI_ENDPOINT,
            api_key=AOAI_KEY,
            api_version=AOAI_API_VERSION,
            timeout=30.0,  # 30 second timeout
            max_retries=2,
        )
        logger.info("[OK] Azure OpenAI embed client initialized successfully")
    except Exception as exc:  # pragma: no cover - optional dependency
        logger.warning("Failed to initialize Azure OpenAI client: %s", exc)
        embed_client = None
elif AOAI_ENDPOINT and AOAI_KEY and AOAI_DEPLOYMENT:
    logger.warning(
        "OpenAI SDK is not installed. Vector embedding support will be disabled.",
    )