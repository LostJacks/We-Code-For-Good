using LabelingTool.Model;
using Model.GeneralDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelingTool.DAL.GeneralRepositories
{
    public interface IProjectRepository
    {
        Task<ProjectDto?> GetByIdAsync(int projectId);
        Task<int> CreateAsync(CreateProjectRequest request);
        void AddLabel(int projectId, string labelName);

    }
}
