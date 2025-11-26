import React from "react";
import { Link } from "react-router-dom";
import { useProjects } from "../../state/ProjectContext";
import useProjectMetaData from "../../state/ProjectMetaDataContext";
import WeatherWidget from "./widgets/WeatherWidget";

const fmtUSD = (n?: number) =>
  n == null
    ? "—"
    : new Intl.NumberFormat("en-US", {
        style: "currency",
        currency: "USD",
        maximumFractionDigits: 0,
      }).format(n);

export default function DashboardPage() {
  const { selected, selectedId } = useProjects();
  const { meta } = useProjectMetaData(selectedId);

  const marketSegment = meta?.marketSegment;
  const megawatts = meta?.megawatts;
  const revenue = meta?.revenue;
  const completion =
    meta?.projectCompletionDate;
  const stage = selected?.opportunityStageName;
  const location =
    [meta?.city, meta?.state]
    .filter(Boolean)
    .join(", ") || "—";

  return (
    <div className="dashboard-page">

      <section className="page-hero">
        <p className="hero-subtitle">
          Overview of all preconstruction optimization modules for {location} - {stage}
        </p>
      </section>

      <section className="stat-grid" aria-label="Key project indicators">
        <article className="stat-card stat-card--primary">
          <div className="stat-label">Market Segment</div>
          <div className="stat-value">{marketSegment}</div>
        </article>

        <article className="stat-card stat-card--warning">
          <div className="stat-label">Planned revenue</div>
          <div className="stat-value">{fmtUSD(revenue)}</div>
        </article>

        <article className="stat-card stat-card--success">
          <div className="stat-label">Capacity</div>
          <div className="stat-value">{megawatts} MW</div>
        </article>

        <article className="stat-card stat-card--info">
          <div className="stat-label">Completion target</div>
          <div className="stat-value">{completion}</div>
        </article>
      </section>

      <section className="module-grid">
        <Link to="/weather" className="module-card module-card--link">
          <div className="module-head">
            <div className="module-title-text">Weather analytics</div>
            <div className="module-subtitle">Wind patterns and construction conditions</div>
          </div>

          <div className="module-body module-body--compact">
            <WeatherWidget/>
          </div>
        </Link>

        <Link to="/design" className="module-card module-card--link">
          <div className="module-head">
            <div className="module-title-text">Engineering design</div>
            <div className="module-subtitle">Geotechnical analysis and foundation optimization</div>
          </div>

          <div className="module-body module-body--compact">
            <div className="module-row">
              <span className="module-label">Soil risk level</span>
              <span className="module-value module-value--success">Low</span>
            </div>
            <div className="module-row">
              <span className="module-label">Risk score</span>
              <span className="module-value">0.31</span>
            </div>
            <div className="module-row module-row--full">
              <span className="module-chip module-chip--amber">Implement drainage solutions</span>
            </div>
          </div>
        </Link>

        <Link to="/document" className="module-card module-card--link">
          <div className="module-head">
            <div className="module-title-text">Document intelligence</div>
            <div className="module-subtitle">Contract analysis and document comparison</div>
          </div>

          <div className="module-body module-body--compact">
            <div className="module-row">
              <span className="module-label">Documents processed</span>
              <span className="module-value">5</span>
            </div>
            <div className="module-row">
              <span className="module-label">Latest upload</span>
              <span className="module-value">9/25/2025</span>
            </div>
          </div>
        </Link>

        <Link to="/estimating" className="module-card module-card--link">
          <div className="module-head">
            <div className="module-title-text">Estimating optimization</div>
            <div className="module-subtitle">Cost modeling and cashflow planning</div>
          </div>

          <div className="module-body module-body--compact">
            <div className="module-row">
              <span className="module-label">Latest estimate</span>
              <span className="module-value">$283.5M</span>
            </div>
            <div className="module-row">
              <span className="module-label">Estimates count</span>
              <span className="module-value">5</span>
            </div>
            <div className="module-row">
              <span className="module-label">Last update</span>
              <span className="module-value">9/24/2025</span>
            </div>
          </div>
        </Link>
      </section>
    </div>
  );
}