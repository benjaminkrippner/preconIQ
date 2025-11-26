export type HeatmapResponse = {
  averageExceedance: number;
  hours: string[];
  months: string[];
  matrix: Array<Array<number | null>>;
};

export type MetSample = {
  timestamp: Date;
  windMS: number;
  heightM?: number;
};
