using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LabelingTool.DAL.Models
{
    public class ProjectFile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FilePath { get; set; } = string.Empty;

        [Required]
        public string FileType { get; set; } = "image"; // e.g., image, video

        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        public Project? Project { get; set; }
    }
}
