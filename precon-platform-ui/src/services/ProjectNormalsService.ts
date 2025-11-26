import http from "../http-common";
import type ProjectNormals from "../types/ProjectNormals";

class ProjectNormalsService {
  async getProjectNormalsById(id: number): Promise<ProjectNormals[]> {
    const res = await http.get<ProjectNormals[]>(
      "/projectNormals/projectNormalsById",
      {
        params: { id },
      }
    );
    return res.data;
  }

  async getProjectNormalsByOppId(oppId: string): Promise<ProjectNormals[]> {
    const res = await http.get<ProjectNormals[]>(
      "/projectNormals/projectNormalsByOppId",
      {
        params: { oppId },
      }
    );
    return res.data;
  }
}

const projectNormalsService = new ProjectNormalsService();
export default projectNormalsService;
