using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ExportDTOs
{
    public class YoloExportDto
    {
        public int ProjectId { get; set; }
        public required string ProjectName { get; set; }
        public required List<YoloImageDto> Images { get; set; }
        public required List<string> Classes { get; set; }
    }

    public class YoloImageDto
    {
        public int MediaId { get; set; }
        public required string ImagePath { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public required List<YoloBoxDto> Boxes { get; set; }
    }

    public class YoloBoxDto
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public float XCenter { get; set; }
        public float YCenter { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
    }
}
