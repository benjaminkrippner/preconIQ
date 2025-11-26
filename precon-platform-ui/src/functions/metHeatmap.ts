import { parseCSV, csvToObjects } from "./csv";
import type { HeatmapResponse, MetSample } from "../types/heatmap";

const TIMESTAMP_FIELDS = ["timestamp", "time", "date_time", "datetime", "date", "date_time_local"];
const WIND_MPH_FIELDS = [
  "wind_mph",
  "windspeed_mph",
  "wind_speed_mph",
  "wind_speed",
  "windspeed",
  "ws_mph",
  "ws",
];
const WIND_MS_FIELDS = [
  "wind_ms",
  "windspeed_ms",
  "wind_speed_ms",
  "wind_mps",
  "windspeed_mps",
  "ws_ms",
];
const HEIGHT_FIELDS = ["height_m", "height", "sensor_height", "sensor_height_m", "hub_height", "hub_height_m"];
const DATE_ONLY_FIELDS = ["date", "date_local", "day_date"];
const TIME_ONLY_FIELDS = ["time", "time_local", "hhmm", "hour_minute"];
const YEAR_FIELDS = ["year", "yyyy"];
const MONTH_FIELDS = ["month", "mm", "mon"];
const DAY_FIELDS = ["day", "dd", "day_of_month"];
const HOUR_FIELDS = ["hour", "hr", "hh", "hour_local"];
const MINUTE_FIELDS = ["minute", "min", "mm_minute"];
const SECOND_FIELDS = ["second", "sec", "ss"];

const HOURS = Array.from({ length: 24 }, (_, index) => `${index.toString().padStart(2, "0")}:00`);
const MONTHS = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
const MS_TO_MPH = 2.23694;
const DEFAULT_ALPHA = 0.143;
const MIN_SAMPLE_COUNT = 3;

const normaliseHeader = (value: string) => value.trim().toLowerCase().replace(/\s+|[^a-z0-9]/g, "_");

const pickField = (headers: string[], candidates: string[]) =>
  headers.find((header) => candidates.includes(header));

const normaliseDelimiter = (text: string) => {
  const [firstLine = ""] = text.split(/\r?\n/, 1);
  if (firstLine.includes(";") && !firstLine.includes(",")) {
    return text.replace(/;/g, ",");
  }
  if (firstLine.includes("\t") && !firstLine.includes(",")) {
    return text.replace(/\t/g, ",");
  }
  return text;
};

