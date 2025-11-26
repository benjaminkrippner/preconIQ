import React from "react";
import "../styles/WeatherAnalyticsPage.css";
import { useProjects } from "../state/ProjectContext";
import useProjectMetaData from "../state/ProjectMetaDataContext";
import useProjectNormals from "../state/ProjectNormalsContext";
import { fetchWeatherSummary, type DailyWeatherPoint } from "../services/WeatherService";
import { computeHeatmap, extractMetSamples } from "../functions/metHeatmap";
import type { HeatmapResponse, MetSample } from "../types/heatmap";

type ChartKey = "temperature" | "rain" | "wind" | "snow";
type SeriesKey =
  | "twentyYearAvgHigh"
  | "twentyYearAvgLow"
  | "recordHigh"
  | "recordLow"
  | "twentyYearAvgRain"
  | "recordHighRain"
  | "recordLowRain"
  | "twentyYearAvgSnow"
  | "recordHighSnow"
  | "recordLowSnow";

type ClimateDataPoint = {
  month: string;
} & Partial<Record<SeriesKey, number>>;

type SeriesRole =
  | "high"
  | "low"
  | "trendHigh"
  | "trendLow"
  | "recordHigh"
  | "recordLow";

type ChartSeriesDefinition = {
  key: SeriesKey;
  label: string;
  color: string;
  description: string;
  role: SeriesRole;
  defaultVisible: boolean;
};

type ChartFootnote = {
  label: string;
  value: string;
  swatch?: "rain" | "snow";
};

type ChartMeta = {
  footnotes?: ChartFootnote[];
};

type WeatherSummary = {
  days: DailyWeatherPoint[];
  highWindDays: number;
  avgWindMaxMph: number;
  totalPrecipIn: number;
};

const SERIES_CONFIG: Record<SeriesKey, ChartSeriesDefinition> = {
  twentyYearAvgHigh: {
    key: "twentyYearAvgHigh",
    label: "Avg High",
    color: "#0EA5E9",
    description: "High temperature climatology over the last two decades",
    role: "trendHigh",
    defaultVisible: true,
  },
  twentyYearAvgLow: {
    key: "twentyYearAvgLow",
    label: "Avg Low",
    color: "#38BDF8",
    description: "Low temperature climatology over the last two decades",
    role: "trendLow",
    defaultVisible: true,
  },
  recordHigh: {
    key: "recordHigh",
    label: "20-Year Record High",
    color: "#DC2626",
    description: "Hottest observed value for the month in the last twenty years",
    role: "recordHigh",
    defaultVisible: true,
  },
  recordLow: {
    key: "recordLow",
    label: "20-Year Record Low",
    color: "#7C3AED",
    description: "Coldest observed value for the month in the last twenty years",
    role: "recordLow",
    defaultVisible: true,
  },
  twentyYearAvgRain: {
    key: "twentyYearAvgRain",
    label: "Avg",
    color: "#0284C7",
    description: "Average monthly rainfall over the last two decades",
    role: "trendHigh",
    defaultVisible: true,
  },
  recordHighRain: {
    key: "recordHighRain",
    label: "20-Year Record High",
    color: "#DC2626",
    description: "Wettest observed rainfall month in the last twenty years",
    role: "recordHigh",
    defaultVisible: true,
  },
  recordLowRain: {
    key: "recordLowRain",
    label: "20-Year Record Low",
    color: "#7C3AED",
    description: "Driest observed rainfall month in the last twenty years",
    role: "recordLow",
    defaultVisible: true,
  },
  twentyYearAvgSnow: {
    key: "twentyYearAvgSnow",
    label: "Avg",
    color: "#6366F1",
    description: "Average monthly snowfall over the last two decades",
    role: "trendHigh",
    defaultVisible: true,
  },
  recordHighSnow: {
    key: "recordHighSnow",
    label: "20-Year Record High",
    color: "#D946EF",
    description: "Snowiest observed month in the last twenty years",
    role: "recordHigh",
    defaultVisible: true,
  },
  recordLowSnow: {
    key: "recordLowSnow",
    label: "20-Year Record Low",
    color: "#581C87",
    description: "Least snowy observed month in the last twenty years",
    role: "recordLow",
    defaultVisible: true,
  },
};

const RAINFALL_SHARE = 0.78;
const SNOWFALL_SHARE = 0.22;

const PRECIP_CLIMATE_DATA: ClimateDataPoint[] = [
  {
    month: "Jan",
    twentyYearAvgRain: 2.8,
    recordHighRain: 6.1,
    recordLowRain: 1.2,
    twentyYearAvgSnow: 0.95,
    recordHighSnow: 2.6,
    recordLowSnow: 0.1,
  },
  {
    month: "Feb",
    twentyYearAvgRain: 2.6,
    recordHighRain: 5.6,
    recordLowRain: 1.1,
    twentyYearAvgSnow: 0.8,
    recordHighSnow: 2.3,
    recordLowSnow: 0.1,
  },
  {
    month: "Mar",
    twentyYearAvgRain: 3.4,
    recordHighRain: 7.1,
    recordLowRain: 1.6,
    twentyYearAvgSnow: 0.6,
    recordHighSnow: 1.7,
    recordLowSnow: 0,
  },
  {
    month: "Apr",
    twentyYearAvgRain: 3.2,
    recordHighRain: 6.3,
    recordLowRain: 1.8,
    twentyYearAvgSnow: 0.3,
    recordHighSnow: 1.2,
    recordLowSnow: 0,
  },
  {
    month: "May",
    twentyYearAvgRain: 3.8,
    recordHighRain: 7,
    recordLowRain: 2.1,
    twentyYearAvgSnow: 0.1,
    recordHighSnow: 0.8,
    recordLowSnow: 0,
  },
  {
    month: "Jun",
    twentyYearAvgRain: 4.15,
    recordHighRain: 7.4,
    recordLowRain: 2.4,
    twentyYearAvgSnow: 0.05,
    recordHighSnow: 0.4,
    recordLowSnow: 0,
  },
  {
    month: "Jul",
    twentyYearAvgRain: 4.35,
    recordHighRain: 7.8,
    recordLowRain: 2.8,
    twentyYearAvgSnow: 0,
    recordHighSnow: 0.2,
    recordLowSnow: 0,
  },
  {
    month: "Aug",
    twentyYearAvgRain: 4.45,
    recordHighRain: 8,
    recordLowRain: 2.9,
    twentyYearAvgSnow: 0,
    recordHighSnow: 0.1,
    recordLowSnow: 0,
  },
  {
    month: "Sep",
    twentyYearAvgRain: 3.8,
    recordHighRain: 7.2,
    recordLowRain: 2.3,
    twentyYearAvgSnow: 0.1,
    recordHighSnow: 0.6,
    recordLowSnow: 0,
  },
  {
    month: "Oct",
    twentyYearAvgRain: 3.25,
    recordHighRain: 6.4,
    recordLowRain: 1.9,
    twentyYearAvgSnow: 0.4,
    recordHighSnow: 1.5,
    recordLowSnow: 0,
  },
  {
    month: "Nov",
    twentyYearAvgRain: 2.95,
    recordHighRain: 5.9,
    recordLowRain: 1.6,
    twentyYearAvgSnow: 0.6,
    recordHighSnow: 2,
    recordLowSnow: 0.1,
  },
  {
    month: "Dec",
    twentyYearAvgRain: 2.8,
    recordHighRain: 6,
    recordLowRain: 1.3,
    twentyYearAvgSnow: 0.85,
    recordHighSnow: 2.4,
    recordLowSnow: 0.1,
  },
];

const CLIMATE_SERIES: Record<
  ChartKey,
  {
    title: string;
    subtitle: string;
    unit: string;
    seriesKeys: SeriesKey[];
    data: ClimateDataPoint[];
    insightHighKey?: SeriesKey;
    insightLowKey?: SeriesKey;
    meta?: ChartMeta;
  }
