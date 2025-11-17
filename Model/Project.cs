using LabelingTool.Models;

namespace LabelingTool.Model
{
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<ProjectFile> Files { get; set; }
        public List<ProjectLabel> Labels { get; set; }
    }
}
