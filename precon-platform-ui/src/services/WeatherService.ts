// src/services/WeatherService.ts
import ProjectNormalsService from "./ProjectNormalsService";
import type ProjectNormals from "../types/ProjectNormals";

/** Back-compat alias so older code that imports `NormalsMonth` still compiles. */
export type NormalsMonth = ProjectNormals;

/** Daily series point (kept for API compatibility even though normals don't provide daily rows). */
export type DailyWeatherPoint = {
  date: string; // YYYY-MM-DD
  windMaxMph: number; // daily max wind (mph)
  precipIn: number; // daily precip accumulation (in)
};

/** Summary used by the widget; fed by the normals API. */
export type WeatherSummary = {
  days: DailyWeatherPoint[]; // empty for normals fallback
  highWindDays: number;
  avgWindMaxMph: number;
  totalPrecipIn: number;
  temps?: {
    avgHighTemp?: number;
    avgLowTemp?: number;
    recordAvgHighTemp?: number;
    recordAvgLowTemp?: number;
  };
  precipNormals?: {
    avgPrecip?: number;
    recordHighAvgPrecip?: number;
    recordLowAvgPrecip?: number;
    avgSnow?: number;
    recordHighAvgSnow?: number;
    recordLowAvgSnow?: number;
  };
  windNormals?: {
    avgWind?: number;
    recordAvgHighWind?: number;
    recordAvgLowWind?: number;
  };
};

export type WeatherOptions = {
  highWindThresholdMph?: number; // threshold for "high wind days" (default 25)
  projectId?: string; // REQUIRED for normals lookup
};

const DEFAULT_HIGH_WIND_THRESHOLD = 25;

const num = (v: any): number | undefined => {
  if (v === null || v === undefined) return undefined;
  const n = Number(String(v).trim());
  return Number.isFinite(n) ? n : undefined;
};
const numFromKeys = (
  o: Record<string, any>,
  keys: string[],
): number | undefined => {
  for (const key of keys) {
    const val = num(o[key]);
    if (val !== undefined) return val;
  }
  return undefined;
};

/** Fetch normals-backed weather summary for the given project. */
export async function fetchWeatherSummary(opts: WeatherOptions = {}): Promise<WeatherSummary> {
  return fetchNormalsFromApi({ ...opts });
}

// -------------------- Normals (ProjectNormalsService) -----------------

async function fetchNormalsFromApi(opts: WeatherOptions): Promise<WeatherSummary> {
  const monthIndex = new Date().getMonth() + 1; // 1..12
  const projectId = opts.projectId?.toString().trim();

  if (!projectId) {
    return { days: [], highWindDays: 0, avgWindMaxMph: 0, totalPrecipIn: 0 };
  }

  const normals = await fetchProjectNormals(projectId);
  const row = normals.find((n) => n.month === monthIndex);

  if (!row) {
    return { days: [], highWindDays: 0, avgWindMaxMph: 0, totalPrecipIn: 0 };
  }

  const temps = {
    avgHighTemp: numFromKeys(row, ["avgHighTemp", "AvgHighTemp"]),
    avgLowTemp: numFromKeys(row, ["avgLowTemp", "AvgLowTemp"]),
    recordAvgHighTemp: numFromKeys(row, ["recordAvgHighTemp", "RecordAvgHighTemp"]),
    recordAvgLowTemp: numFromKeys(row, ["recordAvgLowTemp", "RecordAvgLowTemp"]),
  };

  const precipNormals = {
    avgPrecip: numFromKeys(row, ["avgRain", "AvgRain"]),
    recordHighAvgPrecip: numFromKeys(row, ["recordHighAvgRain", "RecordHighAvgRain"]),
    recordLowAvgPrecip: numFromKeys(row, ["recordLowAvgRain", "RecordLowAvgRain"]),
    avgSnow: numFromKeys(row, ["avgSnow", "AvgSnow"]),
    recordHighAvgSnow: numFromKeys(row, ["recordHighAvgSnow", "RecordHighAvgSnow"]),
    recordLowAvgSnow: numFromKeys(row, ["recordLowAvgSnow", "RecordLowAvgSnow"]),
  };

  const windNormals = {
    avgWind: numFromKeys(row, ["avgHighWind", "avgLowWind", "AvgHighWind", "AvgLowWind"]),
    recordAvgHighWind: numFromKeys(row, ["recordAvgHighWind", "RecordAvgHighWind"]),
    recordAvgLowWind: numFromKeys(row, ["recordAvgLowWind", "RecordAvgLowWind"]),
  };

  const avgWindMaxMph = windNormals.avgWind ?? 0;
  const totalPrecipIn = precipNormals.avgPrecip ?? 0;

  const threshold = opts.highWindThresholdMph ?? DEFAULT_HIGH_WIND_THRESHOLD;
  const daysInMonth = new Date(new Date().getFullYear(), monthIndex, 0).getDate();
  const effectiveMax = windNormals.recordAvgHighWind ?? windNormals.avgWind ?? 0;
  const effectiveMin = windNormals.avgWind ?? 0;
  const range = Math.max(0.001, effectiveMax - effectiveMin);
  const fractionAbove =
    effectiveMax <= threshold ? 0 : Math.max(0, Math.min(1, (effectiveMax - threshold) / range)) / 2;
  const highWindDays = Math.round(daysInMonth * fractionAbove);

  return {
    days: [],
    highWindDays,
    avgWindMaxMph,
    totalPrecipIn,
    temps,
    precipNormals,
    windNormals,
  };
}

/** Returns all rows for a project as `ProjectNormals[]` (also aliased as `NormalsMonth[]`). */
export async function fetchNormalsSeries(projectId: string): Promise<ProjectNormals[]> {
  const trimmedId = projectId?.toString().trim();

  if (!trimmedId) return [];

  const numericId = Number(trimmedId);
  const rows = Number.isFinite(numericId)
    ? await ProjectNormalsService.getProjectNormalsById(numericId)
    : await ProjectNormalsService.getProjectNormalsByOppId(trimmedId);

  return rows
    .filter((r) => (r.month ?? 0) > 0)
    .sort((a, b) => (a.month ?? 0) - (b.month ?? 0));
}

async function fetchProjectNormals(projectId: string): Promise<ProjectNormals[]> {
  const numericId = Number(projectId);

  if (Number.isFinite(numericId)) {
    return ProjectNormalsService.getProjectNormalsById(numericId);
  }

  return ProjectNormalsService.getProjectNormalsByOppId(projectId);
}
