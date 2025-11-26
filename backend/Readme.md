# 🧠 Precon AI Backend

The **Precon AI Backend** hosts modular FastAPI microservices that power AI-driven tools for the Blattner **PreconIQ Platform**.
The first service implemented is the **Geotech Research Assistant**, which provides intelligent retrieval and Q&A over geotechnical reports indexed in **Azure AI Search**.

---

## 📁 Project Structure

```
geotech-api/
│
├── app/
│   ├── main.py                             # FastAPI entrypoint (registers routers)
│   ├── core/                               # Core configuration & shared clients
│   │   ├── config.py                       # Loads .env (from one directory up)
│   │   └── azure_clients.py                # Azure Search + OpenAI client init
│   ├── models/                             # Pydantic models for request/response
│   │   └── search_models.py
│   ├── utils/                              # Shared helpers
│   │   ├── filters.py                      # Filter and OData query builders
│   │   └── embeddings.py                   # Query embedding helper
│   └── routers/
│       └── geotech_research_assistant.py   # /geotech API endpoints
│
├── .env                                    # Environment variables (at project root)
├── requirements.txt
└── README.md
```

---

## ⚙️ Environment Configuration

All environment variables are read automatically from the `.env` file located at the top-level **`/backend`** folder.
This happens via `load_dotenv()` in `app/core/config.py`.

### Example `.env`

```bash
# Azure AI Search
AZURE_SEARCH_SERVICE_ENDPOINT=https://precon-geotech-search-service.search.windows.net
AZURE_SEARCH_API_KEY=<your-key>
AZURE_SEARCH_INDEX_NAME=precon-geotech-search-service

# Azure OpenAI (optional: for hybrid/vector search)
AZURE_OPENAI_ENDPOINT=https://<your-openai>.openai.azure.com
AZURE_OPENAI_API_KEY=<your-aoai-key>
AZURE_OPENAI_EMBED_DEPLOYMENT=text-embedding-3-small
AZURE_OPENAI_API_VERSION=2024-05-01-preview

# Retrieval & CORS
USE_VECTOR=1
SEMANTIC_CONFIG=geotech-semantic
CORS_ALLOW_ORIGINS=*
```

> 💡 **Tip:** The `.env` file should be in the repo root (not inside `/app`) so both Uvicorn and Azure Function environments can detect it easily.

---

## 🧠 Service Overview — Geotech Research Assistant

### Endpoints

| Route              | Description                                                                                                                          |
| ------------------ | ------------------------------------------------------------------------------------------------------------------------------------ |
| `/geotech/search`  | Hybrid (vector + semantic) search across indexed geotechnical reports. Supports filters, map-region queries, and extractive answers. |
| `/geotech/answer`  | RAG-style conversational response to a user question based on indexed geotechnical reports in Azure AI Search                        |
| `/geotech/suggest` | Typeahead suggestions for project names, owners, or segments.                                                                        |

### Supported Query Parameters

| Param                                           | Type   | Description                                                               |
| ----------------------------------------------- | ------ | ------------------------------------------------------------------------- |
| `q`                                             | string | Search text or natural language question                                  |
| `limit_region`                                  | bool   | Restrict search to selected map region (uses `lat/lon/radius_km` or bbox) |
| `search_full_db`                                | bool   | Ignore map filters and search all indexed reports                         |
| `lat/lon/radius_km`                             | float  | Center and radius for geographic filtering                                |
| `state, owner, segment, status, project_number` | string | Metadata filters                                                          |
| `use_vector`                                    | bool   | Enable vector embeddings for hybrid search                                |
| `use_semantic`                                  | bool   | Use Azure semantic re-ranking                                             |
| `include_answer`                                | bool   | Return short extractive answer above results                              |
| `include_facets`                                | bool   | Include facet counts (for filters)                                        |
| `orderby`                                       | string | Optional sort field (e.g., `last_modified desc`)                          |
| `skip/top`                                      | int    | Pagination control                                                        |

### Response Example

```json
{
  "count": 124,
  "results": [
    {
      "project_name": "11 Mile Solar",
      "segment": "Solar",
      "owner": "Ørsted",
      "state": "AZ",
      "snippet": "Geotechnical Engineering Report ...",
      "caption": "Pile foundation stiffness was determined using L-Pile analyses ...",
      "link": "https://quantaservices.sharepoint.com/.../GEO.rpt.pdf#page=12"
    }
  ],
  "facets": { "owner": [{"value": "Ørsted", "count": 42}] },
  "answer": "Pile groups were modeled using L-Pile with field-calibrated parameters.",
  "next_skip": 10
}
```

---

## 🧩 Local Development

### 1. Install dependencies

```bash
python -m venv venv
venv\Scripts\activate      # Windows
# or
source venv/bin/activate   # macOS/Linux

pip install -r requirements.txt
```

### 2. Run the API

From the project root:

```bash
uvicorn app.main:app --reload
```

Then open the interactive docs:
👉 [http://127.0.0.1:8000/docs](http://127.0.0.1:8000/docs)

---

## 🧪 Testing locally

### Example: Regional search

```
GET /geotech/search?q=pile capacity&limit_region=true&lat=32.86&lon=-111.59&radius_km=50
```

### Example: Full DB search

```
GET /geotech/search?q=pile capacity&search_full_db=true
```

### Example: Typeahead

```
GET /geotech/suggest?term=solar
```

---

## ☁️ Deployment (Azure)

### Option 1 — Azure App Service

1. Deploy the entire `app/` folder.
2. Set the following in **Configuration → Application Settings**:

   * `AZURE_SEARCH_SERVICE_ENDPOINT`
   * `AZURE_SEARCH_API_KEY`
   * (and any others from `.env`)
3. Use Gunicorn entry point:

   ```
   gunicorn -w 2 -k uvicorn.workers.UvicornWorker app.main:app
   ```

### Option 2 — Azure Function (Python HTTP Trigger)

1. Package the API as a function with `app.main:app` as the entry.
2. Enable the `ASGI` middleware in the function template.
3. Use the same `.env` values as Function configuration settings.

---

## 🔧 Tech Stack

| Component                 | Purpose                              |
| ------------------------- | ------------------------------------ |
| **FastAPI**               | REST API framework                   |
| **Azure AI Search**       | Semantic + vector retrieval          |
| **Azure OpenAI**          | Query embeddings (hybrid search)     |
| **PyMuPDF** (ingest side) | Document parsing (outside this repo) |
| **Uvicorn**               | ASGI web server                      |
| **python-dotenv**         | Environment loading                  |

---

## 🧭 Quick Start Summary

```bash
git clone <repo-url>
cd geotech-api
cp .env.example .env        # fill in Azure keys
pip install -r requirements.txt
uvicorn app.main:app --reload
```

Then visit:
➡️ **Swagger Docs:** [http://localhost:8000/docs](http://localhost:8000/docs)
➡️ **Health Check:** [http://localhost:8000/healthz](http://localhost:8000/healthz)
➡️ **Geotech Search:** [http://localhost:8000/geotech/search?q=pile%20capacity](http://localhost:8000/geotech/search?q=pile%20capacity)
