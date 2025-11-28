using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using LabelingTool.BLL.Interfaces;
using LabelingTool.BLL.DTOs;        // ✅ for CreateProjectRequest
using LabelingTool.Model;

namespace Smart_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectsService _projectsService;

        public ProjectsController(IProjectsService projectsService)
        {
            _projectsService = projectsService;
        }

        // ============================================================
        // GET: api/projects
        // ============================================================
        [HttpGet]
        public ActionResult<List<Project>> GetAll()
        {
            try
            {
                var projects = _projectsService.GetAllProjects();
                return Ok(projects);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error while getting projects: {ex}");
                return StatusCode(500, $"Error while getting projects: {ex.Message}");
            }
        }

        // ============================================================
        // GET: api/projects/{id}
        // ============================================================
        [HttpGet("{id:int}")]
        public ActionResult<Project> GetById(int id)
        {
            try
            {
                var project = _projectsService.GetProject(id);

                if (project == null)
                    return NotFound();

                return Ok(project);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error while getting project: {ex}");
                return StatusCode(500, $"Error while getting project: {ex.Message}");
            }
        }

        // ============================================================
        // POST: api/projects
        // ============================================================
        [HttpPost]
        public ActionResult<int> Create([FromBody] CreateProjectRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Title))
                return BadRequest("Invalid project data.");

            try
            {
                var project = new Project
                {
                    Title = request.Title,
                    Description = request.Description,
                    CreatedAt = DateTime.UtcNow
                };

                // ✅ Pass labels from the request (List<string>)
                var newId = _projectsService.CreateProject(
                    project,
                    new List<ProjectFile>(),
                    request.Labels ?? new List<string>()
                );

                return Ok(newId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error while creating project: {ex}");
                return StatusCode(500, $"Error while creating project: {ex.Message}");
            }
        }

        // ============================================================
        // (Optional) POST: api/projects/upload
        // ============================================================
        /*
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] UploadRequest request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueName = $"{Guid.NewGuid()}{Path.GetExtension(request.File.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.File.CopyToAsync(stream);
                }

                var relativePath = $"/uploads/{uniqueName}";

                // ✅ Attach file to existing project
                if (request.ProjectId.HasValue)
                {
                    var project = _context.Projects.FirstOrDefault(p => p.Id == request.ProjectId.Value);
                    if (project != null)
                    {
                        var newFile = new ProjectFile
                        {
                            FilePath = relativePath,
                            FileType = Path.GetExtension(request.File.FileName)
                                .ToLower()
                                .Contains("mp4") ? "Video" : "Image",
                            ProjectId = project.Id
                        };

                        _context.ProjectFiles.Add(newFile);

                        if (string.IsNullOrEmpty(project.PreviewImage))
                            project.PreviewImage = relativePath;

                        await _context.SaveChangesAsync();
                    }
                }

                return Ok(new { imagePath = relativePath });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"File upload failed: {ex.Message}");
            }
        }
        */
    }
}
