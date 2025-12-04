using LabelingTool.BLL.Interfaces;
using LabelingTool.DAL.Interfaces;
using LabelingTool.Model;

namespace LabelingTool.BLL.Services
{
    public class ProjectsService : IProjectsService
    {
        private readonly IProjectsRepository _repo;

        public ProjectsService(IProjectsRepository repo)
        {
            _repo = repo;
        }

        public List<Project> GetAllProjects()
        {
            return _repo.GetAll();
        }

        public Project GetProject(int id)
        {
            return _repo.GetById(id);
        }

        public int CreateProject(Project project, List<ProjectFile> files, List<string> labels)
        {
            try
            {
                Console.WriteLine($"[SERVICE] CreateProject called with {labels?.Count ?? 0} labels");  // ✅ ADD
                if (labels != null)
                {
                    foreach (var lbl in labels)
                        Console.WriteLine($"[SERVICE]   - Label: '{lbl}'");  // ✅ ADD
                }

                int projectId = _repo.Create(project);

                for (int i = 0; i < files.Count; i++)
                {
                    _repo.AddFile(projectId, files[i], i + 1);
                }

                foreach (var label in labels)
                {
                    Console.WriteLine($"[SERVICE] Calling AddLabel for: {label}");  // ✅ ADD
                    _repo.AddLabel(projectId, label);
                }

                return projectId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SERVICE] ❌ Error: {ex}");  // ✅ ADD
                throw new Exception("Error in CreateProject(): " + ex.Message, ex);
            }
        }


    }
}
