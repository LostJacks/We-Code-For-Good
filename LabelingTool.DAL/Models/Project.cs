using System;
using System.Collections.Generic;

namespace LabelingTool.DAL.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? PreviewImage { get; set; }

        // Relationships
        public ICollection<ProjectFile> Files { get; set; } = new List<ProjectFile>();
        public ICollection<ProjectLabel> Labels { get; set; } = new List<ProjectLabel>();
    }
}
