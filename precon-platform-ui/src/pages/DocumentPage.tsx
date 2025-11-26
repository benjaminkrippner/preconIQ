import { FormEvent, useMemo, useState } from "react";
import { useProjects } from "../state/ProjectContext";
import useProjectMetaData from "../state/ProjectMetaDataContext";
import "../styles/DocumentPage.css";

type TabKey = "files" | "comparison" | "audit";

type ProjectDocument = {
  id: string;
  name: string;
  type: string;
  version: string;
  uploaded: string;
  owner: string;
};

type AuditFinding = {
  id: number;
  severity: "High" | "Medium" | "Low";
  type: string;
  summary: string;
  confidence: string;
  source: string;
};

type LensKey = "risk" | "scope" | "commercial";

type ComparisonDiff = {
  heading: string;
  detail: string;
};

type ComparisonOutput = {
  heading: string;
  diffs: ComparisonDiff[];
};

const DOCUMENTS: ProjectDocument[] = [
  {
    id: "contract_v1",
    name: "contract_v1.pdf",
    type: "PDF",
    version: "v1.0",
    uploaded: "9/25/2025",
    owner: "Legal",
  },
  {
    id: "specifications_v1",
    name: "specifications_v1.pdf",
    type: "PDF",
    version: "v1.0",
    uploaded: "9/25/2025",
    owner: "Engineering",
  },
  {
    id: "drawings_v1",
    name: "drawings_v1.dxf",
    type: "DXF",
    version: "v1.0",
    uploaded: "9/25/2025",
    owner: "Design",
  },
  {
    id: "osi_matrix",
    name: "osi_matrix.xlsx",
    type: "XLSX",
    version: "v1.0",
    uploaded: "9/25/2025",
    owner: "Preconstruction",
  },
  {
    id: "bop_v1",
    name: "bop_v1.pdf",
    type: "PDF",
    version: "v1.0",
    uploaded: "9/25/2025",
    owner: "Engineering",
  },
];

const AUDIT_FINDINGS: AuditFinding[] = [
  {
    id: 1,
    severity: "High",
    type: "Conflict",
    summary:
      "Contract shall install a vegetative barrier around substation, conflicting with standard civil assumptions.",
    confidence: "95%",
    source: "Site_Survey.pdf, Section 3.2",
  },
  {
    id: 2,
    severity: "Medium",
    type: "Clarification",
    summary:
      "Owner requires weekly progress reports formatted per OSI handbook; clarify reporting cadence with PMO.",
    confidence: "88%",
    source: "OSI_Addendum.pdf, Pg. 5",
  },
  {
    id: 3,
    severity: "High",
    type: "Gap",
    summary:
      "No contingency noted for access road stabilization despite geotech recommendation for wet season work.",
    confidence: "90%",
    source: "Geotech_Report.pdf, Section 4.1",
  },
  {
    id: 4,
    severity: "Low",
    type: "Change",
    summary: "Assumptions reference 34.5kV feeders but OSI document updates distribution to 13.8kV.",
    confidence: "82%",
    source: "Electrical_Basis.pdf, Section 2",
  },
  {
    id: 5,
    severity: "Medium",
    type: "Follow-up",
    summary:
      "Clarify responsibility for erosion control inspections cited in OSI appendix versus contractor SOP.",
    confidence: "85%",
    source: "OSI_Appendix_C.pdf, Pg. 12",
  },
];

const LENS_LABELS: Record<LensKey, { label: string; description: string }> = {
  risk: {
    label: "Risk",
    description: "Surface clauses that shift schedule or liability exposure.",
  },
  scope: {
    label: "Scope",
    description: "Compare deliverables and division of work packages.",
  },
  commercial: {
    label: "Commercial",
    description: "Track pricing adjustments and commercial obligations.",
  },
};

