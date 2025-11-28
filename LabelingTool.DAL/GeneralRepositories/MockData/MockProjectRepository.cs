using Model.GeneralDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelingTool.DAL.GeneralRepositories.MockData
{
    public class MockProjectRepository : IProjectRepository
    {
        private readonly List<ProjectDto> _projects;

        public MockProjectRepository()
        {
            _projects = new List<ProjectDto>
            {
                new ProjectDto
                {
                    ProjectId = 1,
                    Name = "Person Detection",
                    Description = "Detecting people in video frames",
                    UserId = 1,
                    CreatedDate = DateTime.Now.AddDays(-5)
                },
                new ProjectDto
                {
                    ProjectId = 2,
                    Name = "Traffic Analysis",
                    Description = "Detecting vehicles and pedestrians",
                    UserId = 1,
                    CreatedDate = DateTime.Now.AddDays(-10)
                }
            };
        }

        public Task<ProjectDto?> GetByIdAsync(int projectId)
        {
            var project = _projects.Find(p => p.ProjectId == projectId);
            return Task.FromResult(project);
        }

        public Task<int> CreateAsync(CreateProjectRequest request)
        {
            var newId = _projects.Count + 1;
            var project = new ProjectDto
            {
                ProjectId = newId,
                Name = request.Name,
                Description = request.Description,
                CreatedDate = DateTime.Now,
                UserId = 1
            };
            _projects.Add(project);
            return Task.FromResult(newId);
        }
    }
}
