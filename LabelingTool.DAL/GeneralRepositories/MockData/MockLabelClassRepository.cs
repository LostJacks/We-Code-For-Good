using Model.GeneralDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelingTool.DAL.GeneralRepositories.MockData
{
    public class MockLabelClassRepository : ILabelClassRepository
    {
        private readonly List<LabelClassDto> _classes;

        public MockLabelClassRepository()
        {
            _classes = new List<LabelClassDto>
            {
                new LabelClassDto { LabelClassId = 0, Name = "person", Color = "FF0000" },
                new LabelClassDto { LabelClassId = 1, Name = "car", Color = "00FF00" },
                new LabelClassDto { LabelClassId = 2, Name = "dog", Color = "0000FF" },
                new LabelClassDto { LabelClassId = 3, Name = "cat", Color = "FFFF00" }
            };
        }

        public Task<List<LabelClassDto>> GetAllAsync()
        {
            return Task.FromResult(_classes);
        }

        public Task<LabelClassDto?> GetByIdAsync(int labelClassId)
        {
            var labelClass = _classes.Find(c => c.LabelClassId == labelClassId);
            return Task.FromResult(labelClass);
        }
    }
}
