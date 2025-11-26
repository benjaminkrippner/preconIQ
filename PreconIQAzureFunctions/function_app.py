# Azure Functions runtime looks for `app` in this file.
from app_init import app  # re-export for the runtime

# Import routes (they will register with the shared `app`)
import routes.fetch_era5_winddata