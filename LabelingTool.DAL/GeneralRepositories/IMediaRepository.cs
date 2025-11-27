using Model.GeneralDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelingTool.DAL.GeneralRepositories
{
    public interface IMediaRepository
    {
        Task<List<MediaDto>> GetByProjectIdAsync(int projectId);
        Task<MediaDto?> GetByIdAsync(int mediaId);
        Task<int> CreateAsync(CreateMediaRequest request);
    }
}
