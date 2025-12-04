using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.GeneralDTOs
{
    public class ProjectDto
    {
        public int ProjectId { get; set; }
        public int? UserId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string PreviewImage { get; set; } = "";
    }

    public class CreateProjectRequest
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public List<string> Labels { get; set; } = new();
    }
}
