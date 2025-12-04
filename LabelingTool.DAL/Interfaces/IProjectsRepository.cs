using LabelingTool.Model;

namespace LabelingTool.DAL.Interfaces
{
    public interface IProjectsRepository
    {
        Project GetById(int id);
        List<Project> GetAll();
        int Create(Project project);
        void AddFile(int projectId, ProjectFile file, int frameNumber);
        void AddLabel(int projectId, string labelName);
        Task<List<Project>> GetAllAsync();
    }
}
