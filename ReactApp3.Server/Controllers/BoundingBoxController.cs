using LabelingTool.BLL.BBServices;
using LabelingTool.BLL.ExportServices;
using Microsoft.AspNetCore.Mvc;
using Model.BBDTOs;
using Model.ExportDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BoundingBoxController : ControllerBase
    {
        private readonly IBoundingBoxService _service;
        private readonly IYoloExportService _yoloExportService;

        public BoundingBoxController(IBoundingBoxService service, IYoloExportService yoloExportService)
        {
            _service = service;
            _yoloExportService = yoloExportService;
        }

        /// Get all bounding boxes for a specific media/image
        [HttpGet("media/{mediaId}")]
        public async Task<ActionResult<List<BoundingBoxDto>>> GetByMedia(int mediaId)
        {
            try
            {
                var boxes = await _service.GetByMediaIdAsync(mediaId);
                return Ok(boxes);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// Get a single bounding box by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<BoundingBoxDto>> GetById(int id)
        {
            try
            {
                var box = await _service.GetByIdAsync(id);

                if (box == null)
                    return NotFound($"Bounding box with ID {id} not found");

                return Ok(box);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// Create a new bounding box
        [HttpPost]
        public async Task<ActionResult<BoundingBoxDto>> Create([FromBody] CreateBoundingBoxRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var box = await _service.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = box.BoundingBoxId }, box);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// Update an existing bounding box
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBoundingBoxRequest request)
        {
            if (id != request.BoundingBoxId)
                return BadRequest("ID mismatch");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _service.UpdateAsync(request);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// Delete a bounding box
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("export/yolo/{projectId}")]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        public async Task<IActionResult> ExportYolo(int projectId)
        {
            try
            {
                if (projectId <= 0)
                    return BadRequest("Invalid project ID");

                // Generate YOLO export data
                var yoloExport = await _yoloExportService.GenerateYoloExportAsync(projectId);

                if (!yoloExport.Images.Any())
                    return BadRequest("No annotated images found in project");

                // Create ZIP file
                var zipBytes = await _yoloExportService.CreateYoloZipAsync(yoloExport);

                // Return as downloadable ZIP
                string filename = $"yolo_export_{yoloExport.ProjectName}_{DateTime.Now:yyyyMMdd_HHmmss}.zip";
                return File(zipBytes, "application/zip", filename);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"YOLO Export Error: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("export/yolo/{projectId}/preview")]
        [ProducesResponseType(typeof(YoloExportDto), 200)]
        public async Task<IActionResult> PreviewYoloExport(int projectId)
        {
            try
            {
                var yoloExport = await _yoloExportService.GenerateYoloExportAsync(projectId);
                return Ok(yoloExport);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
