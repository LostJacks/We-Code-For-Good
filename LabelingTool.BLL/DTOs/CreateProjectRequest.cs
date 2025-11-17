namespace LabelingTool.BLL.DTOs
{
    public class CreateProjectRequest
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public List<string>? Labels { get; set; }
    }
}
