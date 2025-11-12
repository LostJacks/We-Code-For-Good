using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using LabelingTool.DAL;
using LabelingTool.DAL.Models;
using Smart_Server.Models; // For UploadRequest
using System.IO;
using System.Linq;

namespace Smart_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =============================
        // ✅ GET: api/projects
        // =============================
        [HttpGet]
        public IActionResult GetProjects()
        {
            var projects = _context.Projects
                .Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.Description,
                    p.CreatedAt,
                    PreviewImage = !string.IsNullOrEmpty(p.PreviewImage)
                        ? p.PreviewImage
                        : p.Files.Select(f => f.FilePath).FirstOrDefault()
                          ?? "/assets/default-placeholder.png",
                    Labels = p.Labels.Select(l => l.LabelName).ToList(),
                    Files = p.Files.Select(f => new { f.Id, f.FilePath, f.FileType })
                })
                .ToList();

            return Ok(projects);
        }

        // =============================
        // ✅ POST: api/projects
        // =============================
        [HttpPost]
        public IActionResult CreateProject([FromBody] Project project)
        {
            if (project == null || string.IsNullOrWhiteSpace(project.Title))
                return BadRequest("Invalid project data.");

            try
            {
                project.CreatedAt = DateTime.Now;

                // ✅ Extract labels if sent from frontend
                var incomingLabels = project.Labels?.Select(l => l.LabelName).ToList() ?? new List<string>();

                // Detach labels to avoid EF Core tracking issues
                project.Labels = null;

                // ✅ Save the project first
                _context.Projects.Add(project);
                _context.SaveChanges();

                // ✅ Save labels (if any)
                foreach (var labelName in incomingLabels)
                {
                    if (!string.IsNullOrWhiteSpace(labelName))
                    {
                        _context.ProjectLabels.Add(new ProjectLabel
                        {
                            LabelName = labelName,
                            ProjectId = project.Id
                        });
                    }
                }

                _context.SaveChanges();

                return Ok(project);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // =============================
        // ✅ POST: api/projects/upload
        // =============================
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] UploadRequest request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                // Create uploads directory if missing
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                Directory.CreateDirectory(uploadsFolder);

                // Generate unique file name
                var uniqueName = $"{Guid.NewGuid()}{Path.GetExtension(request.File.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueName);

                // Save file to disk
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

                        // Set as preview if project has none
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
    }
}
