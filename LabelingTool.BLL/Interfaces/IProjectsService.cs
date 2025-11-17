using LabelingTool.Model;

namespace LabelingTool.BLL.Interfaces
{
    public interface IProjectsService
    {
        List<Project> GetAllProjects();
        Project GetProject(int id);
        int CreateProject(Project project, List<ProjectFile> files, List<string> labels);
    }
}
