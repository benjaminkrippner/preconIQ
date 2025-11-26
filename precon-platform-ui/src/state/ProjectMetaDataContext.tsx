import { useEffect, useState } from "react";
import ProjectMetaDataService from "../services/ProjectMetaDataService";
import type ProjectMetaData from "../types/ProjectMetaData";

const defaultError = "Failed to load project metadata.";

export default function useProjectMetaData(projectId: string | null) {
  const [meta, setMeta] = useState<ProjectMetaData | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchMeta = async (id: string) => {
    setLoading(true);
    setError(null);
    try {
      const data = await ProjectMetaDataService.getProjectMetaDataById(
        Number(id)
      );
      setMeta(data);
      return data;
    } catch (err: any) {
      const message = err?.message || defaultError;
      setMeta(null);
      setError(message);
      throw err;
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    let active = true;

    if (!projectId) {
      setMeta(null);
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
        const data = await ProjectMetaDataService.getProjectMetaDataById(
          Number(projectId)
        );
        if (active) setMeta(data);
      } catch (err: any) {
        if (active) {
          const message = err?.message || defaultError;
          setMeta(null);
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

  const refresh = () =>
    projectId ? fetchMeta(projectId) : Promise.resolve<ProjectMetaData | null>(null);

  return { meta, loading, error, refresh };
}