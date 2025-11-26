import { useEffect, useMemo, useState } from "react";
import { fetchBasisOfDesign, type WindBoDFlat } from "./BasisOfDesignServices";
import "./BasisOfDesignWidget.css";

export type ParamStatus =
  | "Blattner Assumed"
  | "AI Extracted"
  | "Verified Assumption"
  | "Missing";

interface FlatParamRow {
  key: keyof WindBoDFlat;
  displayValue: string | null;
  rawValue: WindBoDFlat[keyof WindBoDFlat];
  status: ParamStatus;
}

interface StatsSummary {
  total: number;
  known: number;   // AI Extracted + Verified Assumption
  assumed: number; // Blattner Assumed
  missing: number;
  pct: number;
}

// Helper: safely extract message from unknown error
function getErrorMessage(error: unknown): string {
  if (error instanceof Error) return error.message;
  if (typeof error === "string") return error;
  try {
    return JSON.stringify(error);
  } catch {
    return "Failed to load Basis of Design";
  }
}

// Helper: does the AI value exist in a meaningful way?
function hasAiValue(value: WindBoDFlat[keyof WindBoDFlat]): boolean {
  if (value === null || value === undefined) return false;
  if (typeof value === "string") return value.trim() !== "";
  return true;
}

// Helper: format value for display
function formatDisplayValue(
  value: WindBoDFlat[keyof WindBoDFlat]
): string | null {
  if (value === null || value === undefined) return null;

  if (typeof value === "string") return value;
  if (typeof value === "number" || typeof value === "boolean") {
    return String(value);
  }

  // Fallback for unexpected shapes
  try {
    return JSON.stringify(value);
  } catch {
    return String(value);
  }
}