> = {
  temperature: {
    title: "Temperature Outlook",
    subtitle: "Benchmark highs, lows, and long-term records for each month",
    unit: "°F",
    seriesKeys: [
      "twentyYearAvgHigh",
      "twentyYearAvgLow",
      "recordHigh",
      "recordLow",
    ],
    insightHighKey: "recordHigh",
    insightLowKey: "recordLow",
    data: [
      {
        month: "Jan",
        twentyYearAvgHigh: 40,
        twentyYearAvgLow: 26,
        recordHigh: 58,
        recordLow: 8,
      },
      {
        month: "Feb",
        twentyYearAvgHigh: 43,
        twentyYearAvgLow: 28,
        recordHigh: 62,
        recordLow: 12,
      },
      {
        month: "Mar",
        twentyYearAvgHigh: 51,
        twentyYearAvgLow: 34,
        recordHigh: 71,
        recordLow: 18,
      },
      {
        month: "Apr",
        twentyYearAvgHigh: 61,
        twentyYearAvgLow: 42,
        recordHigh: 82,
        recordLow: 29,
      },
      {
        month: "May",
        twentyYearAvgHigh: 70,
        twentyYearAvgLow: 53,
        recordHigh: 90,
        recordLow: 39,
      },
      {
        month: "Jun",
        twentyYearAvgHigh: 81,
        twentyYearAvgLow: 62,
        recordHigh: 97,
        recordLow: 50,
      },
      {
        month: "Jul",
        twentyYearAvgHigh: 85,
        twentyYearAvgLow: 67,
        recordHigh: 101,
        recordLow: 55,
      },
      {
        month: "Aug",
        twentyYearAvgHigh: 84,
        twentyYearAvgLow: 66,
        recordHigh: 100,
        recordLow: 54,
      },
      {
        month: "Sep",
        twentyYearAvgHigh: 77,
        twentyYearAvgLow: 59,
        recordHigh: 93,
        recordLow: 47,
      },
      {
        month: "Oct",
        twentyYearAvgHigh: 66,
        twentyYearAvgLow: 49,
        recordHigh: 84,
        recordLow: 34,
      },
      {
        month: "Nov",
        twentyYearAvgHigh: 55,
        twentyYearAvgLow: 39,
        recordHigh: 72,
        recordLow: 24,
      },
      {
        month: "Dec",
        twentyYearAvgHigh: 44,
        twentyYearAvgLow: 29,
        recordHigh: 59,
        recordLow: 10,
      },
    ],
  },
  rain: {
    title: "Rainfall Outlook",
    subtitle: "Benchmark rainfall intensity and long-term extremes by month",
    unit: "in",
    seriesKeys: [
      "twentyYearAvgRain",
      "recordHighRain",
      "recordLowRain",
    ],
    insightHighKey: "recordHighRain",
    insightLowKey: "recordLowRain",
    meta: {
      footnotes: [
        {
          swatch: "rain",
          label: "Rainfall contribution",
          value: `${Math.round(RAINFALL_SHARE * 100)}%`,
        },
        {
          swatch: "snow",
          label: "Snow & ice contribution",
          value: `${Math.round(SNOWFALL_SHARE * 100)}%`,
        },
      ],
    },
    data: PRECIP_CLIMATE_DATA,
  },
  wind: {
    title: "Wind Speed Outlook",
    subtitle: "Contrast routine gusts against historical extremes",
    unit: "mph",
    seriesKeys: [
      "twentyYearAvgHigh",
      "twentyYearAvgLow",
      "recordHigh",
      "recordLow",
    ],
    insightHighKey: "recordHigh",
    insightLowKey: "recordLow",
    data: [
      {
        month: "Jan",
        twentyYearAvgHigh: 24,
        twentyYearAvgLow: 9,
        recordHigh: 41,
        recordLow: 4,
      },
      {
        month: "Feb",
        twentyYearAvgHigh: 26,
        twentyYearAvgLow: 10,
        recordHigh: 43,
        recordLow: 5,
      },
      {
        month: "Mar",
        twentyYearAvgHigh: 29,
        twentyYearAvgLow: 12,
        recordHigh: 46,
        recordLow: 6,
      },
      {
        month: "Apr",
        twentyYearAvgHigh: 31,
        twentyYearAvgLow: 13,
        recordHigh: 48,
        recordLow: 7,
      },
      {
        month: "May",
        twentyYearAvgHigh: 27,
        twentyYearAvgLow: 11,
        recordHigh: 44,
        recordLow: 6,
      },
      {
        month: "Jun",
        twentyYearAvgHigh: 23,
        twentyYearAvgLow: 9,
        recordHigh: 38,
        recordLow: 4,
      },
      {
        month: "Jul",
        twentyYearAvgHigh: 22,
        twentyYearAvgLow: 8,
        recordHigh: 36,
        recordLow: 3,
      },
      {
        month: "Aug",
        twentyYearAvgHigh: 22,
        twentyYearAvgLow: 8,
        recordHigh: 37,
        recordLow: 3,
      },
      {
        month: "Sep",
        twentyYearAvgHigh: 23,
        twentyYearAvgLow: 9,
        recordHigh: 39,
        recordLow: 4,
      },
      {
        month: "Oct",
        twentyYearAvgHigh: 25,
        twentyYearAvgLow: 10,
        recordHigh: 41,
        recordLow: 5,
      },
      {
        month: "Nov",
        twentyYearAvgHigh: 27,
        twentyYearAvgLow: 11,
        recordHigh: 44,
        recordLow: 5,
      },
      {
        month: "Dec",
        twentyYearAvgHigh: 24,
        twentyYearAvgLow: 9,
        recordHigh: 41,
        recordLow: 4,
      },
    ],
  },
  snow: {
    title: "Snow & Ice Outlook",
    subtitle: "Snowfall intensity and cold-season accumulation benchmarks",
    unit: "in",
    seriesKeys: [
      "twentyYearAvgSnow",
      "recordHighSnow",
      "recordLowSnow",
    ],
    insightHighKey: "recordHighSnow",
    insightLowKey: "recordLowSnow",
    meta: {
      footnotes: [
        {
          swatch: "snow",
          label: "Snow & ice share of annual precipitation",
          value: `${Math.round(SNOWFALL_SHARE * 100)}%`,
        },
      ],
    },
    data: PRECIP_CLIMATE_DATA,
  },
};

type ClimateSeriesMap = typeof CLIMATE_SERIES;

const MONTH_LABELS = [
  "Jan",
  "Feb",
  "Mar",
  "Apr",
  "May",
  "Jun",
  "Jul",
  "Aug",
  "Sep",
  "Oct",
  "Nov",
  "Dec",
];

type HeatmapRow = {
  label: string;
  values: Array<number | null>;
};

type ShiftWindow = {
  monthIndex: number;
  startRow: number;
  average: number | null;
  length: number;
  coverage: number;
  validObservations: number;
};

type ShiftWindowStats = {
  average: number;
  validObservations: number;
};

type MonthSummary = {
  monthIndex: number;
  label: string;
  average: number | null;
  coverage: number;
};

const SHIFT_DURATION_MIN = 1;
const SHIFT_DURATION_MAX = 12;

const clamp = (value: number, min: number, max: number) => Math.min(Math.max(value, min), max);

const normalizeIndex = (index: number, total: number) => ((index % total) + total) % Math.max(total, 1);

const isIndexWithinWindow = (index: number, start: number, length: number, total: number) => {
  if (total <= 0 || length <= 0) {
    return false;
  }

  const normalizedTotal = Math.max(total, 1);
  const normalizedLength = Math.min(length, normalizedTotal);
  const normalizedStart = normalizeIndex(start, normalizedTotal);
  const normalizedIndex = normalizeIndex(index, normalizedTotal);
  if (normalizedLength >= normalizedTotal) {
    return true;
  }

  const end = (normalizedStart + normalizedLength) % normalizedTotal;
  if (normalizedStart + normalizedLength <= normalizedTotal) {
    return normalizedIndex >= normalizedStart && normalizedIndex < normalizedStart + normalizedLength;
  }

  return normalizedIndex >= normalizedStart || normalizedIndex < end;
};

const windowWraps = (start: number, length: number, total: number) => {
  if (total <= 0 || length <= 0) {
    return false;
  }

  const normalizedTotal = Math.max(total, 1);
  const normalizedStart = normalizeIndex(start, normalizedTotal);
  const normalizedLength = Math.min(length, normalizedTotal);
  return normalizedStart + normalizedLength > normalizedTotal;
};

const computeShiftAverage = (
  matrix: Array<Array<number | null | undefined>>,
  monthIndex: number,
  startRow: number,
  length: number,
): ShiftWindowStats | null => {
  if (!matrix.length || length <= 0) return null;
  const hours = matrix.length;
  const normalizedLength = Math.min(length, hours);
  const normalizedStart = ((startRow % hours) + hours) % hours;
  let sum = 0;
  let count = 0;
  for (let offset = 0; offset < normalizedLength; offset += 1) {
    const row = (normalizedStart + offset) % hours;
    const value = matrix[row]?.[monthIndex];
    if (typeof value !== "number") {
      continue;
    }
    sum += value;
    count += 1;
  }

  if (count === 0) {
    return null;
  }

  return {
    average: sum / count,
    validObservations: count,
  };
};

const formatPercent = (value: number | null | undefined, digits = 1) =>
  value == null
    ? "—"
    : `${value.toLocaleString(undefined, {
        minimumFractionDigits: digits,
        maximumFractionDigits: digits,
      })}%`;

const defaultSeriesState = (): Record<SeriesKey, boolean> =>
  Object.fromEntries(
    (Object.keys(SERIES_CONFIG) as SeriesKey[]).map((key) => [key, SERIES_CONFIG[key].defaultVisible])
  ) as Record<SeriesKey, boolean>;

const WIND_THRESHOLD_MIN = 1;
const WIND_THRESHOLD_MAX = 50;
const HUB_HEIGHT_MIN = 60;
const HUB_HEIGHT_MAX = 140;

const fmtNumber = (n: number | undefined, unit: string) =>
  n == null
    ? "—"
    : `${n.toLocaleString(undefined, {
        minimumFractionDigits: unit === "in" ? 1 : 0,
        maximumFractionDigits: unit === "in" ? 2 : 0,
      })} ${unit}`;

