import http from "../http-common";
import type ProjectWindData from "../types/ProjectWindData";

class ProjectWindDataService {
  async getProjectWindDataById(id: number): Promise<ProjectWindData[]> {
    const res = await http.get<ProjectWindData[]>("/projectWindData/projectWindDataById", {
      params: { id },
    });
    return res.data;
  }

  async getProjectWindDataByProjectId(id: number): Promise<ProjectWindData[]> {
    const res = await http.get<ProjectWindData[]>(
      "/projectWindData/projectWindDataByProjectId",
      {
        params: { id },
      }
    );
    return res.data;
  }

  async getProjectWindDataByOppId(oppId: string): Promise<ProjectWindData[]> {
    const res = await http.get<ProjectWindData[]>(
      "/projectWindData/projectWindDataByOppId",
      {
        params: { oppId },
      }
    );
    return res.data;
  }
}

export default new ProjectWindDataService();
