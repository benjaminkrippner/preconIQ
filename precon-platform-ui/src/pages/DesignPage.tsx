import {
  useEffect,
  useMemo,
  useRef,
  useState,
} from "react";
import "../styles/DesignPage.css";
import BasisOfDesignWidget from "../components/dashboard/widgets/BasisOfDesign/BasisOfDesignWidget";

const MAP_CENTER: [number, number] = [35.1983, -111.6513];
const SQ_METERS_PER_SQ_MILE = 2_589_988.110336;

const LEAFLET_SCRIPT_URL = "https://unpkg.com/leaflet@1.9.4/dist/leaflet.js";
const LEAFLET_DRAW_SCRIPT_URL =
  "https://unpkg.com/leaflet-draw@1.0.4/dist/leaflet.draw.js";

const SCRIPT_ATTRIBUTES: Record<string, Partial<HTMLScriptElement>> = {
  [LEAFLET_SCRIPT_URL]: { crossOrigin: "anonymous" },
  [LEAFLET_DRAW_SCRIPT_URL]: { crossOrigin: "anonymous" },
};

const loadedScripts = new Map<string, Promise<void>>();

function loadScriptOnce(src: string) {
  const existingPromise = loadedScripts.get(src);
  if (existingPromise) return existingPromise;

  const promise = new Promise<void>((resolve, reject) => {
    const existingScript = Array.from(
      document.getElementsByTagName("script")
    ).find((s) => s.src === src);

    if (existingScript) {
      if (existingScript.getAttribute("data-loaded") === "true") {
        resolve();
        return;
      }
      existingScript.addEventListener("load", () => resolve(), { once: true });
      existingScript.addEventListener(
        "error",
        () => reject(new Error(`Failed to load script: ${src}`)),
        { once: true }
      );
      return;
    }

    const script = document.createElement("script");
    script.src = src;
    script.defer = true;
    const attrs = SCRIPT_ATTRIBUTES[src];
    if (attrs?.crossOrigin) script.crossOrigin = attrs.crossOrigin;
    script.addEventListener("load", () => {
      script.setAttribute("data-loaded", "true");
      resolve();
    });
    script.addEventListener("error", () => {
      loadedScripts.delete(src);
      reject(new Error(`Failed to load script: ${src}`));
    });
    document.body.appendChild(script);
  });

  loadedScripts.set(src, promise);
  return promise;
}

async function ensureLeaflet() {
  const { L: existingL } = window as { L?: any };
  if (existingL?.Draw) return existingL;

  await loadScriptOnce(LEAFLET_SCRIPT_URL);
  const { L } = window as { L?: any };
  if (!L) throw new Error("Leaflet failed to load");

  if (!L.Draw) await loadScriptOnce(LEAFLET_DRAW_SCRIPT_URL);
  if (!L.Draw) throw new Error("Leaflet Draw failed to load");

  return L;
}

function radiusToMeters(miles: number) {
  return miles * 1609.34;
}
function formatAreaLabel(areaSqMiles: number | null) {
  if (areaSqMiles == null || Number.isNaN(areaSqMiles)) return "—";
  const formatted = areaSqMiles.toLocaleString(undefined, {
    minimumFractionDigits: areaSqMiles < 10 ? 2 : 1,
    maximumFractionDigits: areaSqMiles < 10 ? 2 : 1,
  });
  return `${formatted} sq mi`;
}
function computePolygonAreaSqMiles(latlngs: Array<{ lat: number; lng: number }>) {
  if (latlngs.length < 3) return 0;
  const EARTH_RADIUS = 6378137;
  let area = 0;
  const points = latlngs.map(({ lat, lng }) => {
    const rad = Math.PI / 180;
    const x = lng * rad * EARTH_RADIUS;
    const y = Math.log(Math.tan(Math.PI / 4 + (lat * rad) / 2)) * EARTH_RADIUS;
    return { x, y };
  });
  for (let i = 0; i < points.length; i++) {
    const cur = points[i];
    const next = points[(i + 1) % points.length];
    area += cur.x * next.y - next.x * cur.y;
  }
  return Math.abs(area) / 2 / SQ_METERS_PER_SQ_MILE;
}

const REPORTS = [
  { id: 1, category: "Borehole Log", project: "Mountain View Solar 750MW", modified: "12/03/2024", boreholes: 18, link: "#" },
  { id: 2, category: "Lab Results", project: "Mountain View Solar 750MW", modified: "11/24/2024", boreholes: 12, link: "#" },
  { id: 3, category: "Site Geophysics", project: "Mountain View Solar 750MW", modified: "10/17/2024", boreholes: 9, link: "#" },
  { id: 4, category: "Environmental", project: "Mountain View Solar 750MW", modified: "09/28/2024", boreholes: 6, link: "#" },
];

