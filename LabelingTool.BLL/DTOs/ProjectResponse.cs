using LabelingTool.Model;

namespace LabelingTool.BLL.DTOs
{
    public class ProjectResponse
    {
        public Project Project { get; set; }
        public List<ProjectFile> Files { get; set; }
        public List<ProjectLabel> Labels { get; set; }
    }
}