export default function WeatherAnalyticsPage() {
  const { selected, selectedId } = useProjects();
  const { meta } = useProjectMetaData(selectedId);
  const [activeTab, setActiveTab] = React.useState<"general" | "solar" | "wind">("general");
  const [analysisThreshold, setAnalysisThreshold] = React.useState(30);
  const [analysisThresholdInput, setAnalysisThresholdInput] = React.useState("30");
  const [hubHeight, setHubHeight] = React.useState(80);
  const [hubHeightInput, setHubHeightInput] = React.useState("80");
  const [shiftDuration, setShiftDuration] = React.useState(8);
  const [shiftDurationInput, setShiftDurationInput] = React.useState("8");
  const [metFiles, setMetFiles] = React.useState<File[]>([]);
  const [metError, setMetError] = React.useState<string | null>(null);
  const [metSamples, setMetSamples] = React.useState<MetSample[] | null>(null);
  const [heatmapLoading, setHeatmapLoading] = React.useState(false);
  const [heatmapValidation, setHeatmapValidation] = React.useState<string[]>([]);
  const [customShifts, setCustomShifts] = React.useState<Record<number, { startRow: number }>>({});
  const [activeCustomMonth, setActiveCustomMonth] = React.useState<number | null>(null);
  const [isSelectingShift, setIsSelectingShift] = React.useState(false);

  const [expandedChart, setExpandedChart] = React.useState<ChartKey | null>(null);
  const [chartVisibility, setChartVisibility] = React.useState<
    Record<ChartKey, Record<SeriesKey, boolean>>
  >({
    temperature: defaultSeriesState(),
    rain: defaultSeriesState(),
    wind: defaultSeriesState(),
    snow: defaultSeriesState(),
  });

  const [summary, setSummary] = React.useState<WeatherSummary | null>(null);
  const [loadingSummary, setLoadingSummary] = React.useState(false);
  const [summaryError, setSummaryError] = React.useState<string | null>(null);

  const projectId = selected?.opportunityId ?? selected?.id ?? null;
  const projectCity = meta?.city;
  const projectState = meta?.state;
  const { normals } = useProjectNormals(projectId ? String(projectId) : null);

  const climateSeries = React.useMemo<ClimateSeriesMap>(() => {
    if (!normals || normals.length === 0) {
      return CLIMATE_SERIES;
    }

    const sorted = [...normals].sort((a, b) => (a.month ?? 0) - (b.month ?? 0));
    const buildMonthLabel = (month: number | undefined, index: number) => {
      const monthIndex = (month ?? index + 1) - 1;
      if (monthIndex >= 0 && monthIndex < MONTH_LABELS.length) {
        return MONTH_LABELS[monthIndex];
      }

      const fallbackIndex = Math.max(index, 0);
      return MONTH_LABELS[fallbackIndex % MONTH_LABELS.length];
    };

    const temperatureData = sorted.map((normal, index) => ({
      month: buildMonthLabel(normal.month, index),
      twentyYearAvgHigh: normal.avgHighTemp ?? undefined,
      twentyYearAvgLow: normal.avgLowTemp ?? undefined,
      recordHigh: normal.recordAvgHighTemp ?? undefined,
      recordLow: normal.recordAvgLowTemp ?? undefined,
    }));

    const precipData = sorted.map((normal, index) => ({
      month: buildMonthLabel(normal.month, index),
      twentyYearAvgRain: normal.avgRain ?? undefined,
      recordHighRain: normal.recordHighAvgRain ?? undefined,
      recordLowRain: normal.recordLowAvgRain ?? undefined,
      twentyYearAvgSnow: normal.avgSnow ?? undefined,
      recordHighSnow: normal.recordHighAvgSnow ?? undefined,
      recordLowSnow: normal.recordLowAvgSnow ?? undefined,
    }));

    const windData = sorted.map((normal, index) => ({
      month: buildMonthLabel(normal.month, index),
      twentyYearAvgHigh: normal.avgHighWind ?? undefined,
      twentyYearAvgLow: normal.avgLowWind ?? undefined,
      recordHigh: normal.recordAvgHighWind ?? undefined,
      recordLow: normal.recordAvgLowWind ?? undefined,
    }));

    return {
      temperature: { ...CLIMATE_SERIES.temperature, data: temperatureData },
      rain: { ...CLIMATE_SERIES.rain, data: precipData },
      wind: { ...CLIMATE_SERIES.wind, data: windData },
      snow: { ...CLIMATE_SERIES.snow, data: precipData },
    };
  }, [normals]);

  const commitWindThreshold = React.useCallback(
    (rawValue: string) => {
      if (rawValue.trim() === "") {
        setAnalysisThresholdInput(String(analysisThreshold));
        return;
      }

      const numericValue = Number(rawValue);
      if (Number.isNaN(numericValue)) {
        setAnalysisThresholdInput(String(analysisThreshold));
        return;
      }

      const clampedValue = Math.min(
        WIND_THRESHOLD_MAX,
        Math.max(WIND_THRESHOLD_MIN, Math.round(numericValue))
      );
      setAnalysisThreshold(clampedValue);
      setAnalysisThresholdInput(String(clampedValue));
    },
    [analysisThreshold]
  );

  const commitHubHeight = React.useCallback(
    (rawValue: string) => {
      if (rawValue.trim() === "") {
        setHubHeightInput(String(hubHeight));
        return;
      }

      const numericValue = Number(rawValue);
      if (Number.isNaN(numericValue)) {
        setHubHeightInput(String(hubHeight));
        return;
      }

      const clampedValue = Math.min(
        HUB_HEIGHT_MAX,
        Math.max(HUB_HEIGHT_MIN, Math.round(numericValue))
      );
      setHubHeight(clampedValue);
      setHubHeightInput(String(clampedValue));
    },
    [hubHeight]
  );

  const commitShiftDuration = React.useCallback(
    (rawValue: string) => {
      if (rawValue.trim() === "") {
        setShiftDurationInput(String(shiftDuration));
        return;
      }

      const numericValue = Number(rawValue);
      if (Number.isNaN(numericValue)) {
        setShiftDurationInput(String(shiftDuration));
        return;
      }

      const clampedValue = clamp(Math.round(numericValue), SHIFT_DURATION_MIN, SHIFT_DURATION_MAX);
      setShiftDuration(clampedValue);
      setShiftDurationInput(String(clampedValue));
    },
    [shiftDuration]
  );

  React.useEffect(() => {
    setAnalysisThresholdInput(String(analysisThreshold));
  }, [analysisThreshold]);

  React.useEffect(() => {
    setHubHeightInput(String(hubHeight));
  }, [hubHeight]);

  React.useEffect(() => {
    setShiftDurationInput(String(shiftDuration));
  }, [shiftDuration]);

  React.useEffect(() => {
    const handleMouseUp = () => {
      setIsSelectingShift(false);
    };

    window.addEventListener("mouseup", handleMouseUp);
    window.addEventListener("blur", handleMouseUp);

    return () => {
      window.removeEventListener("mouseup", handleMouseUp);
      window.removeEventListener("blur", handleMouseUp);
    };
  }, []);

  React.useEffect(() => {
    if (!projectId) {
      setSummary(null);
      return;
    }

    let on = true;
    (async () => {
      try {
        setLoadingSummary(true);
        setSummaryError(null);
        const response = await fetchWeatherSummary({
          highWindThresholdMph: analysisThreshold,
          projectId: projectId ?? undefined,
          /*
          days: 30,
          highWindThresholdMph: analysisThreshold,
          projectId: projectId ?? undefined,
          city: selected?.projectCity,
          state: selected?.projectState,
          */
        });
        if (!on) return;
        setSummary(response);
      } catch (error: any) {
        if (!on) return;
        setSummaryError(error?.message || "Unable to load weather summary");
      } finally {
        if (!on) return;
        setLoadingSummary(false);
      }
    })();

    return () => {
      on = false;
    };
  }, [projectId, analysisThreshold]);

  const metricCards = React.useMemo(() => {
    const placeholderDays = 30;
    const totalPrecip = summary?.totalPrecipIn ?? 12.9;
    const avgWind = summary?.avgWindMaxMph ?? 12.7;
    const windyDays = summary?.highWindDays ?? 38;
    const forecastDays = summary?.days?.length ?? placeholderDays;

    return [
      {
        label: "30-Day Rainfall Total",
        value: `${totalPrecip.toFixed(1)} in`,
        sublabel: "Schedule concrete and earthwork around soggy stretches.",
      },
      {
        label: "Avg Daily Max Gust",
        value: `${avgWind.toFixed(1)} mph`,
        sublabel: `Stress-test crane ops at the ${analysisThreshold} mph threshold.`,
      },
      {
        label: "High-Wind Days",
        value: windyDays.toString(),
        sublabel: `Gusts projected above ${analysisThreshold} mph.`,
      },
      {
        label: "Forecast Horizon",
        value: `${forecastDays} days`,
        sublabel: projectCity ? "Location-adjusted outlook." : "Standard reference outlook.",
      },
    ];
  }, [summary, analysisThreshold, projectCity]);

  const climateEntries = React.useMemo(
    () =>
      [
        ["temperature", climateSeries.temperature],
        ["rain", climateSeries.rain],
        ["wind", climateSeries.wind],
        ["snow", climateSeries.snow],
      ] as [ChartKey, ClimateSeriesMap[ChartKey]][],
    [climateSeries]
  );

  const computedHeatmap = React.useMemo<HeatmapResponse | null>(() => {
    if (!metSamples || metSamples.length === 0) {
      return null;
    }

    return computeHeatmap(metSamples, analysisThreshold, hubHeight);
  }, [metSamples, analysisThreshold, hubHeight]);

  const heatmap = React.useMemo(() => {
    if (!computedHeatmap) {
      return null;
    }

    const rows: HeatmapRow[] = computedHeatmap.matrix.map((values, hourIndex) => {
      const hourLabel = computedHeatmap.hours?.[hourIndex] ?? `${hourIndex.toString().padStart(2, "0")}:00`;
      return {
        label: hourLabel,
        values: values.map((value) => (value == null ? null : Math.round(value))),
      };
    });

    return {
      rows,
      average: Math.round(computedHeatmap.averageExceedance ?? 0),
    };
  }, [computedHeatmap]);

  const heatmapMonths = React.useMemo(() => {
    if (!computedHeatmap) {
      return [] as string[];
    }

    if (computedHeatmap.months?.length) {
      return computedHeatmap.months;
    }

    const columnCount = computedHeatmap.matrix?.[0]?.length ?? 0;
    return Array.from({ length: columnCount }, (_, index) => `M${index + 1}`);
  }, [computedHeatmap]);

  const monthlySummaries = React.useMemo<MonthSummary[]>(() => {
    if (!computedHeatmap) {
      return [];
    }

    const matrix = computedHeatmap.matrix ?? [];
    const rows = matrix.length;
    const months = matrix[0]?.length ?? 0;
    if (!months) {
      return [];
    }

    const summaries: MonthSummary[] = [];
    for (let monthIndex = 0; monthIndex < months; monthIndex += 1) {
      let sum = 0;
      let count = 0;
      for (let rowIndex = 0; rowIndex < rows; rowIndex += 1) {
        const value = matrix[rowIndex]?.[monthIndex];
        if (typeof value !== "number") {
          continue;
        }
        sum += value;
        count += 1;
      }

      summaries.push({
        monthIndex,
        label: heatmapMonths[monthIndex] ?? `Month ${monthIndex + 1}`,
        average: count ? sum / count : null,
        coverage: rows ? count / rows : 0,
      });
    }

    return summaries;
  }, [computedHeatmap, heatmapMonths]);

  const shiftLength = React.useMemo(() => clamp(shiftDuration, SHIFT_DURATION_MIN, SHIFT_DURATION_MAX), [shiftDuration]);

  React.useEffect(() => {
    if (!computedHeatmap) {
      if (Object.keys(customShifts).length) {
        setCustomShifts({});
      }
      if (activeCustomMonth != null) {
        setActiveCustomMonth(null);
      }
      return;
    }

    const matrix = computedHeatmap.matrix ?? [];
    const hours = matrix.length;
    const months = matrix[0]?.length ?? 0;

    if (!hours || !months) {
      if (Object.keys(customShifts).length) {
        setCustomShifts({});
      }
      if (activeCustomMonth != null) {
        setActiveCustomMonth(null);
      }
      return;
    }

    const normalized: Record<number, { startRow: number }> = {};
    let changed = false;
    Object.entries(customShifts).forEach(([key, value]) => {
      const monthIndex = Number(key);
      if (!Number.isFinite(monthIndex) || monthIndex < 0 || monthIndex >= months) {
        changed = true;
        return;
      }

      const normalizedStart = ((value.startRow % hours) + hours) % hours;
      normalized[monthIndex] = { startRow: normalizedStart };
      if (normalizedStart !== value.startRow) {
        changed = true;
      }
    });

    if (Object.keys(normalized).length !== Object.keys(customShifts).length) {
      changed = true;
    }

    if (changed) {
      setCustomShifts(normalized);
    }

    if (activeCustomMonth != null && (activeCustomMonth < 0 || activeCustomMonth >= months)) {
      const fallbackKey = Object.keys(normalized)[0];
      setActiveCustomMonth(fallbackKey == null ? null : Number(fallbackKey));
    }
  }, [computedHeatmap, customShifts, activeCustomMonth]);

  const optimalShifts = React.useMemo(() => {
    if (!computedHeatmap) {
      return [] as ShiftWindow[];
    }

    const matrix = computedHeatmap.matrix ?? [];
    const hours = matrix.length;
    const months = matrix[0]?.length ?? 0;
    if (!hours || !months) {
      return [];
    }

    const windowLength = Math.min(shiftLength, hours);
    if (windowLength <= 0) {
      return [];
    }

    const results: ShiftWindow[] = [];
    for (let monthIndex = 0; monthIndex < months; monthIndex += 1) {
      let best: ShiftWindow | null = null;
      for (let startRow = 0; startRow < hours; startRow += 1) {
        const stats = computeShiftAverage(matrix, monthIndex, startRow, windowLength);
        if (!stats) {
          continue;
        }

        const coverage = stats.validObservations / windowLength;
        if (
          !best ||
          coverage > best.coverage + 0.001 ||
          (Math.abs(coverage - best.coverage) < 0.001 && (best.average == null || stats.average < best.average))
        ) {
          best = {
            monthIndex,
            startRow,
            average: stats.average,
            length: windowLength,
            coverage,
            validObservations: stats.validObservations,
          };
        }
      }

      if (!best) {
        best = {
          monthIndex,
          startRow: 0,
          average: null,
          length: windowLength,
          coverage: 0,
          validObservations: 0,
        };
      }

      results.push(best);
    }

    return results;
  }, [computedHeatmap, shiftLength]);

  React.useEffect(() => {
    if (!computedHeatmap) {
      return;
    }

    const matrix = computedHeatmap.matrix ?? [];
    const hours = matrix.length;
    const months = matrix[0]?.length ?? 0;
    if (!hours || !months) {
      return;
    }

    setCustomShifts((prev) => {
      let changed = false;
      const next: Record<number, { startRow: number }> = {};

      for (let monthIndex = 0; monthIndex < months; monthIndex += 1) {
        const optimal = optimalShifts[monthIndex];
        if (!optimal) {
          continue;
        }

        const normalizedStart = ((optimal.startRow % hours) + hours) % hours;
        next[monthIndex] = { startRow: normalizedStart };
        if (prev[monthIndex]?.startRow !== normalizedStart) {
          changed = true;
        }
      }

      if (Object.keys(prev).length !== Object.keys(next).length) {
        changed = true;
      }

      return changed ? next : prev;
    });
  }, [computedHeatmap, optimalShifts]);

  const bestShift = React.useMemo(() => {
    let winner: ShiftWindow | null = null;
    for (const shift of optimalShifts) {
      if (!shift) continue;
      if (!winner) {
        winner = shift;
        continue;
      }

      const winnerCoverage = winner.coverage ?? 0;
      const shiftCoverage = shift.coverage ?? 0;
      const winnerAverage = winner.average ?? Number.POSITIVE_INFINITY;
      const shiftAverage = shift.average ?? Number.POSITIVE_INFINITY;

      if (
        shiftCoverage > winnerCoverage + 0.001 ||
        (Math.abs(shiftCoverage - winnerCoverage) < 0.001 && shiftAverage < winnerAverage)
      ) {
        winner = shift;
      }
    }
    return winner;
  }, [optimalShifts]);

  const hourLabels = React.useMemo(() => {
    if (heatmap?.rows) {
      return heatmap.rows.map((row) => row.label);
    }

    if (computedHeatmap?.hours?.length) {
      return computedHeatmap.hours;
    }

    const rows = computedHeatmap?.matrix.length ?? 0;
    return Array.from({ length: rows }, (_, index) => `${index.toString().padStart(2, "0")}:00`);
  }, [heatmap, computedHeatmap]);

  const customShiftStatsByMonth = React.useMemo(() => {
    if (!computedHeatmap) {
      return {} as Record<number, ShiftWindow>;
    }

    const matrix = computedHeatmap.matrix ?? [];
    const hours = matrix.length;
    const months = matrix[0]?.length ?? 0;
    if (!hours || !months) {
      return {} as Record<number, ShiftWindow>;
    }

    const windowLength = Math.min(shiftLength, hours);
    if (windowLength <= 0) {
      return {} as Record<number, ShiftWindow>;
    }

    const results: Record<number, ShiftWindow> = {};
    for (let monthIndex = 0; monthIndex < months; monthIndex += 1) {
      const optimalStart = optimalShifts[monthIndex]?.startRow ?? 0;
      const customEntry = customShifts[monthIndex];
      const normalizedStart =
        customEntry?.startRow != null
          ? ((customEntry.startRow % hours) + hours) % hours
          : ((optimalStart % hours) + hours) % hours;

      const stats = computeShiftAverage(matrix, monthIndex, normalizedStart, windowLength);
      if (!stats) {
        results[monthIndex] = {
          monthIndex,
          startRow: normalizedStart,
          average: null,
          length: windowLength,
          coverage: 0,
          validObservations: 0,
        };
        continue;
      }

      results[monthIndex] = {
        monthIndex,
        startRow: normalizedStart,
        average: stats.average,
        length: windowLength,
        coverage: stats.validObservations / windowLength,
        validObservations: stats.validObservations,
      };
    }

    return results;
  }, [computedHeatmap, customShifts, optimalShifts, shiftLength]);

  const normalizeShiftStart = React.useCallback(
    (rowIndex: number) => {
      const rows = computedHeatmap?.matrix?.length ?? heatmap?.rows?.length ?? 0;
      if (!rows) {
        return 0;
      }

      const windowLength = Math.min(shiftLength, rows);
      if (windowLength <= 0) {
        return 0;
      }

      const normalized = ((rowIndex % rows) + rows) % rows;
      return normalized;
    },
    [computedHeatmap, heatmap, shiftLength],
  );

  const handleShiftSelectionStart = React.useCallback(
    (monthIndex: number, rowIndex: number) => {
      const months = computedHeatmap?.matrix?.[0]?.length ?? heatmapMonths.length;
      if (!months) {
        return;
      }

      const boundedMonth = clamp(monthIndex, 0, months - 1);
      const startRow = normalizeShiftStart(rowIndex);
      setCustomShifts((prev) => {
        const existing = prev[boundedMonth]?.startRow;
        if (existing === startRow) {
          return prev;
        }
        return { ...prev, [boundedMonth]: { startRow } };
      });
      setActiveCustomMonth(boundedMonth);
      setIsSelectingShift(true);
    },
    [computedHeatmap, heatmapMonths, normalizeShiftStart],
  );

  const handleShiftSelectionMove = React.useCallback(
    (monthIndex: number, rowIndex: number) => {
      if (!isSelectingShift) {
        return;
      }

      const months = computedHeatmap?.matrix?.[0]?.length ?? heatmapMonths.length;
      if (!months) {
        return;
      }

      const boundedMonth = clamp(monthIndex, 0, months - 1);
      const startRow = normalizeShiftStart(rowIndex);
      setCustomShifts((prev) => {
        const existing = prev[boundedMonth]?.startRow;
        if (existing === startRow) {
          return prev;
        }
        return { ...prev, [boundedMonth]: { startRow } };
      });
      setActiveCustomMonth(boundedMonth);
    },
    [computedHeatmap, heatmapMonths, isSelectingShift, normalizeShiftStart],
  );

  const handleShiftSelectionEnd = React.useCallback(() => {
    setIsSelectingShift(false);
  }, []);

  const describeWindow = React.useCallback(
    (startRow: number, length: number) => {
      if (!hourLabels.length) {
        return null;
      }

      const totalRows = hourLabels.length;
      if (totalRows === 0) {
        return null;
      }

      const normalizedStart = ((startRow % totalRows) + totalRows) % totalRows;
      const normalizedLength = Math.min(Math.max(length, 0), totalRows);
      const endRow = (normalizedStart + Math.max(normalizedLength - 1, 0)) % totalRows;
      const startLabel = hourLabels[normalizedStart] ?? `${normalizedStart.toString().padStart(2, "0")}:00`;
      const endLabel = hourLabels[endRow] ?? `${endRow.toString().padStart(2, "0")}:00`;
      return `${startLabel} – ${endLabel}`;
    },
    [hourLabels],
  );

  const optimalShiftLabelsByMonth = React.useMemo(() => {
    const labels: Record<number, string | null> = {};
    optimalShifts.forEach((shift) => {
      if (!shift) return;
      labels[shift.monthIndex] = describeWindow(shift.startRow, shift.length);
    });
    return labels;
  }, [optimalShifts, describeWindow]);

  const handleToggleSeries = (chartKey: ChartKey, seriesKey: SeriesKey) => {
    setChartVisibility((prev) => ({
      ...prev,
      [chartKey]: { ...prev[chartKey], [seriesKey]: !prev[chartKey][seriesKey] },
    }));
  };

  const handleFileUpload = (event: React.ChangeEvent<HTMLInputElement>) => {
    const files = Array.from(event.target.files ?? []);
    if (files.length === 0) return;

    setMetFiles((prev) => {
      const existing = new Map(prev.map((file) => [file.name + file.size, file]));
      files.forEach((file) => {
        existing.set(file.name + file.size, file);
      });
      return Array.from(existing.values());
    });
    setMetError(null);
    setHeatmapValidation([]);
    event.target.value = "";
  };

  const handleRemoveFile = React.useCallback(
    (name: string) => {
      setMetFiles((prev) => prev.filter((file) => file.name !== name));
      setMetError(null);
      setHeatmapValidation([]);
      setMetSamples(null);
    },
    [],
  );

  const handleProcessMetData = React.useCallback(async () => {
    if (metFiles.length === 0) {
      setMetError("Select at least one MET data file (.csv or .txt).");
      return;
    }

    setHeatmapLoading(true);
    setMetError(null);
    setHeatmapValidation([]);

    try {
      const { samples, errors } = await extractMetSamples(metFiles);
      if (errors.length) {
        setHeatmapValidation(errors);
        setMetError(errors[0]);
        setMetSamples(null);
        return;
      }

      setMetSamples(samples);
      setMetError(null);
      setHeatmapValidation([]);
    } catch (error: any) {
      setMetSamples(null);
      setHeatmapValidation([]);
      setMetError(error?.message || "Unable to process MET data files.");
    } finally {
      setHeatmapLoading(false);
    }
  }, [metFiles]);

  const activeChart = expandedChart ? climateSeries[expandedChart] : null;
  const locationLabel = [projectCity, projectState].filter(Boolean).join(", ");

  return (
    <div className="dashboard-card weather-analytics-card">
      <div className="page-heading">
        <h1 className="page-title">Weather Analytics</h1>
        <p className="page-subtitle">
          {selected?.opportunityName ?? "—"}
          {locationLabel ? ` • ${locationLabel}` : ""}
        </p>
      </div>

      <div className="analytics-tablist" role="tablist" aria-label="Weather analytics views">
        <button
          type="button"
          role="tab"
          aria-selected={activeTab === "general"}
          className={`analytics-tab ${activeTab === "general" ? "analytics-tab--active" : ""}`}
          onClick={() => setActiveTab("general")}
        >
          General Conditions
        </button>
        <button
          type="button"
          role="tab"
          aria-selected={activeTab === "wind"}
          className={`analytics-tab ${activeTab === "wind" ? "analytics-tab--active" : ""}`}
          onClick={() => setActiveTab("wind")}
        >
          Wind Analytics
        </button>
        <button
          type="button"
          role="tab"
          aria-selected={activeTab === "solar"}
          className={`analytics-tab ${activeTab === "solar" ? "analytics-tab--active" : ""}`}
          onClick={() => setActiveTab("solar")}
        >
          Solar Insights
        </button>
      </div>

      {activeTab === "general" && (
        <section aria-labelledby="general-conditions" className="analytics-tabpanel">
          <h2 id="general-conditions" className="sr-only">
            General conditions
          </h2>

          <div className="analytics-kpi-row">
            {metricCards.map((metric) => (
              <article key={metric.label} className="analytics-kpi-card">
                <div className="analytics-kpi-label">{metric.label}</div>
                <div className="analytics-kpi-value">{loadingSummary ? "…" : metric.value}</div>
                <div className="analytics-kpi-sub">{metric.sublabel}</div>
              </article>
            ))}
          </div>

          <div className="climate-chart-grid">
            {climateEntries.map(([chartKey, config]) => (
              <article key={chartKey} className="climate-chart-card">
                <header className="chart-card-header">
                  <div>
                    <h3>{config.title}</h3>
                    <p>{config.subtitle}</p>
                  </div>
                  <button
                    type="button"
                    className="chart-expand"
                    onClick={() => setExpandedChart(chartKey)}
                  >
                    Expand
                  </button>
                </header>

                <div className="chart-legend" role="group" aria-label={`${config.title} series toggles`}>
                  {config.seriesKeys.map((seriesKey) => {
                    const isVisible = chartVisibility[chartKey][seriesKey] ?? true;
                    return (
                      <button
                        key={seriesKey}
                        type="button"
                        className={`legend-toggle ${
                        isVisible ? "legend-toggle--active" : "legend-toggle--off"
                      }`}
                      style={{ borderColor: SERIES_CONFIG[seriesKey].color }}
                      onClick={() => handleToggleSeries(chartKey, seriesKey)}
                      title={SERIES_CONFIG[seriesKey].description}
                    >
                      <span
                        className="legend-swatch"
                        style={{ backgroundColor: SERIES_CONFIG[seriesKey].color }}
                        aria-hidden
                      />
                      <span>{SERIES_CONFIG[seriesKey].label}</span>
                    </button>
                    );
                  })}
                </div>

                <MultiSeriesChart
                  data={config.data}
                  seriesKeys={config.seriesKeys}
                  visibleSeries={chartVisibility[chartKey]}
                  unit={config.unit}
                />
                {config.meta?.footnotes?.length ? (
                  <div className="chart-footnote" aria-live="polite">
                    {config.meta.footnotes.map((footnote) => (
                      <div key={footnote.label} className="chart-footnote__item">
                        {footnote.swatch && (
                          <span
                            className={`chart-footnote__swatch chart-footnote__swatch--${footnote.swatch}`}
                            aria-hidden
                          />
                        )}
                        <span>
                          {footnote.label} <strong>{footnote.value}</strong>
                        </span>
                      </div>
                    ))}
                  </div>
                ) : null}
              </article>
            ))}
          </div>
        </section>
      )}

      {activeTab === "wind" && (
        <section aria-labelledby="wind-analytics" className="analytics-tabpanel">
          <h2 id="wind-analytics" className="sr-only">
            Wind analytics
          </h2>

          <div className="wind-layout">
            <div className="heatmap-wrapper">
              <article className="heatmap-card">
                <header>
                  <div className="heatmap-heading">
                    <h3>Operational Risk Heat Map</h3>
                    <p>
                      Observations above {analysisThreshold} mph at {hubHeight} m hub height.
                    </p>
                  </div>
                  <div className="heatmap-header-controls">
                    <div className="heatmap-summary">
                      {heatmapLoading ? (
                        <span className="heatmap-loading">Processing MET data…</span>
                      ) : heatmap ? (
                        <>
                          Average exceedance: <strong>{Math.round(heatmap.average)}%</strong>
                        </>
                      ) : (
                        <span className="heatmap-empty-label">No MET data</span>
                      )}
                    </div>
                  </div>
                </header>
                {heatmap && !heatmapLoading && (
                  <>
                    {monthlySummaries.length > 0 && (
                      <div className="heatmap-month-tabs">
                        <div className="heatmap-month-tabs__spacer" aria-hidden />
                        {monthlySummaries.map((summary) => {
                          const isOverallBest =
                            bestShift && bestShift.average != null && bestShift.monthIndex === summary.monthIndex;
                          const optimalStats = optimalShifts[summary.monthIndex] ?? null;
                          const optimalAverage = optimalStats?.average ?? null;
                          const optimalLabel = optimalShiftLabelsByMonth[summary.monthIndex] ?? "—";
                          const customStats = customShiftStatsByMonth[summary.monthIndex] ?? null;
                          const customLabel = customStats
                            ? describeWindow(customStats.startRow, customStats.length)
                            : null;
                          const customAverage = customStats?.average ?? null;
                          const isActiveCustom = activeCustomMonth === summary.monthIndex;
                          let deltaLabel = "—";
                          const deltaClassNames = ["heatmap-month-tab__delta-value"];
                          if (customAverage != null && optimalAverage != null) {
                            const delta = customAverage - optimalAverage;
                            const magnitude = Math.abs(delta);
                            if (magnitude < 0.05) {
                              deltaLabel = "0.0% worse";
                              deltaClassNames.push("heatmap-month-tab__delta-value--neutral");
                            } else if (delta > 0) {
                              deltaLabel = `${formatPercent(magnitude, 1)} worse`;
                              deltaClassNames.push("heatmap-month-tab__delta-value--worse");
                            } else {
                              deltaLabel = `${formatPercent(magnitude, 1)} better`;
                              deltaClassNames.push("heatmap-month-tab__delta-value--better");
                            }
                          }
                          return (
                            <div
                              key={`month-summary-${summary.monthIndex}`}
                              className={`heatmap-month-tab${
                                isOverallBest ? " heatmap-month-tab--optimal" : ""
                              }${isActiveCustom && customStats ? " heatmap-month-tab--custom-active" : ""}`}
                              role="button"
                              tabIndex={0}
                              aria-pressed={isActiveCustom && !!customStats}
                              onClick={() => {
                                setActiveCustomMonth(customStats ? summary.monthIndex : null);
                              }}
                              onKeyDown={(event) => {
                                if (event.key === "Enter" || event.key === " " || event.key === "Space") {
                                  event.preventDefault();
                                  setActiveCustomMonth(customStats ? summary.monthIndex : null);
                                }
                              }}
                            >
                              {isOverallBest && <span className="heatmap-month-tab__badge">Optimal month</span>}
                              <span className="heatmap-month-tab__label">{summary.label}</span>
                              <span className="heatmap-month-tab__value">{formatPercent(summary.average, 1)}</span>
                              <span className="heatmap-month-tab__hint">avg exceedance</span>
                              <div className="heatmap-month-tab__shifts">
                                <div className="heatmap-month-tab__shift heatmap-month-tab__shift--optimal">
                                  <span className="heatmap-month-tab__shift-label">Optimal shift</span>
                                  <span className="heatmap-month-tab__shift-time">
                                    {optimalLabel}
                                    {optimalAverage != null ? ` • ${formatPercent(optimalAverage, 1)}` : ""}
                                  </span>
                                </div>
                                <div className="heatmap-month-tab__shift heatmap-month-tab__shift--custom">
                                  <span className="heatmap-month-tab__shift-label">Custom shift</span>
                                  <span className="heatmap-month-tab__shift-time">
                                    {customLabel ?? "—"}
                                    {customAverage != null ? ` • ${formatPercent(customAverage, 1)}` : ""}
                                  </span>
                                </div>
                                <div className="heatmap-month-tab__delta">
                                  <span className="heatmap-month-tab__delta-label">Custom vs optimal</span>
                                  <span className={deltaClassNames.join(" ")}>{deltaLabel}</span>
                                </div>
                              </div>
                            </div>
                          );
                        })}
                      </div>
                    )}
                  </>
                )}
                {heatmap ? (
                  <>
                    <div className="heatmap-table-wrapper">
                      <table className="heatmap-table">
                        <thead>
                          <tr>
                            <th>Hour</th>
                            {heatmapMonths.map((month, index) => (
                              <th key={`${month}-${index}`}>{month}</th>
                            ))}
                          </tr>
                        </thead>
                        <tbody>
                          {heatmap.rows.map((row, rowIndex) => (
                            <tr key={row.label}>
                              <th scope="row">{row.label}</th>
                              {row.values.map((value, monthIndex) => {
                                const optimalForMonth = optimalShifts[monthIndex] ?? null;
                                const customForMonth = customShiftStatsByMonth[monthIndex] ?? null;
                                const totalRows = heatmap.rows.length;
                                const optimalLength = optimalForMonth?.length ?? 0;
                                const optimalStart =
                                  optimalForMonth != null ? normalizeIndex(optimalForMonth.startRow, totalRows) : -1;
                                const optimalEnd =
                                  optimalForMonth != null
                                    ? (optimalStart + Math.max(optimalLength - 1, 0)) % Math.max(totalRows, 1)
                                    : -1;
                                const optimalWraps =
                                  optimalForMonth != null
                                  ? windowWraps(optimalForMonth.startRow, optimalForMonth.length, totalRows)
                                  : false;
                                const isOptimalCell =
                                  !!optimalForMonth &&
                                  optimalForMonth.average != null &&
                                  isIndexWithinWindow(
                                    rowIndex,
                                    optimalForMonth.startRow,
                                    optimalForMonth.length,
                                    totalRows,
                                  );
                                const isOptimalStart =
                                  isOptimalCell &&
                                  optimalForMonth != null &&
                                  (rowIndex === optimalStart || (optimalWraps && rowIndex === 0));
                                const isOptimalEnd =
                                  isOptimalCell &&
                                  optimalForMonth != null &&
                                  (rowIndex === optimalEnd || (optimalWraps && rowIndex === totalRows - 1));

                                const isCustomCell =
                                  !!customForMonth &&
                                  isIndexWithinWindow(
                                    rowIndex,
                                    customForMonth.startRow,
                                    customForMonth.length,
                                    totalRows,
                                  );

                                const customStart =
                                  customForMonth != null ? normalizeIndex(customForMonth.startRow, totalRows) : -1;
                                const customEnd =
                                  customForMonth != null
                                    ? (customStart + Math.max(customForMonth.length - 1, 0)) % Math.max(totalRows, 1)
                                    : -1;
                                const customWraps =
                                  customForMonth != null
                                    ? windowWraps(customForMonth.startRow, customForMonth.length, totalRows)
                                    : false;
                                const isCustomStart =
                                  isCustomCell &&
                                  customForMonth != null &&
                                  (rowIndex === customStart || (customWraps && rowIndex === 0));
                                const isCustomEnd =
                                  isCustomCell &&
                                  customForMonth != null &&
                                  (rowIndex === customEnd || (customWraps && rowIndex === totalRows - 1));

                                const cellClassNames = ["heatmap-cell"];
                                if (isOptimalCell) cellClassNames.push("heatmap-cell--optimal");
                                if (isOptimalStart) cellClassNames.push("heatmap-cell--optimal-start");
                                if (isOptimalEnd) cellClassNames.push("heatmap-cell--optimal-end");
                                if (isCustomCell) cellClassNames.push("heatmap-cell--custom");
                                if (isCustomStart) cellClassNames.push("heatmap-cell--custom-start");
                                if (isCustomEnd) cellClassNames.push("heatmap-cell--custom-end");

                                return (
                                  <td
                                    key={`${row.label}-${heatmapMonths[monthIndex] ?? monthIndex}`}
                                    className={cellClassNames.join(" ")}
                                    style={{ background: heatColor(value) }}
                                    data-month-index={monthIndex}
                                    data-row-index={rowIndex}
                                    onMouseDown={(event) => {
                                      event.preventDefault();
                                      handleShiftSelectionStart(monthIndex, rowIndex);
                                    }}
                                    onMouseEnter={() => handleShiftSelectionMove(monthIndex, rowIndex)}
                                    onMouseUp={handleShiftSelectionEnd}
                                    onTouchStart={(event) => {
                                      event.preventDefault();
                                      handleShiftSelectionStart(monthIndex, rowIndex);
                                    }}
                                    onTouchMove={(event) => {
                                      const touch = event.touches[0];
                                      if (!touch) return;
                                      const element = document.elementFromPoint(touch.clientX, touch.clientY);
                                      const monthAttr = element?.getAttribute?.("data-month-index");
                                      const rowAttr = element?.getAttribute?.("data-row-index");
                                      if (monthAttr == null || rowAttr == null) return;
                                      handleShiftSelectionMove(Number(monthAttr), Number(rowAttr));
                                    }}
                                    onTouchEnd={handleShiftSelectionEnd}
                                    onTouchCancel={handleShiftSelectionEnd}
                                  >
                                    <span className="heatmap-cell-value">{value == null ? "—" : `${value}%`}</span>
                                  </td>
                                );
                              })}
                            </tr>
                          ))}
                        </tbody>
                      </table>
                    </div>
                    <p className="heatmap-footnote">
                      Colors scale from low (green) to high (red) exceedance frequency. Align crews with the calmest
                      hours, or upload MET data to tailor the diurnal profile to your site.
                    </p>
                  </>
                ) : (
                  !heatmapLoading && (
                    <div className="heatmap-empty">
                      <h4>No MET data available</h4>
                      <p>Upload MET measurements to generate the operational risk heat map.</p>
                    </div>
                  )
                )}
              </article>
            </div>
            <div className="wind-controls-column">
              <article className="wind-card">
                <header>
                  <h3>MET Data Upload</h3>
                  <p>Ingest on-site measurements to refine thresholds.</p>
                </header>
                <div className="upload-panel">
                  <label className="upload-drop" htmlFor="met-upload">
                    <span className="upload-instructions">
                      <strong>Drag &amp; drop MET files</strong>
                      <small>.csv or .txt up to 10 MB</small>
                    </span>
                    <input
                      id="met-upload"
                      type="file"
                      accept=".csv,.txt"
                      multiple
                      onChange={handleFileUpload}
                    />
                  </label>
                  {metFiles.length > 0 && (
                    <ul className="upload-file-list">
                      {metFiles.map((file) => (
                        <li key={file.name}>
                          <div>
                            <strong>{file.name}</strong>
                            <small>{Math.round(file.size / 1024)} KB</small>
                          </div>
                          <button type="button" onClick={() => handleRemoveFile(file.name)} aria-label={`Remove ${file.name}`}>
                            ×
                          </button>
                        </li>
                      ))}
                    </ul>
                  )}
                  <button
                    type="button"
                    className="primary-btn"
                    onClick={handleProcessMetData}
                    disabled={metFiles.length === 0 || heatmapLoading}
                  >
                    {heatmapLoading
                      ? "Processing…"
                      : metSamples
                        ? "Re-run analysis"
                        : "Run analysis"}
                  </button>
                  {metError && <p className="upload-error">{metError}</p>}
                  {heatmapValidation.length > 0 && (
                    <ul className="upload-error-list">
                      {heatmapValidation.map((message, index) => (
                        <li key={`${message}-${index}`}>{message}</li>
                      ))}
                    </ul>
                  )}
                </div>
              </article>

              <article className="wind-card">
                <header>
                  <h3>Threshold Settings</h3>
                  <p>Adjust operating limits to stress test schedules.</p>
                </header>
                <div className="slider-control">
                  <label htmlFor="threshold-range">Wind speed threshold</label>
                  <div className="slider-inputs">
                    <input
                      id="threshold-range"
                      type="range"
                      min={WIND_THRESHOLD_MIN}
                      max={WIND_THRESHOLD_MAX}
                      step={1}
                      value={analysisThreshold}
                      onChange={(event) => setAnalysisThreshold(Number(event.target.value))}
                    />
                    <div className="slider-number-wrapper">
                      <input
                        id="threshold-input"
                        type="number"
                        min={WIND_THRESHOLD_MIN}
                        max={WIND_THRESHOLD_MAX}
                        step={1}
                        value={analysisThresholdInput}
                        aria-label="Wind speed threshold (mph)"
                        onChange={(event) => setAnalysisThresholdInput(event.target.value)}
                        onBlur={(event) => commitWindThreshold(event.currentTarget.value)}
                        onKeyDown={(event) => {
                          if (event.key === "Enter") {
                            event.preventDefault();
                            commitWindThreshold(event.currentTarget.value);
                          }
                        }}
                      />
                      <span aria-hidden>mph</span>
                    </div>
                  </div>
                </div>
                <p className="control-footnote">
                  Values represent gusts exceeding operational tolerance.
                </p>
              </article>

              <article className="wind-card">
                <header>
                  <h3>Hub Height</h3>
                  <p>Normalize exposure levels for turbine assessment.</p>
                </header>
                <div className="slider-control">
                  <label htmlFor="hub-height-range">Reference hub height</label>
                  <div className="slider-inputs">
                    <input
                      id="hub-height-range"
                      type="range"
                      min={HUB_HEIGHT_MIN}
                      max={HUB_HEIGHT_MAX}
                      step={5}
                      value={hubHeight}
                      onChange={(event) => setHubHeight(Number(event.target.value))}
                    />
                    <div className="slider-number-wrapper">
                      <input
                        id="hub-height-input"
                        type="number"
                        min={HUB_HEIGHT_MIN}
                        max={HUB_HEIGHT_MAX}
                        step={1}
                        value={hubHeightInput}
                        aria-label="Reference hub height (m)"
                        onChange={(event) => setHubHeightInput(event.target.value)}
                        onBlur={(event) => commitHubHeight(event.currentTarget.value)}
                        onKeyDown={(event) => {
                          if (event.key === "Enter") {
                            event.preventDefault();
                            commitHubHeight(event.currentTarget.value);
                          }
                        }}
                      />
                      <span aria-hidden>m</span>
                    </div>
                  </div>
                </div>
                <p className="control-footnote">Impacts shear correction for above-threshold counts.</p>
              </article>

              <article className="wind-card">
                <header>
                  <h3>Shift Duration</h3>
                  <p>Surface calmer crew windows for every month.</p>
                </header>
                <div className="slider-control">
                  <label htmlFor="shift-duration-range">Crew shift length</label>
                  <div className="slider-inputs">
                    <input
                      id="shift-duration-range"
                      type="range"
                      min={SHIFT_DURATION_MIN}
                      max={SHIFT_DURATION_MAX}
                      step={1}
                      value={shiftDuration}
                      onChange={(event) => {
                        const nextValue = Number(event.target.value);
                        setShiftDuration(nextValue);
                        setShiftDurationInput(String(nextValue));
                      }}
                    />
                    <div className="slider-number-wrapper">
                      <input
                        id="shift-duration-input"
                        type="number"
                        min={SHIFT_DURATION_MIN}
                        max={SHIFT_DURATION_MAX}
                        step={1}
                        value={shiftDurationInput}
                        aria-label="Crew shift length (hours)"
                        onChange={(event) => setShiftDurationInput(event.target.value)}
                        onBlur={(event) => commitShiftDuration(event.currentTarget.value)}
                        onKeyDown={(event) => {
                          if (event.key === "Enter") {
                            event.preventDefault();
                            commitShiftDuration(event.currentTarget.value);
                          }
                        }}
                      />
                      <span aria-hidden>hr</span>
                    </div>
                  </div>
                </div>
                <p className="control-footnote">Applies the selected shift window across all months.</p>
              </article>
            </div>
          </div>

        </section>
      )}

      {activeTab === "solar" && (
        <section aria-labelledby="solar-insights" className="analytics-tabpanel">
          <h2 id="solar-insights" className="sr-only">
            Solar insights
          </h2>
          <div className="placeholder-card">
            <p>
              Solar analytics are coming soon. Upload MET data or adjust wind thresholds while the solar module is
              finalized.
            </p>
          </div>
        </section>
      )}

      {summaryError && <div className="analytics-error">{summaryError}</div>}

      {expandedChart && activeChart && (
        <ExpandedChartModal
          chartKey={expandedChart}
          chart={activeChart}
          visibleSeries={chartVisibility[expandedChart]}
          onToggleSeries={handleToggleSeries}
          onDismiss={() => setExpandedChart(null)}
        />
      )}
    </div>
  );
}

