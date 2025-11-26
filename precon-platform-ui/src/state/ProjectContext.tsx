import React from "react";
import type Project from "../types/Project";
//import { loadProjects } from "../services/ProjectService";
import projectService from "../services/ProjectService";

type Ctx = {
  projects: Project[];
  selectedId: string | null;
  setSelectedId: (id: string) => void;
  selected: Project | null;            // ← add this
  loading: boolean;
  error: string | null;
};

const ProjectContext = React.createContext<Ctx | undefined>(undefined);

export function ProjectProvider({ children }: { children: React.ReactNode }) {
  const [projects, setProjects]   = React.useState<Project[]>([]);
  const [selectedId, setSelected] = React.useState<string | null>(() =>
    localStorage.getItem("ppui:selectedProjectId")
  );
  const [loading, setLoading]     = React.useState(true);
  const [error, setError]         = React.useState<string | null>(null);

  React.useEffect(() => {
    let on = true;
    (async () => {
      try {
        setLoading(true);
        //const list = await loadProjects();   // loads CSV
        //alert(list);
        const list = await projectService.getProjects();   // loads api
        if (!on) return;
        // Ensure list is an array before setting state
        const projectsList = Array.isArray(list) ? list : [];
        setProjects(projectsList);
        if (!selectedId && projectsList.length) {
          setSelected(projectsList[0].id);
          localStorage.setItem("ppui:selectedProjectId", projectsList[0].id);
        }
      } catch (e: any) {
        if (on) {
          setError(e?.message || "Failed to load projects.");
          setProjects([]); // Ensure projects is always an array even on error
        }
      } finally {
        if (on) setLoading(false);
      }
    })();
    return () => {
      on = false;
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []); // Only run once on mount

  const setSelectedId = (id: string) => {
    setSelected(id);
    localStorage.setItem("ppui:selectedProjectId", id);
  };

  const selected = React.useMemo(
    () => (Array.isArray(projects) && projects.length > 0) 
      ? projects.find(p => p.id === selectedId) || null 
      : null,
    [projects, selectedId]
  );

  return (
    <ProjectContext.Provider value={{ projects, selectedId, setSelectedId, selected, loading, error }}>
      {children}
    </ProjectContext.Provider>
  );
}

export function useProjects() {
  const ctx = React.useContext(ProjectContext);
  if (!ctx) throw new Error("useProjects must be used within <ProjectProvider />");
  return ctx;

}