const parseTimestamp = (value: string): Date | null => {
  const trimmed = value.trim();
  if (!trimmed) {
    return null;
  }

  const numeric = Number(trimmed);
  if (Number.isFinite(numeric) && trimmed.length >= 5) {
    const fromEpoch = new Date(numeric);
    if (!Number.isNaN(fromEpoch.getTime())) {
      return fromEpoch;
    }
  }

  const european = trimmed.match(
    /^(\d{1,2})[/-](\d{1,2})[/-](\d{2,4})(?:[ T](\d{1,2})(?::(\d{1,2})(?::(\d{1,2}))?)?)?$/
  );
  if (european) {
    const [, dd, mm, yyyy, hh = "0", min = "0", ss = "0"] = european;
    const year = Number.parseInt(yyyy.length === 2 ? `20${yyyy}` : yyyy, 10);
    const month = Number.parseInt(mm, 10) - 1;
    const day = Number.parseInt(dd, 10);
    const hour = Number.parseInt(hh, 10);
    const minute = Number.parseInt(min, 10);
    const second = Number.parseInt(ss, 10);
    const fromParts = new Date(year, month, day, hour, minute, second);
    if (!Number.isNaN(fromParts.getTime())) {
      return fromParts;
    }
  }

  const isoLike = trimmed.replace(/\//g, "-");
  const parsed = new Date(isoLike);
  if (!Number.isNaN(parsed.getTime())) {
    return parsed;
  }

  return null;
};

const coerceInt = (value: string | undefined) => {
  if (!value) return null;
  const parsed = Number.parseInt(value, 10);
  return Number.isFinite(parsed) ? parsed : null;
};

const parseDateParts = (record: Record<string, string>, fields: {
  year?: string;
  month?: string;
  day?: string;
  hour?: string;
  minute?: string;
  second?: string;
}) => {
  const year = coerceInt(fields.year ? record[fields.year] : undefined);
  const month = coerceInt(fields.month ? record[fields.month] : undefined);
  const day = coerceInt(fields.day ? record[fields.day] : undefined);
  if (year == null || month == null || day == null) {
    return null;
  }

  const hourRaw = coerceInt(fields.hour ? record[fields.hour] : undefined);
  const minuteRaw = coerceInt(fields.minute ? record[fields.minute] : undefined);
  const secondRaw = coerceInt(fields.second ? record[fields.second] : undefined);

  const hour = hourRaw == null ? 0 : Math.max(0, Math.min(23, hourRaw));
  const minute = minuteRaw == null ? 0 : Math.max(0, Math.min(59, minuteRaw));
  const second = secondRaw == null ? 0 : Math.max(0, Math.min(59, secondRaw));

  const constructed = new Date(Date.UTC(year, month - 1, day, hour, minute, second));
  if (Number.isNaN(constructed.getTime())) {
    return null;
  }

  return constructed;
};

const parseDateAndTimeFields = (record: Record<string, string>, dateField?: string, timeField?: string) => {
  if (!dateField && !timeField) {
    return null;
  }
  const combined = [dateField ? record[dateField] : "", timeField ? record[timeField] : ""]
    .map((value) => (value ?? "").trim())
    .filter(Boolean)
    .join(" ");
  if (!combined) {
    return null;
  }
  return parseTimestamp(combined);
};

const NUMBER_SENTINELS = new Set(["-9999", "9999", "-9999.0", "9999.0"]);

const parseNumber = (raw: string | undefined): number | null => {
  if (raw == null) return null;
  const trimmed = raw.trim();
  if (!trimmed) return null;
  if (NUMBER_SENTINELS.has(trimmed)) return null;
  const numeric = Number.parseFloat(trimmed);
  if (!Number.isFinite(numeric)) return null;
  if (Math.abs(numeric) >= 9999) return null;
  return numeric;
};

type WindField = { field: string; unit: "mph" | "ms"; height?: number };

const deriveHeightFromField = (field: string): number | undefined => {
  const match = field.match(/(\d{2,3})(?:m)?/);
  if (!match) {
    return undefined;
  }
  const value = Number.parseInt(match[1], 10);
  if (!Number.isFinite(value) || value <= 0) {
    return undefined;
  }
  return value;
};

const WIND_FIELD_EXCLUDE = [/^wd/, /^dir/, /direction/, /^temp/, /^deg/, /^press/, /^baro/, /^battery/, /^volt/, /^rain/, /^wvcheck/, /_std(dev)?$/];

const detectWindFields = (headers: string[]): WindField[] => {
  const fields: WindField[] = [];

  headers.forEach((header) => {
    if (WIND_FIELD_EXCLUDE.some((pattern) => pattern.test(header))) {
      return;
    }
    if (WIND_MPH_FIELDS.includes(header) || WIND_MS_FIELDS.includes(header)) {
      return;
    }

    if (!/(wind|ws|speed|avg|mean|gust|anem|^se\d+|^sw\d+|^ne\d+|^nw\d+)/.test(header)) {
      return;
    }

    if (/gust/.test(header) && !/(avg|mean)/.test(header)) {
      // Gust-only fields are often peak gusts; skip unless no alternatives.
      return;
    }

    const height = deriveHeightFromField(header);
    fields.push({ field: header, unit: "ms", height });
  });

  return fields;
};

const adjustToHubHeight = (sample: MetSample, hubHeightM: number) => {
  const referenceHeight = sample.heightM && sample.heightM > 0 ? sample.heightM : hubHeightM;
  if (!referenceHeight || referenceHeight <= 0 || !hubHeightM || hubHeightM <= 0) {
    return sample.windMS;
  }

  const ratio = hubHeightM / referenceHeight;
  return sample.windMS * Math.pow(ratio, DEFAULT_ALPHA);
};

export async function extractMetSamples(files: File[]): Promise<{ samples: MetSample[]; errors: string[] }> {
  const samples: MetSample[] = [];
  const missingFields = new Set<string>();
  let hasTimestamp = false;
  let hasWind = false;

  for (const file of files) {
    const text = await file.text();
    const normalised = normaliseDelimiter(text);
    const rows = parseCSV(normalised);
    if (!rows.length) {
      continue;
    }

    const header = rows[0].map((cell) => normaliseHeader(cell));
    const timestampField = pickField(header, TIMESTAMP_FIELDS);
    const windMphField = pickField(header, WIND_MPH_FIELDS);
    const windMsField = pickField(header, WIND_MS_FIELDS);
    const heightField = pickField(header, HEIGHT_FIELDS);
    const dateField = timestampField ? undefined : pickField(header, DATE_ONLY_FIELDS);
    const timeField = timestampField ? undefined : pickField(header, TIME_ONLY_FIELDS);
    const yearField = timestampField ? undefined : pickField(header, YEAR_FIELDS);
    const monthField = timestampField ? undefined : pickField(header, MONTH_FIELDS);
    const dayField = timestampField ? undefined : pickField(header, DAY_FIELDS);
    const hourField = pickField(header, HOUR_FIELDS);
    const minuteField = pickField(header, MINUTE_FIELDS);
    const secondField = pickField(header, SECOND_FIELDS);

    const detectedWindFields = detectWindFields(header);

    const windFields: WindField[] = [];
    if (windMphField) {
      windFields.push({ field: windMphField, unit: "mph" });
    }
    if (windMsField) {
      windFields.push({ field: windMsField, unit: "ms" });
    }
    detectedWindFields.forEach((field) => {
      if (!windFields.some((existing) => existing.field === field.field)) {
        windFields.push(field);
      }
    });

    windFields.sort((a, b) => {
      const score = (entry: WindField) => (entry.height ? 10 : 0) + (entry.unit === "mph" ? 1 : 0);
      return score(b) - score(a);
    });

    const records = csvToObjects(rows);
    for (const record of records) {
      let timestamp: Date | null = null;
      if (timestampField) {
        timestamp = parseTimestamp(record[timestampField] ?? "");
      }
      if (!timestamp && (dateField || timeField)) {
        timestamp = parseDateAndTimeFields(record, dateField, timeField);
      }
      if (!timestamp && yearField && monthField && dayField) {
        timestamp = parseDateParts(record, {
          year: yearField,
          month: monthField,
          day: dayField,
          hour: hourField,
          minute: minuteField,
          second: secondField,
        });
      }
      if (!timestamp) {
        continue;
      }

      hasTimestamp = true;

      let windMS: number | null = null;
      let derivedHeight: number | undefined;
      for (const field of windFields) {
        const numeric = parseNumber(record[field.field]);
        if (numeric == null) {
          continue;
        }
        windMS = field.unit === "mph" ? numeric / MS_TO_MPH : numeric;
        derivedHeight = field.height ?? derivedHeight;
        hasWind = true;
        break;
      }

      if (windMS == null || !Number.isFinite(windMS)) {
        continue;
      }

      let heightM: number | undefined;
      if (heightField) {
        const numericHeight = parseNumber(record[heightField]);
        if (numericHeight != null && numericHeight > 0) {
          heightM = numericHeight;
        }
      }

      if (heightM == null && derivedHeight) {
        heightM = derivedHeight;
      }

      samples.push({ timestamp, windMS, heightM });
    }
  }

  if (!hasTimestamp) missingFields.add("timestamp");
  if (!hasWind) missingFields.add("wind speed");

  const errors: string[] = [];
  if (missingFields.size) {
    errors.push(`Missing required fields: ${Array.from(missingFields).join(", ")}`);
  }

  if (!samples.length) {
    errors.push("No valid MET measurements were found in the uploaded files.");
  }

  return { samples, errors };
}

export function computeHeatmap(
  samples: MetSample[],
  thresholdMph: number,
  hubHeightM: number,
): HeatmapResponse {
  const totals = Array.from({ length: 24 }, () =>
    Array.from({ length: 12 }, () => ({ count: 0, exceed: 0 })),
  );

  for (const sample of samples) {
    const timestamp = sample.timestamp;
    if (!(timestamp instanceof Date) || Number.isNaN(timestamp.getTime())) {
      continue;
    }

    const hour = timestamp.getHours();
    const month = timestamp.getMonth();

    if (hour < 0 || hour > 23 || month < 0 || month > 11) {
      continue;
    }

    const adjustedMs = adjustToHubHeight(sample, hubHeightM);
    const windMph = adjustedMs * MS_TO_MPH;

    const cell = totals[hour][month];
    cell.count += 1;
    if (windMph > thresholdMph) {
      cell.exceed += 1;
    }
  }

  const matrix: Array<Array<number | null>> = totals.map((row) =>
    row.map((cell) => {
      if (cell.count < MIN_SAMPLE_COUNT) {
        return null;
      }
      const percentage = (cell.exceed / cell.count) * 100;
      return Math.round(percentage);
    }),
  );

  const values = matrix.flat().filter((value): value is number => value != null);
  const averageExceedance = values.length
    ? values.reduce((sum, value) => sum + value, 0) / values.length
    : 0;

  return {
    averageExceedance,
    hours: HOURS,
    months: MONTHS,
    matrix,
  };
}
