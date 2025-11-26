import React from "react";
import { useProjects } from "../../state/ProjectContext";

type ProjectSelectorProps = {
  /** Variant controls appearance: 'page' (full card) or 'header' (compact top bar) */
  variant?: "page" | "header";
};

export default function ProjectSelector({ variant = "page" }: ProjectSelectorProps) {
  const { projects, selectedId, setSelectedId, loading, error } = useProjects();
  const sel = projects.find((p) => p.id === selectedId);

  const location = '';//[sel?.projectCity, sel?.projectState].filter(Boolean).join(", ");
  const stage = sel?.opportunityStageName;

  const Wrapper = variant === "page" ? "section" : "div";
  const labelText = variant === "page" ? "Active Project" : "Project:";

  return (
    <Wrapper
      className={[
        "project-selector",
        variant === "header" ? "project-selector--compact" : "",
      ]
        .filter(Boolean)
        .join(" ")}
      {...(variant === "page" ? { "aria-labelledby": "project-selector-heading" } : {})}
    >
      <div className="project-selector__main">
        <div>
          {variant === "page" ? (
            <p id="project-selector-heading" className="project-selector__label">
              {labelText}
            </p>
          ) : (
            <span className="project-selector__label project-selector__label--compact">
              {labelText}
            </span>
          )}

          <select
            value={selectedId ?? ""}
            onChange={(event) => setSelectedId(event.target.value)}
            disabled={loading || !!error}
            className="project-selector__control"
          >
            {projects.map((project) => (
              <option key={project.id} value={project.id}>
                {project.opportunityName}
              </option>
            ))}
          </select>
        </div>

        <div className="project-selector__meta">
          {location && (
            <span className="project-selector__pill">{location}</span>
          )}
          {stage && (
            <span className="project-selector__pill project-selector__pill--muted">
              {stage}
            </span>
          )}
        </div>
      </div>

      {variant === "page" && error && (
        <div className="project-selector__error">{error}</div>
      )}
    </Wrapper>
  );
}