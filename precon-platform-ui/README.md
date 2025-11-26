# Preconstruction Weather Intelligence Dashboard

An interactive dashboard for comparing long-term weather risk between renewable energy projects. The app visualizes 20-year climatology against the most recent 3-year trends so estimators and field planners can understand how temperature, wind, and precipitation patterns shift between sites.

## Highlights

- **Dual project coverage** – Rock Creek Wind (Laramie, WY) and Chillingham Solar (Bell County, TX) use independent climate baselines tailored to wind and solar construction challenges.
- **3-year vs. 20-year climate splits** – Every monthly normal tracks both the recent 3-year trend and the historical 20-year average for temperatures, wind speeds, rainfall, and wet days.
- **Workability insights** – Summaries translate the climate data into workable day estimates, extreme temperature counts, and severe weather indicators.
- **Rich visualization** – Chart.js power users will find temperature, wind, precipitation, and workable-day charts that adapt to the selected project profile.

## Project climate profiles

| Project | Focus | 20-year tendencies | Recent 3-year signal |
| --- | --- | --- | --- |
| **Rock Creek Wind** | High-elevation wind farm outside Laramie, WY | Cold winters with frequent snow, strong baseline winds, steady spring moisture | Warmer and wetter shoulder seasons, higher peak gusts (31–35 mph), additional wet days through spring and early fall |
| **Chillingham Solar** | Utility-scale solar build in central Texas | Hot, humid growing season with consistent summer storms | Hotter and drier recent summers (panel-temperature stress), lighter winds during the hottest months, modest rainfall declines in winter |

The contrasting datasets ensure that temperature, wind, precipitation, and wet-day statistics produce distinct 3-year and 20-year metrics for each project throughout the dashboard.

## Running the dashboard locally

You can serve the static site with either Python or Node. Use whichever tooling you already have installed.

### Option 1: Python 3 (no dependencies)

```bash
cd PreconWeather
python -m http.server 8000
```

Then open `http://localhost:8000/index.html` in your browser.

### Option 2: Node live-server (requires `npm install` once)

```bash
cd PreconWeather
npm install
npm run start
```

The live server defaults to `http://127.0.0.1:8080`. Adjust the port with `npm run start -- --port=8000` if you want consistency with the Python example.

## Repository layout

```
PreconWeather/
├── index.html               # Dashboard entry point
├── dashboard.html           # Optional deep-link landing page
├── src/
│   ├── styles/              # Base, component, utility, and layout CSS
│   ├── scripts/
│   │   ├── main.js          # Application bootstrapper
│   │   ├── config/projects.js   # Project metadata & climate normals
│   │   ├── data/windProfiles.js # Wind exceedance lookup tables
│   │   ├── services/            # Weather + project data services
│   │   └── components/          # Charts, tables, and tab UIs
│   └── assets/
│       └── images/          # Logos and illustration assets
├── docs/datasets/           # CSV data ingested by ProjectService
└── tests/                   # Node test stubs for service logic
```

Key dataset files inside `docs/datasets/`:

- `project_normals.csv` – Month-by-month climate normals with separate columns for recent 3-year (`*_recent`) values to drive the 3-year/20-year comparisons.
- `projects.csv` – Project metadata (name, coordinates, risk rating, etc.).
- `solar_schedule_*.csv` – Construction schedule scaffolding for solar timelines.