export default function BasisOfDesignWidget() {
  const [data, setData] = useState<WindBoDFlat | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [feedback, setFeedback] = useState<
    Record<string, "up" | "down" | null>
  >({});

  // Typed by WindBoDFlat keys for safety
  const [userGuesses, setUserGuesses] = useState<
    Partial<Record<keyof WindBoDFlat, string>>
  >({});

  // Load BoD data from service
  useEffect(() => {
    let cancelled = false;

    const load = async () => {
      setLoading(true);
      setError(null);

      try {
        const bod = await fetchBasisOfDesign(); // TODO: pass projectId when available
        if (!cancelled) {
          setData(bod);
        }
      } catch (e) {
        if (!cancelled) {
          setError(getErrorMessage(e));
        }
      } finally {
        if (!cancelled) {
          setLoading(false);
        }
      }
    };

    load();

    return () => {
      cancelled = true;
    };
  }, []);

  const paramRows: FlatParamRow[] = useMemo(() => {
    if (!data) return [];

    return (Object.keys(data) as (keyof WindBoDFlat)[]).map((key) => {
      const keyString = String(key);
      const rawValue = data[key];
      const aiHasValue = hasAiValue(rawValue);
      const userGuess = userGuesses[key] ?? "";
      const displayValue = formatDisplayValue(rawValue);

      let status: ParamStatus;

      switch (true) {
        case feedback[keyString] === "up":
          status = "Verified Assumption";
          break;

        case feedback[keyString] === "down":
          status = "Missing";
          break;

        case typeof userGuess === "string" && userGuess.trim() !== "":
          status = "Blattner Assumed";
          break;

        case aiHasValue:
          status = "AI Extracted";
          break;

        default:
          status = "Missing";
          break;
      }

      return {
        key,
        rawValue,
        displayValue,
        status,
      };
    });
  }, [data, userGuesses, feedback]);


  // KPI stats derived from paramRows
  const stats: StatsSummary = useMemo(() => {
    const total = paramRows.length;
    if (total === 0) {
      return { total: 0, known: 0, assumed: 0, missing: 0, pct: 0 };
    }

    let known = 0;
    let assumed = 0;
    let missing = 0;

    for (const row of paramRows) {
      if (row.status === "AI Extracted" || row.status === "Verified Assumption") {
        known += 1;
      } else if (row.status === "Blattner Assumed") {
        assumed += 1;
      } else {
        missing += 1;
      }
    }

    const pct = Math.round(((known + assumed) / total) * 100);

    return { total, known, assumed, missing, pct };
  }, [paramRows]);

  if (loading) {
    return (
      <section className="dashboard-card" aria-busy="true">
        <header className="card-header">
          <div className="card-title">Basis of Design</div>
        </header>
        <div className="module-body">Loading Basis of Design…</div>
      </section>
    );
  }

  if (error || !data) {
    return (
      <section className="dashboard-card" aria-live="polite">
        <header className="card-header">
          <div className="card-title">Basis of Design</div>
        </header>
        <div className="module-body module-body--error">
          Unable to load Basis of Design.
          {error && <div className="module-error-detail">{error}</div>}
        </div>
      </section>
    );
  }

  return (
    <section className="dashboard-card">
      {/* HEADER */}
      <header className="card-header">
        <div>
          <div className="card-title">Basis of Design</div>
          <div className="card-subtitle">
            Flat view of all parameters detected in the BoD JSON.
          </div>
        </div>
        <div className="card-actions">
          <button
            type="button"
            className="dp-tab ghost"
            onClick={() => {
              // placeholder to avoid “dead” feeling button
              // eslint-disable-next-line no-console
              console.log("Run BoD Update clicked");
            }}
          >
            Run BoD Update
          </button>
        </div>
      </header>

      {/* KPI BAR */}
      <div className="kpi-bar">
        <div className="kpi-item">
          <div className="kpi-label">Completion</div>
          <div className="kpi-value kpi-value-large">{stats.pct}%</div>
          <div className="kpi-subtext">
            {stats.known + stats.assumed} of {stats.total} completed
          </div>
        </div>
        <div className="kpi-item">
          <div className="kpi-label">Known (AI / Verified)</div>
          <div className="kpi-value">{stats.known}</div>
        </div>
        <div className="kpi-item">
          <div className="kpi-label">Blattner Assumed</div>
          <div className="kpi-value">{stats.assumed}</div>
        </div>
        <div className="kpi-item">
          <div className="kpi-label">Missing</div>
          <div className="kpi-value kpi-value-missing">
            {stats.missing}
          </div>
        </div>
      </div>

      {/* PARAMETERS TABLE */}
      <div className="module-body">
        <h3 className="module-section-title">Design Parameters</h3>

        <div className="bod-table-scroll">
          <table
            className="table design-table"
            aria-label="Basis of Design parameters"
          >
            <thead>
              <tr>
                <th style={{ width: "22%" }}>Parameter (JSON Key)</th>
                <th style={{ width: "22%" }}>Verified Assumption</th>
                <th style={{ width: "22%" }}>AI Extracted Values</th>
                <th style={{ width: "12%" }}>Source</th>
                <th style={{ width: "12%" }}>Status</th>
                <th style={{ width: "12%" }}>Feedback</th>
              </tr>
            </thead>

            <tbody>
              {paramRows.map((row) => {
                const keyString = String(row.key);
                const guessValue = userGuesses[row.key] ?? "";

                let statusClass = "";
                switch (row.status) {
                  case "AI Extracted":
                    statusClass = "param-status--extracted";
                    break;
                  case "Blattner Assumed":
                    statusClass = "param-status--assumed";
                    break;
                  case "Verified Assumption":
                    statusClass = "param-status--verified";
                    break;
                  case "Missing":
                  default:
                    statusClass = "param-status--missing";
                    break;
                }

                const inputId = `bod-guess-${keyString}`;

                return (
                  <tr key={keyString}>
                    {/* Parameter name */}
                    <td className="param-label">
                      <label htmlFor={inputId}>{keyString}</label>
                    </td>

                    <td className="param-verified-assumption">
                      <input
                        id={inputId}
                        type="text"
                        className="param-verified-assumption-input"
                        value={guessValue}
                        onChange={(e) => {
                          const value = e.target.value;
                          setUserGuesses((prev) => ({
                            ...prev,
                            [row.key]: value,
                          }));
                        }}
                        placeholder={
                          row.status === "Missing"
                            ? "Enter guess…"
                            : "Override…"
                        }
                      />
                    </td>

                    {/* AI Extracted Values */}
                    <td className="param-value">
                      {row.displayValue ?? (
                        <span className="param-missing">—</span>
                      )}
                    </td>

                    {/* Source */}
                    <td className="param-source"></td>

                    {/* Status Pill */}
                    <td>
                      <div className={`param-status ${statusClass}`}>
                        {row.status}
                      </div>
                    </td>

                    {/* Feedback */}
                    <td className="param-feedback">
                      <div className="feedback-buttons">
                        <button
                          type="button"
                          className={`feedback-btn feedback-btn--up ${
                            feedback[keyString] === "up" ? "selected" : ""
                          }`}
                          onClick={() =>
                            setFeedback((prev) => ({
                              ...prev,
                              [keyString]: prev[keyString] === "up" ? null : "up",
                            }))
                          }
                          aria-label="Thumbs up"
                        >
                          👍
                        </button>

                        <button
                          type="button"
                          className={`feedback-btn feedback-btn--down ${
                            feedback[keyString] === "down" ? "selected" : ""
                          }`}
                          onClick={() =>
                            setFeedback((prev) => ({
                              ...prev,
                              [keyString]: prev[keyString] === "down" ? null : "down",
                            }))
                          }
                          aria-label="Thumbs down"
                        >
                          👎
                        </button>
                      </div>
                    </td>

                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>
      </div>
    </section>
  );
}
