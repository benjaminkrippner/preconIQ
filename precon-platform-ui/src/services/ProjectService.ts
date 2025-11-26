import http from "../http-common";
import Project from "../types/Project";

class ProjectService {
  async getProjects(searchString = ""): Promise<Project[]> {
    const res = await http.get<Project[]>("/getProjects", {
      params: searchString ? { searchString } : undefined,
    });
    return res.data;
  }
}

const projectService = new ProjectService();
export default projectService;