const COMPARISON_LIBRARY: Record<LensKey, ComparisonOutput> = {
  risk: {
    heading: "Risk deviations",
    diffs: [
      {
        heading: "Milestone liability",
        detail:
          "Document B introduces liquidated damages for turbine delivery beyond 15 days, absent from Document A.",
      },
      {
        heading: "Weather suspension",
        detail:
          "Document A allows weather suspension with 24-hour notice, whereas Document B requires owner approval.",
      },
      {
        heading: "Safety governance",
        detail:
          "Document B references the owner's 2025 safety playbook; ensure contractor procedures are crosswalked.",
      },
    ],
  },
  scope: {
    heading: "Scope deltas",
    diffs: [
      {
        heading: "Cable routing",
        detail:
          "Document B adds duct bank installation through substation yard; not included in Document A's civil scope.",
      },
      {
        heading: "Commissioning",
        detail:
          "Document A assigns MV commissioning to owner reps, while Document B shifts activity to EPC contractor.",
      },
      {
        heading: "Spare parts",
        detail:
          "Document B lists a 3% spare inverter kit requirement; confirm allowance in baseline BOM.",
      },
    ],
  },
  commercial: {
    heading: "Commercial variances",
    diffs: [
      {
        heading: "Payment terms",
        detail:
          "Document A retains Net 45 progress payments; Document B revises to Net 30 with 5% retainage holdback.",
      },
      {
        heading: "Escalation",
        detail:
          "Document B caps escalation at CPI +1.5%; Document A permits pass-through of steel index swings.",
      },
      {
        heading: "Warranty",
        detail:
          "Document B extends balance-of-plant warranty to 30 months; baseline assumption holds at 24 months.",
      },
    ],
  },
};

const TABS: Array<{ key: TabKey; label: string; description: string }> = [
  { key: "files", label: "Document Files", description: "Browse contract packages and revisions" },
  { key: "comparison", label: "Compare Documents", description: "Run side-by-side analysis" },
  { key: "audit", label: "Assumptions Audit", description: "Cross-check assumptions against OSI" },
];

