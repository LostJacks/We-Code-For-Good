using LabelingTool.BLL.DTOs;        // ✅ for CreateProjectRequest
using LabelingTool.BLL.ExportServices;
using LabelingTool.BLL.Interfaces;
using LabelingTool.BLL.Services;
using LabelingTool.DAL.GeneralRepositories;
using LabelingTool.DAL.Interfaces;
using LabelingTool.DAL.Repositories;
using LabelingTool.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.GeneralDTOs;
using SmartServer.DTOs;
using System;
using System.Collections.Generic;

namespace Smart_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IProjectsService _projectsService;
        private readonly IProjectFileRepository _projectFileRepository;
        private readonly IProjectsRepository _projectRepository;
        private readonly IMediaRepository _mediaRepository;
        private readonly IYoloExportService _yoloExportService;
        private readonly ILabelClassRepository _labelClassRepository;

        public ProjectsController(IWebHostEnvironment env, IProjectsService projectsService, IProjectFileRepository projectFileRepository, IMediaRepository mediaRepository, 
            IYoloExportService yoloExportService, ILabelClassRepository labelClassRepository, IProjectsRepository projectRepository)
        {
            _env = env;
            _projectsService = projectsService;
            _projectFileRepository = projectFileRepository;
            _mediaRepository = mediaRepository;
            _yoloExportService = yoloExportService;
            _labelClassRepository = labelClassRepository;
            _projectRepository = projectRepository;
        }

        // ============================================================
        // GET: api/projects
        // ============================================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetAll()
        {
            var projects = await _projectRepository.GetAllAsync();

            foreach (var project in projects)
            {
                var firstMedia = await _mediaRepository.GetByProjectIdAsync(project.Id);
                project.PreviewImage = firstMedia?.FirstOrDefault()?.FilePath ?? "";
            }

            return Ok(projects);
        }

        // ============================================================
        // GET: api/projects/{id}
        // ============================================================
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult> GetById(int id)
        {
            try
            {
                var project = _projectsService.GetProject(id);

                if (project == null)
                    return NotFound();

                // ✅ Get files for this project
                var projectFiles = await _projectFileRepository.GetFilesByProjectIdAsync(id);
                var labelClasses = await _labelClassRepository.GetByProjectIdAsync(id);

                Console.WriteLine($"[CTRL] Retrieved {projectFiles.Count} files");
                Console.WriteLine($"[CTRL] Retrieved {labelClasses.Count} label classes");

                return Ok(new
                {
                    projectId = project.Id,
                    title = project.Title,
                    description = project.Description,
                    createdAt = project.CreatedAt,
                    labelClasses = labelClasses.Select(lc => lc.Name).ToList(),
                    files = projectFiles.Select(f => new
                    {
                        id = f.Id,
                        filePath = f.FilePath,
                        fileType = f.FileType
                    })
                });
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
        [Authorize]
        public ActionResult<int> Create([FromBody] LabelingTool.BLL.DTOs.CreateProjectRequest request)
        {
            Console.WriteLine($"[CTRL] Create endpoint hit");  // ✅ ADD
            Console.WriteLine($"[CTRL] Request: {System.Text.Json.JsonSerializer.Serialize(request)}");  // ✅ ADD

            if (request == null || string.IsNullOrWhiteSpace(request.Title))
                return BadRequest("Invalid project data.");

            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                Console.WriteLine($"[DEBUG] UserID Claim Value: '{userIdClaim}'");

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "User ID not found in token. Please login." });
                }
                Console.WriteLine($"[DEBUG] Parsed userId: {userId}");

                var project = new Project
                {
                    UserId = userId,
                    Title = request.Title,
                    Description = request.Description,
                    CreatedAt = DateTime.UtcNow
                };

                Console.WriteLine($"[CTRL] Labels count: {request.Labels?.Count ?? 0}");  // ✅ ADD

                var newId = _projectsService.CreateProject(
                    project,
                    new List<ProjectFile>(),
                    request.Labels ?? new List<string>()
                );

                return Ok(new { projectId = newId, message = "Project created successfully" });
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
        [HttpPost("upload")]
        [Authorize]
        public async Task<IActionResult> UploadFile([FromForm] UploadRequest request)
        {
            try
            {
                Console.WriteLine($"[1] Upload endpoint hit");

                if (request.File == null || request.File.Length == 0)
                    return BadRequest("No file uploaded.");

                if (!request.ProjectId.HasValue)
                    return BadRequest("ProjectId is required.");

                Console.WriteLine($"[2] ProjectId: {request.ProjectId.Value}");

                var project = _projectsService.GetProject(request.ProjectId.Value);
                if (project == null)
                    return NotFound("Project not found.");

                Console.WriteLine($"[3] Project found");

                var projectFolder = Path.Combine(_env.WebRootPath, "uploads", request.ProjectId.Value.ToString());
                if (!Directory.Exists(projectFolder))
                    Directory.CreateDirectory(projectFolder);

                var uniqueName = $"{Guid.NewGuid()}{Path.GetExtension(request.File.FileName)}";
                var filePath = Path.Combine(projectFolder, uniqueName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.File.CopyToAsync(stream);
                }

                Console.WriteLine($"[4] File saved to disk: {filePath}");

                var relativePath = $"/uploads/{request.ProjectId.Value}/{uniqueName}";
                string fileType = Path.GetExtension(request.File.FileName).ToLower().Contains("mp4") ? "Video" : "Image";

                Console.WriteLine($"[5] About to call repository.AddFileAsync");
                Console.WriteLine($"[5a] ProjectId={request.ProjectId.Value}, FilePath={relativePath}, FileType={fileType}");

                await _projectFileRepository.AddFileAsync(
                    request.ProjectId.Value,
                    relativePath,
                    fileType
                );

                Console.WriteLine($"[5.5] ProjectFile created, now creating Media record");

                // ✅ NEW: Create Media record using your existing repo
                int mediaId = await _mediaRepository.CreateAsync(new CreateMediaRequest
                {
                    ProjectId = request.ProjectId.Value,
                    FrameNumber = 0,  // Default frame number
                    FilePath = relativePath,
                    Width = 1920,     // Default width
                    Height = 1080     // Default height
                });

                Console.WriteLine($"[6] ✅ Media created with ID: {mediaId}");

                return Ok(new
                {
                    filePath = relativePath,
                    mediaId = mediaId,
                    message = "File uploaded successfully"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR: {ex.Message}");
                Console.WriteLine($"❌ STACK: {ex.StackTrace}");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{projectId:int}/export")]
        [Authorize]
        public async Task<IActionResult> ExportYolo(int projectId)
        {
            try
            {
                var project = _projectsService.GetProject(projectId);
                if (project == null)
                    return NotFound("Project not found");

                // ✅ Generate YOLO export data
                var yoloExport = await _yoloExportService.GenerateYoloExportAsync(projectId);

                // ✅ Create ZIP file
                var zipBytes = await _yoloExportService.CreateYoloZipAsync(yoloExport);

                return File(zipBytes, "application/zip", $"project-{projectId}-export.zip");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Export error: {ex.Message}");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