type MultiSeriesChartProps = {
  data: ClimateDataPoint[];
  seriesKeys: SeriesKey[];
  visibleSeries: Record<SeriesKey, boolean>;
  unit: string;
  height?: number;
  showAxes?: boolean;
};

function MultiSeriesChart({
  data,
  seriesKeys,
  visibleSeries,
  unit,
  height: requestedHeight = 240,
  showAxes = true,
}: MultiSeriesChartProps) {
  const padding = { top: 30, right: 30, bottom: 42, left: 64 };
  const width = 820;
  const minOuterHeight = padding.top + padding.bottom + 60;
  const outerHeight = Math.max(requestedHeight, minOuterHeight);
  const activeSeries = seriesKeys.filter((key) => (visibleSeries[key] ?? true));
  const renderSeries = activeSeries.length ? activeSeries : seriesKeys.slice(0, 1);

  if (!renderSeries.length) {
    return <div className="chart-empty">Select at least one series to view the chart.</div>;
  }

  const numericValues = renderSeries
    .flatMap((key) => data.map((point) => point[key]))
    .filter((value): value is number => typeof value === "number" && Number.isFinite(value));

  if (!numericValues.length) {
    return <div className="chart-empty">No data available for the selected series.</div>;
  }

  const rawMin = Math.min(...numericValues);
  const rawMax = Math.max(...numericValues);
  const rawSpan = rawMax - rawMin || 1;
  const paddingSize = rawSpan * 0.12;
  let minDomain = rawMin - paddingSize;
  let maxDomain = rawMax + paddingSize;

  if (rawMin >= 0) {
    minDomain = Math.max(0, rawMin - paddingSize * 1.1);
  }
  if (rawMax <= 0) {
    maxDomain = Math.min(0, rawMax + paddingSize * 1.1);
  }
  if (minDomain === maxDomain) {
    maxDomain = minDomain + 1;
  }

  const chartHeight = outerHeight - padding.top - padding.bottom;
  const chartWidth = width - padding.left - padding.right;
  const stepX = chartWidth / Math.max(1, data.length - 1);

  const scaleY = (value: number) =>
    padding.top + chartHeight - ((value - minDomain) / (maxDomain - minDomain)) * chartHeight;
  const scaleX = (index: number) => padding.left + index * stepX;

  const tickCount = 5;
  const gridLines = Array.from({ length: tickCount }, (_, index) => {
    const ratio = index / (tickCount - 1 || 1);
    const value = minDomain + (maxDomain - minDomain) * ratio;
    const y = scaleY(value);
    return { value, y };
  });

  const formatTick = (value: number) => {
    if (unit === "in") {
      return value.toFixed(1);
    }
    if (Math.abs(value) >= 100) {
      return value.toFixed(0);
    }
    if (Math.abs(value) >= 10) {
      return value.toFixed(0);
    }
    return value.toFixed(1);
  };

  return (
    <svg
      viewBox={`0 0 ${width} ${outerHeight}`}
      height={outerHeight}
      className="multi-series-chart"
      role="img"
      aria-label="Climate chart"
    >
      <defs>
        <filter id="shadow" x="-50%" y="-50%" width="200%" height="200%">
          <feDropShadow dx="0" dy="2" stdDeviation="2" floodColor="rgba(15, 23, 42, 0.25)" />
        </filter>
      </defs>
      {showAxes && (
        <>
          <line
            x1={padding.left}
            y1={padding.top}
            x2={padding.left}
            y2={outerHeight - padding.bottom}
            className="chart-axis"
          />
          <line
            x1={padding.left}
            y1={outerHeight - padding.bottom}
            x2={width - padding.right}
            y2={outerHeight - padding.bottom}
            className="chart-axis"
          />
          {gridLines.map((tick, index) => (
            <g key={index}>
              <line
                x1={padding.left}
                y1={tick.y}
                x2={width - padding.right}
                y2={tick.y}
                className="chart-gridline"
              />
              <text x={padding.left - 12} y={tick.y + 4} className="chart-axis-label" textAnchor="end">
                {formatTick(tick.value)}
              </text>
            </g>
          ))}
          {data.map((point, index) => (
            <text
              key={point.month}
              x={scaleX(index)}
              y={outerHeight - padding.bottom + 28}
              className="chart-axis-label"
            >
              {point.month}
            </text>
          ))}
        </>
      )}

      {renderSeries.map((seriesKey) => {
        const segments: string[] = [];
        data.forEach((point, index) => {
          const value = point[seriesKey];
          if (typeof value !== "number" || Number.isNaN(value)) {
            return;
          }
          const command = segments.length === 0 ? "M" : "L";
          segments.push(`${command}${scaleX(index)},${scaleY(value)}`);
        });

        if (!segments.length) {
          return null;
        }

        return (
          <g key={seriesKey}>
            <path
              d={segments.join(" ")}
              fill="none"
              stroke={SERIES_CONFIG[seriesKey].color}
              strokeWidth={2.8}
              strokeLinecap="round"
              className="chart-line"
            />
            {data.map((point, index) => {
              const value = point[seriesKey];
              if (typeof value !== "number" || Number.isNaN(value)) {
                return null;
              }
              const x = scaleX(index);
              const y = scaleY(value);
              return (
                <circle
                  key={`${seriesKey}-${index}`}
                  cx={x}
                  cy={y}
                  r={4.5}
                  fill="#ffffff"
                  stroke={SERIES_CONFIG[seriesKey].color}
                  strokeWidth={2}
                  filter="url(#shadow)"
                >
                  <title>{`${point.month}: ${fmtNumber(value, unit)}`}</title>
                </circle>
              );
            })}
          </g>
        );
      })}
    </svg>
  );
}

