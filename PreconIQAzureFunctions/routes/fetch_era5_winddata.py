import logging
import cdsapi
import azure.functions as func
import xarray as xr
import pandas as pd
from app_init import app

@app.route(route="fetch_era5_winddata")
def fetch_era5_winddata(req: func.HttpRequest) -> func.HttpResponse:
    logging.info("🌬️ /fetch_era5_winddata triggered")

    try:
        # === Required query params ===
        lat_param = req.params.get("lat")
        lon_param = req.params.get("lon")
        year_param = req.params.get("year")
        months_param = req.params.get("months")  # e.g., "1-3" or "1,2,3"

        if year_param is None:
            return func.HttpResponse("Provide 'year' query parameter.", status_code=400)

        # Parse and validate
        try:
            latitude = float(lat_param)
            longitude = float(lon_param)
            year = int(year_param)
        except ValueError:
            return func.HttpResponse("Invalid 'lat', 'lon', or 'year' value.", status_code=400)

        # Parse months
        if months_param:
            # Handle ranges like "1-3" or lists like "1,2,3"
            if "-" in months_param:
                start, end = months_param.split("-")
                months = list(range(int(start), int(end) + 1))
            else:
                months = [int(m) for m in months_param.split(",")]
        else:
            months = list(range(1, 13))  # Default: all months

        # Validate months
        if any(m < 1 or m > 12 for m in months):
            return func.HttpResponse("'months' must be in range 1..12.", status_code=400)

        # === CDS client (kept as-is per your request) ===
        c = cdsapi.Client(
            url="__CDSAPIURL__",
            key="__CDSAPIKEY__"
        )
        logging.info("✅ CDS API client initialized successfully.")

        all_months_data = []

        for month in months:
            file_path = f'/tmp/test_{year}_{month}.nc'
            logging.info(f"⬇️ Retrieving ERA5 for {year}-{month}")

            # Request for each month
            c.retrieve(
                'reanalysis-era5-single-levels',
                {
                    'product_type': 'reanalysis',
                    'variable': ['10m_u_component_of_wind', '10m_v_component_of_wind'],
                    'year': str(year),
                    'month': str(month),
                    'day': [f"{d:02d}" for d in range(1, 32)],
                    'time': [f"{h:02d}:00" for h in range(24)],
                    'format': 'netcdf',
                    'area': [latitude + 0.01, longitude - 0.01, latitude - 0.01, longitude + 0.01],
                },
                file_path
            )
            logging.info(f"✅ File saved at {file_path}")

            # Open NetCDF and extract
            ds = xr.open_dataset(file_path)
            try:
                time_name = 'valid_time' if 'valid_time' in ds.variables else 'time'
                times = ds[time_name].values
                lat = float(ds['latitude'].values[0])
                lon = float(ds['longitude'].values[0])
                u10 = ds['u10'].values[:, 0, 0]
                v10 = ds['v10'].values[:, 0, 0]
                number = ds['number'].values if 'number' in ds else [0] * len(times)
                expver = ds['expver'].values if 'expver' in ds else [0] * len(times)

                df = pd.DataFrame({
                    'valid_time': [str(t) for t in times],
                    'latitude': lat,
                    'longitude': lon,
                    'u10': u10,
                    'v10': v10,
                    'number': number,
                    'expver': expver,
                    'year': year,
                    'month': month,
                })
                all_months_data.append(df)
            finally:
                ds.close()

        # Combine all months
        final_df = pd.concat(all_months_data, ignore_index=True)
        json_data = final_df.to_json(orient='records')
        return func.HttpResponse(json_data, mimetype='application/json', status_code=200)

    except Exception as e:
        logging.exception("❌ Error in fetch_era5_winddata")
        return func.HttpResponse(f"❌ {e}", status_code=500)