export default function DesignPage() {
  const [activeTab, setActiveTab] = useState<"GeoTech" | "BasisOfDesign">("GeoTech");
  const [mode, setMode] = useState<"radius" | "lasso">("radius");
  const [radius, setRadius] = useState(35);
  const [lassoAreaSqMiles, setLassoAreaSqMiles] = useState<number | null>(null);
  const [isMapReady, setIsMapReady] = useState(false);

  const mapContainerRef = useRef<HTMLDivElement | null>(null);
  const mapRef = useRef<any>(null);
  const circleRef = useRef<any>(null);
  const drawnItemsRef = useRef<any>(null);
  const initialRadiusRef = useRef(radius);

  const shapeArea = useMemo(() => {
    const milesArea = mode === "radius" ? Math.PI * Math.pow(radius, 2) : lassoAreaSqMiles ?? 0;
    return formatAreaLabel(milesArea);
  }, [mode, radius, lassoAreaSqMiles]);


  /** Initialize map once */
  useEffect(() => {
    if (!mapContainerRef.current || mapRef.current) return;
    let cancel = false;

    (async () => {
      try {
        const L = await ensureLeaflet();
        if (cancel || !mapContainerRef.current) return;
        const map = L.map(mapContainerRef.current, { center: MAP_CENTER, zoom: 10, zoomControl: false });
        L.control.zoom({ position: "topright" }).addTo(map);
        L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
          attribution: "&copy; OpenStreetMap contributors", maxZoom: 18,
        }).addTo(map);
        const circle = L.circle(MAP_CENTER, {
          radius: radiusToMeters(initialRadiusRef.current),
          color: "#02568A", fillColor: "#0B6DA8", fillOpacity: 0.18, weight: 2.5,
        }).addTo(map);
        const centerMarker = L.circleMarker(MAP_CENTER, {
          radius: 6, color: "#fff", weight: 2, fillColor: "#d7263d", fillOpacity: 1,
        }).addTo(map);
        centerMarker.bindTooltip("Active coordinate", { permanent: true, direction: "top" });

        const drawnItems = new L.FeatureGroup().addTo(map);
        const drawControl = new L.Control.Draw({
          draw: {
            polygon: {
              allowIntersection: false,
              drawError: { color: "#d7263d", message: "Self-intersections not allowed" },
              shapeOptions: { color: "#02568A", fillColor: "#0B6DA8", fillOpacity: 0.25, weight: 2.5 },
            },
            circle: false, rectangle: false, marker: false, polyline: false, circlemarker: false,
          },
          edit: { featureGroup: drawnItems, edit: true, remove: true },
        });
        map.addControl(drawControl);

        const syncPolygonArea = (layer: any) => {
          const latlngs = layer.getLatLngs?.()[0] ?? [];
          if (latlngs.length >= 3) {
            setLassoAreaSqMiles(computePolygonAreaSqMiles(latlngs));
          } else {
            setLassoAreaSqMiles(null);
          }
        };
        map.on(L.Draw.Event.CREATED, (e: any) => {
          drawnItems.clearLayers();
          drawnItems.addLayer(e.layer);
          syncPolygonArea(e.layer);
        });
        map.on(L.Draw.Event.EDITED, (e: any) =>
          e.layers.eachLayer((layer: any) => syncPolygonArea(layer))
        );
        map.on(L.Draw.Event.DELETED, () => {
          setLassoAreaSqMiles(null);
          // setHasLassoShape(false);
        });

        mapRef.current = map;
        circleRef.current = circle;
        drawnItemsRef.current = drawnItems;
        setIsMapReady(true);
      } catch (e) {
        console.error("Map init failed", e);
      }
    })();

    return () => {
      cancel = true;
      mapRef.current?.remove?.();
      mapRef.current = null;
    };
  }, []);

  /** Update radius */
  useEffect(() => {
    circleRef.current?.setRadius(radiusToMeters(radius));
  }, [radius]);

  /** Toggle circle visibility by mode */
  useEffect(() => {
    const map = mapRef.current, circle = circleRef.current;
    if (!map || !circle) return;
    if (mode === "radius") { if (!map.hasLayer(circle)) circle.addTo(map); }
    else if (map.hasLayer(circle)) map.removeLayer(circle);
  }, [mode]);

  /** 🔄 Repaint when returning to GeoTech tab */
  useEffect(() => {
    if (activeTab === "GeoTech" && mapRef.current) {
      setTimeout(() => mapRef.current.invalidateSize(), 0);
    }
  }, [activeTab]);

  return (
    <div className="design-page">
      {/* Tab header */}
      <section className="dashboard-card design-hero">
        <div>
          <h1>Engineering Design Optimization</h1>
          <p className="design-hero__subtitle">
            Regional geotechnical report analysis for Mountain View Solar — 200MW
          </p>
        </div>
      </section>

      <div className="dp-tablist" role="tablist">
        <button
          type="button"
          role="tab"
          className={`dp-tab ${activeTab === "GeoTech" ? "dp-tab--active" : ""}`}
          aria-selected={activeTab === "GeoTech"}
          onClick={() => setActiveTab("GeoTech")}
        >
          GeoTech
        </button>
        <button
          type="button"
          role="tab"
          className={`dp-tab ${activeTab === "BasisOfDesign" ? "dp-tab--active" : ""}`}
          aria-selected={activeTab === "BasisOfDesign"}
          onClick={() => setActiveTab("BasisOfDesign")}
        >
          Basis of Design
        </button>
      </div>

      {/* 👇 Keep both mounted; toggle with hidden */}
      <div role="tabpanel" hidden={activeTab !== "GeoTech"}>
        {/* GeoTech content */}
        <div className="design-sections">
          {/* Map and controls (unchanged) */}
          <section className="dashboard-card design-map-card design-map-card--full">
            <header className="card-header design-map-card__header">
              <div>
                <div className="card-title">Regional Geotech Reports</div>

                <div className="card-subtitle">
                  Search radius in miles determines how far we collect available boring logs
                  and lab results.

                  <div className="design-hero__meta">
                    <div>
                      <span className="meta-label">Active Selection</span>
                      <strong>
                        {mode === "radius" ? "Radius Search" : "Custom Lasso Region"}
                      </strong>
                    </div>

                    <div>
                      <span className="meta-label">Area Coverage</span>
                      <strong>{shapeArea}</strong>
                    </div>

                    <div>
                      <span className="meta-label">Reports Discovered</span>
                      <strong>{REPORTS.length}</strong>
                    </div>
                  </div>
                </div>
              </div>

              <div className="design-mode-toggle" role="tablist">
                <button
                  type="button"
                  role="tab"
                  aria-selected={mode === "radius"}
                  className={mode === "radius" ? "active" : ""}
                  onClick={() => setMode("radius")}
                >
                  Add Circle
                </button>

                <button
                  type="button"
                  role="tab"
                  aria-selected={mode === "lasso"}
                  className={mode === "lasso" ? "active" : ""}
                  onClick={() => setMode("lasso")}
                >
                  Draw Lasso
                </button>
              </div>
            </header>
            <div className="design-controls">
              {mode === "radius" ? (
                <label className="radius-control">
                  <span>Search Radius: {radius} miles</span>
                  <input
                    type="range"
                    min={10}
                    max={75}
                    step={1}
                    value={radius}
                    onChange={(e) => setRadius(Number(e.target.value))}
                    aria-label="Search radius in miles"
                  />
                </label>
              ) : (
                <div className="lasso-instructions">
                  <div className="lasso-instructions__title">
                    Click on the map to place vertices and double-click to close the polygon.
                  </div>
                </div>
              )}
            </div>

            <div className="design-map" data-mode={mode}>
              <div ref={mapContainerRef} className="design-map__viewport" />
              {!isMapReady && (
                <div className="design-map__loading" aria-live="polite">
                  Loading interactive map…
                </div>
              )}
            </div>

            <footer className="design-map__footer">
              <div><span className="legend-circle" /> Circle coverage</div>
              <div><span className="legend-lasso" /> Lasso polygon</div>
              <div><span className="legend-pin" /> Active coordinate</div>
            </footer>
          </section>

          {/* Reports table */}
          <section className="dashboard-card design-reports design-reports--full">
            <header className="card-header">
              <div>
                <div className="card-title">Filtered Reports</div>
                <div className="card-subtitle">
                  {REPORTS.length} geotechnical files within selected boundary.
                </div>
              </div>
            </header>
            <table className="table design-table">
              <thead>
                <tr>
                  <th>Category</th>
                  <th>Project Name</th>
                  <th>Modified</th>
                  <th>Boreholes</th>
                  <th>Action</th>
                </tr>
              </thead>
              <tbody>
                {REPORTS.map((r) => (
                  <tr key={r.id}>
                    <td>{r.category}</td>
                    <td>{r.project}</td>
                    <td>{r.modified}</td>
                    <td>{r.boreholes}</td>
                    <td><a href={r.link}>Open</a></td>
                  </tr>
                ))}
              </tbody>
            </table>
          </section>
        </div>
      </div>

      <div role="tabpanel" hidden={activeTab !== "BasisOfDesign"}>
        <BasisOfDesignWidget />
      </div>
    </div>
  );
}
