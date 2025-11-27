using Model.BBDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelingTool.BLL.BBServices
{
    public interface IBoundingBoxService
    {
        Task<List<BoundingBoxDto>> GetByMediaIdAsync(int mediaId);
        Task<BoundingBoxDto> GetByIdAsync(int boundingBoxId);
        Task<BoundingBoxDto> CreateAsync(CreateBoundingBoxRequest request);
        Task UpdateAsync(UpdateBoundingBoxRequest request);
        Task DeleteAsync(int boundingBoxId);
    }
}
