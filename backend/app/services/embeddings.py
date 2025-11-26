"""Embedding helpers for hybrid/vector search.

Moved under the services namespace; behavior mirrors the original utility
module.
"""
from typing import List, Optional

from ..infra.azure_clients import AOAI_EMBED_DEPLOYMENT, embed_client


def embed_query(text: str) -> Optional[List[float]]:
    """Embed the query text using Azure OpenAI embeddings if configured."""
    if not embed_client or not AOAI_EMBED_DEPLOYMENT:
        return None
    resp = embed_client.embeddings.create(model=AOAI_EMBED_DEPLOYMENT, input=[text])
    return resp.data[0].embedding