export default function DocumentPage() {
  const { selected, selectedId } = useProjects();
  const { meta } = useProjectMetaData(selectedId);
  const [activeTab, setActiveTab] = useState<TabKey>("files");
  const [selectedDocA, setSelectedDocA] = useState(DOCUMENTS[0]?.id ?? "");
  const [selectedDocB, setSelectedDocB] = useState(DOCUMENTS[1]?.id ?? "");
  const [activeLens, setActiveLens] = useState<LensKey>("risk");
  const [comparisonOutput, setComparisonOutput] = useState<ComparisonOutput | null>(null);
  const [comparisonMessage, setComparisonMessage] = useState(
    "Select documents and run comparison to see results."
  );

  const projectName = selected?.opportunityName ?? "Mountain View Solar";
  const megawatts = meta?.megawatts;
  const capacity = megawatts != null ? `${megawatts} MW` : "200 MW";
  const projectLine = `${projectName} · ${capacity}`;
  const locationLine = useMemo(() => {
    const location = [meta?.city, meta?.state]
      .filter(Boolean)
      .join(", ");
    const stage = selected?.opportunityStageName ?? "Pre-Construction";
    return location ? `${location} · ${stage}` : stage;
  }, [meta?.city, meta?.state, selected?.opportunityStageName]);

  const metricCards = useMemo(
    () => [
      { label: "Total Documents", value: DOCUMENTS.length.toString() },
      { label: "Latest Version", value: "1.0" },
      { label: "Last Upload", value: "9/25/2025" },
    ],
    []
  );

  const handleCompare = (event: FormEvent) => {
    event.preventDefault();
    if (!selectedDocA || !selectedDocB) {
      setComparisonOutput(null);
      setComparisonMessage("Select two documents to run comparison.");
      return;
    }

    if (selectedDocA === selectedDocB) {
      setComparisonOutput(null);
      setComparisonMessage("Choose two different documents to compare.");
      return;
    }

    const output = COMPARISON_LIBRARY[activeLens];
    setComparisonOutput(output);
    setComparisonMessage(output.heading);
  };

  return (
    <div className="document-page">

      <section className="dashboard-card document-hero">
        <header className="document-hero__header">
          <div className="document-hero__project">
            <span className="document-hero__eyebrow">Project</span>
            <h2 className="document-hero__title">{projectLine}</h2>
            <p className="document-hero__meta">{locationLine}</p>
          </div>
          <div className="document-hero__summary">
            <span className="document-hero__eyebrow">Document Intelligence</span>
            <p className="document-hero__text">
              Contract analysis and document comparison for {projectName}.
            </p>
          </div>
        </header>
        <div className="document-metrics" role="list">
          {metricCards.map((metric) => (
            <div key={metric.label} className="document-metric" role="listitem">
              <span className="document-metric__label">{metric.label}</span>
              <span className="document-metric__value">{metric.value}</span>
            </div>
          ))}
        </div>
      </section>

      <nav className="document-tabs" role="tablist" aria-label="Document intelligence views">
        {TABS.map((tab) => (
          <button
            key={tab.key}
            type="button"
            role="tab"
            aria-selected={activeTab === tab.key}
            aria-controls={`document-tabpanel-${tab.key}`}
            className={activeTab === tab.key ? "document-tab is-active" : "document-tab"}
            onClick={() => {
              setActiveTab(tab.key);
              if (tab.key !== "comparison") {
                setComparisonOutput(null);
                setComparisonMessage("Select documents and run comparison to see results.");
              }
            }}
          >
            <span className="document-tab__label">{tab.label}</span>
            <span className="document-tab__description">{tab.description}</span>
          </button>
        ))}
      </nav>

      <div className="document-panels">
        {activeTab === "files" && (
          <section
            id="document-tabpanel-files"
            role="tabpanel"
            aria-labelledby="document-tab-files"
            className="dashboard-card document-panel"
          >
            <header className="document-panel__header">
              <div>
                <h3>Project Documents</h3>
                <p>All project documents with extracted metadata and version tracking.</p>
              </div>
              <button type="button" className="button button--ghost document-panel__action">
                Upload documents
              </button>
            </header>
            <div className="document-table__wrap">
              <table className="table document-table">
                <thead>
                  <tr>
                    <th scope="col">Document</th>
                    <th scope="col">Type</th>
                    <th scope="col">Version</th>
                    <th scope="col">Upload Date</th>
                    <th scope="col">Owner</th>
                    <th scope="col" className="document-table__actions-col">
                      Actions
                    </th>
                  </tr>
                </thead>
                <tbody>
                  {DOCUMENTS.map((doc) => (
                    <tr key={doc.id}>
                      <td data-label="Document">
                        <span className="document-name">{doc.name}</span>
                      </td>
                      <td data-label="Type">{doc.type}</td>
                      <td data-label="Version">{doc.version}</td>
                      <td data-label="Upload Date">{doc.uploaded}</td>
                      <td data-label="Owner">{doc.owner}</td>
                      <td data-label="Actions" className="document-table__actions">
                        <button type="button" className="document-link">
                          View
                        </button>
                        <button type="button" className="document-link">
                          Download
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </section>
        )}

        {activeTab === "comparison" && (
          <section
            id="document-tabpanel-comparison"
            role="tabpanel"
            aria-labelledby="document-tab-comparison"
            className="document-comparison"
          >
            <form className="dashboard-card document-comparison__form" onSubmit={handleCompare}>
              <header className="document-panel__header">
                <div>
                  <h3>Document Comparison</h3>
                  <p>Choose two documents and apply an analysis lens.</p>
                </div>
                <button type="submit" className="button button--primary">
                  Compare Documents
                </button>
              </header>
              <div className="document-form-grid">
                <label className="document-field">
                  <span className="document-field__label">Document A</span>
                  <select
                    value={selectedDocA}
                    onChange={(event) => setSelectedDocA(event.target.value)}
                    className="document-select"
                  >
                    {DOCUMENTS.map((doc) => (
                      <option key={doc.id} value={doc.id}>
                        {doc.name}
                      </option>
                    ))}
                  </select>
                </label>
                <label className="document-field">
                  <span className="document-field__label">Document B</span>
                  <select
                    value={selectedDocB}
                    onChange={(event) => setSelectedDocB(event.target.value)}
                    className="document-select"
                  >
                    {DOCUMENTS.map((doc) => (
                      <option key={doc.id} value={doc.id}>
                        {doc.name}
                      </option>
                    ))}
                  </select>
                </label>
              </div>
              <div className="document-lens">
                <span className="document-field__label">Analysis Lens</span>
                <div className="document-lens__choices" role="radiogroup" aria-label="Analysis lens">
                  {(Object.keys(LENS_LABELS) as LensKey[]).map((key) => (
                    <button
                      key={key}
                      type="button"
                      role="radio"
                      aria-checked={activeLens === key}
                      className={activeLens === key ? "lens-chip is-active" : "lens-chip"}
                      onClick={() => setActiveLens(key)}
                    >
                      <span className="lens-chip__label">{LENS_LABELS[key].label}</span>
                      <span className="lens-chip__description">{LENS_LABELS[key].description}</span>
                    </button>
                  ))}
                </div>
              </div>
            </form>

            <section className="dashboard-card document-comparison__panel" aria-live="polite">
              <header className="document-panel__header">
                <div>
                  <h3>Comparison Results</h3>
                  <p>Review variances aligned to the selected lens.</p>
                </div>
              </header>
              {comparisonOutput ? (
                <div className="document-result">
                  <h4>{comparisonMessage}</h4>
                  <ul className="document-result__list">
                    {comparisonOutput.diffs.map((diff) => (
                      <li key={diff.heading}>
                        <strong>{diff.heading}</strong>
                        <p>{diff.detail}</p>
                      </li>
                    ))}
                  </ul>
                </div>
              ) : (
                <div className="document-empty">{comparisonMessage}</div>
              )}
            </section>
          </section>
        )}

        {activeTab === "audit" && (
          <section
            id="document-tabpanel-audit"
            role="tabpanel"
            aria-labelledby="document-tab-audit"
            className="document-audit"
          >
            <div className="document-upload-grid">
              <article className="dashboard-card document-upload-card">
                <header className="document-panel__header">
                  <div>
                    <h3>Standard Assumptions</h3>
                    <p>Upload or select the standard assumptions document.</p>
                  </div>
                </header>
                <div className="document-dropzone">
                  <span className="document-dropzone__label">Upload assumptions file</span>
                  <button type="button" className="button button--ghost document-dropzone__button">
                    Select File
                  </button>
                </div>
              </article>

              <article className="dashboard-card document-upload-card">
                <header className="document-panel__header">
                  <div>
                    <h3>OSI Documents</h3>
                    <p>Upload one or more Owner-Supplied Information documents.</p>
                  </div>
                </header>
                <div className="document-dropzone">
                  <span className="document-dropzone__label">Upload OSI documents</span>
                  <button type="button" className="button button--ghost document-dropzone__button">
                    Select Files
                  </button>
                </div>
              </article>
            </div>

            <div className="document-audit__actions">
              <button type="button" className="button button--primary">
                Run Assumptions Audit
              </button>
            </div>

            <section className="dashboard-card document-findings">
              <header className="document-panel__header">
                <div>
                  <h3>Audit Findings</h3>
                  <p>5 findings identified. Click a row to view details.</p>
                </div>
              </header>
              <div className="document-table__wrap">
                <table className="table document-table document-table--findings">
                  <thead>
                    <tr>
                      <th scope="col">Severity</th>
                      <th scope="col">Type</th>
                      <th scope="col">Summary</th>
                      <th scope="col">Confidence</th>
                      <th scope="col">Source</th>
                    </tr>
                  </thead>
                  <tbody>
                    {AUDIT_FINDINGS.map((finding) => (
                      <tr key={finding.id}>
                        <td data-label="Severity">
                          <span className={`severity-pill severity-pill--${finding.severity.toLowerCase()}`}>
                            {finding.severity}
                          </span>
                        </td>
                        <td data-label="Type">{finding.type}</td>
                        <td data-label="Summary" className="document-summary-cell">
                          {finding.summary}
                        </td>
                        <td data-label="Confidence">{finding.confidence}</td>
                        <td data-label="Source">{finding.source}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </section>
          </section>
        )}
      </div>
    </div>
  );
}