type ExpandedChartModalProps = {
  chartKey: ChartKey;
  chart: ClimateSeriesMap[ChartKey];
  visibleSeries: Record<SeriesKey, boolean>;
  onToggleSeries: (chartKey: ChartKey, seriesKey: SeriesKey) => void;
  onDismiss: () => void;
};

function ExpandedChartModal({ chartKey, chart, visibleSeries, onToggleSeries, onDismiss }: ExpandedChartModalProps) {
  const renderSeries = React.useMemo(() => {
    const active = chart.seriesKeys.filter((key) => visibleSeries[key] ?? true);
    return active.length ? active : chart.seriesKeys.slice(0, 1);
  }, [chart.seriesKeys, visibleSeries]);

  const highlight = React.useMemo(() => {
    const fallbackKey =
      renderSeries[0] ?? chart.seriesKeys[0] ?? (Object.keys(SERIES_CONFIG)[0] as SeriesKey);
    if (!chart.data.length) {
      return {
        highPoint: chart.data[0] ?? ({ month: "—" } as ClimateDataPoint),
        lowPoint: chart.data[0] ?? ({ month: "—" } as ClimateDataPoint),
        highKey: fallbackKey,
        lowKey: fallbackKey,
      };
    }

    const ensureInView = (key?: SeriesKey) => (key && renderSeries.includes(key) ? key : undefined);
    const findByRole = (roles: SeriesRole[]) =>
      renderSeries.find((key) => roles.includes(SERIES_CONFIG[key].role));

    const highKey =
      ensureInView(chart.insightHighKey) ??
      findByRole(["recordHigh", "trendHigh", "high"]) ??
      fallbackKey;
    const lowKey =
      ensureInView(chart.insightLowKey) ??
      findByRole(["recordLow", "trendLow", "low"]) ??
      renderSeries[renderSeries.length - 1] ??
      fallbackKey;

    const pickExtreme = (
      key: SeriesKey,
      comparator: (curr: number, prev: number) => boolean,
    ): ClimateDataPoint => {
      return chart.data.reduce((prev, curr) => {
        const prevValue = prev?.[key];
        const currValue = curr?.[key];
        if (typeof currValue !== "number") {
          return prev;
        }
        if (typeof prevValue !== "number") {
          return curr;
        }
        return comparator(currValue, prevValue) ? curr : prev;
      });
    };

    const highPoint = pickExtreme(highKey, (curr, prev) => curr > prev);
    const lowPoint = pickExtreme(lowKey, (curr, prev) => curr < prev);
    return { highPoint, lowPoint, highKey, lowKey };
  }, [chart.data, chart.insightHighKey, chart.insightLowKey, chart.seriesKeys, renderSeries]);

  return (
    <div className="chart-modal" role="dialog" aria-modal="true" aria-labelledby="expanded-chart-title">
      <div className="chart-modal__backdrop" onClick={onDismiss} />
      <div className="chart-modal__content">
        <header className="chart-modal__header">
          <div>
            <h3 id="expanded-chart-title">{chart.title}</h3>
            <p>{chart.subtitle}</p>
          </div>
          <button type="button" onClick={onDismiss} className="chart-modal__close">
            Close
          </button>
        </header>

        <div className="chart-modal__legend">
          {chart.seriesKeys.map((seriesKey) => {
            const isVisible = visibleSeries[seriesKey] ?? true;
            return (
              <button
                key={seriesKey}
                type="button"
                className={`legend-toggle ${isVisible ? "legend-toggle--active" : "legend-toggle--off"}`}
                onClick={() => onToggleSeries(chartKey, seriesKey)}
                style={{ borderColor: SERIES_CONFIG[seriesKey].color }}
                title={SERIES_CONFIG[seriesKey].description}
              >
                <span className="legend-swatch" style={{ backgroundColor: SERIES_CONFIG[seriesKey].color }} aria-hidden />
                <span>{SERIES_CONFIG[seriesKey].label}</span>
              </button>
            );
          })}
        </div>

        <MultiSeriesChart
          data={chart.data}
          seriesKeys={chart.seriesKeys}
          visibleSeries={visibleSeries}
          unit={chart.unit}
          height={360}
        />

        <p className="chart-modal__insight">
          Peak month ({SERIES_CONFIG[highlight.highKey].label}):&nbsp;
          <strong>{highlight.highPoint.month}</strong> ({fmtNumber(highlight.highPoint[highlight.highKey], chart.unit)}).&nbsp;
          Lowest month ({SERIES_CONFIG[highlight.lowKey].label}):&nbsp;
          <strong>{highlight.lowPoint.month}</strong> ({fmtNumber(highlight.lowPoint[highlight.lowKey], chart.unit)}).
        </p>

        <div className="chart-modal__table-wrapper">
          <table>
            <thead>
              <tr>
                <th>Month</th>
                {renderSeries.map((seriesKey) => (
                  <th key={seriesKey}>{SERIES_CONFIG[seriesKey].label}</th>
                ))}
              </tr>
            </thead>
            <tbody>
              {chart.data.map((row) => (
                <tr key={row.month}>
                  <th scope="row">{row.month}</th>
                  {renderSeries.map((seriesKey) => (
                    <td key={seriesKey}>{fmtNumber(row[seriesKey], chart.unit)}</td>
                  ))}
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}

function heatColor(value: number | null) {
  if (value == null) {
    return "rgba(226, 232, 240, 0.8)"; // muted gray for insufficient data
  }

  const clamped = Math.max(0, Math.min(100, value));
  const ratio = clamped / 100;
  const low = { r: 34, g: 197, b: 94 }; // #22c55e
  const mid = { r: 250, g: 204, b: 21 }; // #facc15
  const high = { r: 220, g: 38, b: 38 }; // #dc2626

  let red: number;
  let green: number;
  let blue: number;

  if (ratio <= 0.5) {
    const t = ratio / 0.5;
    red = Math.round(low.r + (mid.r - low.r) * t);
    green = Math.round(low.g + (mid.g - low.g) * t);
    blue = Math.round(low.b + (mid.b - low.b) * t);
  } else {
    const t = (ratio - 0.5) / 0.5;
    red = Math.round(mid.r + (high.r - mid.r) * t);
    green = Math.round(mid.g + (high.g - mid.g) * t);
    blue = Math.round(mid.b + (high.b - mid.b) * t);
  }

  return `rgba(${red}, ${green}, ${blue}, 0.85)`;
}
