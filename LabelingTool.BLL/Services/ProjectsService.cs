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
				int projectId = _repo.Create(project);

				foreach (var file in files)
				{
					_repo.AddFile(projectId, file);
				}

				foreach (var label in labels)
				{
					_repo.AddLabel(projectId, label);
				}

				return projectId;
			}
			catch (Exception ex)
			{
				throw new Exception("Error in CreateProject(): " + ex.Message, ex);
			}
		}

	}
}
