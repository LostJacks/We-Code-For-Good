namespace LabelingTool.DAL.Models
{
    public class ProjectLabel
    {
        public int Id { get; set; }
        public string LabelName { get; set; } = string.Empty;
        public int ProjectId { get; set; }
        public Project? Project { get; set; }
    }
}
