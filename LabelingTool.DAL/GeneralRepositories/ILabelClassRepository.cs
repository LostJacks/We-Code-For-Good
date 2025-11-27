using Model.GeneralDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelingTool.DAL.GeneralRepositories
{
    public interface ILabelClassRepository
    {
        Task<List<LabelClassDto>> GetAllAsync();
        Task<LabelClassDto?> GetByIdAsync(int labelClassId);
    }
}
