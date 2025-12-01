namespace LabelingTool.Model
{
    public class ProjectFile
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public required string FilePath { get; set; }
        public required string FileType { get; set; }
    }
}
