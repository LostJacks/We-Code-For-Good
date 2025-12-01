using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.GeneralDTOs
{
    public class MediaDto
    {
        public int MediaId { get; set; }
        public int ProjectId { get; set; }
        public int FrameNumber { get; set; }
        public required string FilePath { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class CreateMediaRequest
    {
        public int ProjectId { get; set; }
        public int FrameNumber { get; set; }
        public required string FilePath { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
