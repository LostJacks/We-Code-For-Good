using Model.BBDTOs;
using Model.ExportDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelingTool.BLL.ExportServices
{
    public interface IYoloExportService
    {
        Task<YoloExportDto> GenerateYoloExportAsync(int projectId);
        Task<byte[]> CreateYoloZipAsync(YoloExportDto export);
        string ConvertToYoloFormat(BoundingBoxDto box, int imageWidth, int imageHeight);
    }
}
