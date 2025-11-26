import { useEffect, useState } from "react";
import ProjectNormalsService from "../services/ProjectNormalsService";
import type ProjectNormals from "../types/ProjectNormals";

const defaultError = "Failed to load project normals.";

export default function useProjectNormals(projectId: string | null) {
  const [normals, setNormals] = useState<ProjectNormals[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchNormals = async (id: string) => {
    setLoading(true);
    setError(null);
    try {
      const data = /^\d+$/.test(id)
        ? await ProjectNormalsService.getProjectNormalsById(Number(id))
        : await ProjectNormalsService.getProjectNormalsByOppId(id);
      setNormals(data ?? []);
      return data;
    } catch (err: any) {
      const message = err?.message || defaultError;
      setNormals([]);
      setError(message);
      throw err;
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    let active = true;

    if (!projectId) {
      setNormals([]);
      setError(null);
      setLoading(false);
      return () => {
        active = false;
      };
    }

    setLoading(true);
    setError(null);

    (async () => {
      try {
        const data = /^\d+$/.test(projectId)
          ? await ProjectNormalsService.getProjectNormalsById(Number(projectId))
          : await ProjectNormalsService.getProjectNormalsByOppId(projectId);
        if (active) setNormals(data ?? []);
      } catch (err: any) {
        if (active) {
          const message = err?.message || defaultError;
          setNormals([]);
          setError(message);
        }
      } finally {
        if (active) setLoading(false);
      }
    })();

    return () => {
      active = false;
    };
  }, [projectId]);

  const refresh = () => (projectId ? fetchNormals(projectId) : Promise.resolve<ProjectNormals[]>([]));

  return { normals, loading, error, refresh };
}
