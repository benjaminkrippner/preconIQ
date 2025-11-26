import http from "../http-common";
import ProjectMetaData from "../types/ProjectMetaData";

class ProjectMetaDataService {
  async getProjectMetaDataById(id: number): Promise<ProjectMetaData> {
    const res = await http.get<ProjectMetaData>(
      "/projectMetaData/projectMetaDataById",
      {
        params: { id },
      }
    );
    return res.data;
  }

  async getProjectMetaDataByOppId(oppId: string): Promise<ProjectMetaData> {
    const res = await http.get<ProjectMetaData>(
      "/projectMetaData/projectMetaDataByOppId",
      {
        params: { oppId },
      }
    );
    return res.data;
  }
}

const projectMetaDataService = new ProjectMetaDataService();
export default projectMetaDataService;