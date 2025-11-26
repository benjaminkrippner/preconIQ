import React from "react";
import { useProjects } from "../../../state/ProjectContext";
import useProjectNormals from "../../../state/ProjectNormalsContext";

const fmt = (n?: number, d = 1, suffix = "") =>
  n == null ? "—" : `${n.toFixed(d)}${suffix}`;

export default function WeatherWidget() {
  const { selectedId } = useProjects();
  const { normals, loading, error } = useProjectNormals(selectedId ?? null);

  const currentMonth = React.useMemo(() => {
    if (!normals?.length) return null;
    const month = new Date().getMonth() + 1; // 1-indexed
    return normals.find((n) => n.month === month) ?? null;
  }, [normals]);

  const temps = React.useMemo(
    () =>
      currentMonth
        ? {
            avgHighTemp: currentMonth.avgHighTemp,
            avgLowTemp: currentMonth.avgLowTemp,
          }
        : null,
    [currentMonth],
  );

  const wind = React.useMemo(
    () =>
      currentMonth
        ? {
            avgWind: currentMonth.avgHighWind ?? currentMonth.avgLowWind,
          }
        : null,
    [currentMonth],
  );

  const val = (display: string) => (loading ? "…" : error ? "—" : display);

  return (
    <div className="module-body--compact">
      <div className="module-row">
        <span className="module-label">Avg High Temp</span>
        <span className="module-value">{val(fmt(temps?.avgHighTemp, 1, " °F"))}</span>
      </div>

      <div className="module-row">
        <span className="module-label">Avg Low Temp</span>
        <span className="module-value">{val(fmt(temps?.avgLowTemp, 1, " °F"))}</span>
      </div>

      <div className="module-row">
        <span className="module-label">Avg Wind</span>
        <span className="module-value">{val(fmt(wind?.avgWind, 1, " mph"))}</span>
      </div>
    </div>
  );
}