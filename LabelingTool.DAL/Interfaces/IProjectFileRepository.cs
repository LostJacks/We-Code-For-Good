using LabelingTool.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelingTool.DAL.Interfaces
{
    public interface IProjectFileRepository
    {
        Task AddFileAsync(int projectId,string filePath, string fileType);
        Task<List<ProjectFile>> GetFilesByProjectIdAsync(int projectId);
    }

}
