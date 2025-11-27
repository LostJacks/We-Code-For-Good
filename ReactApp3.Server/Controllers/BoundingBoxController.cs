using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LabelingTool.BLL.BBServices;
using Model.BBDTOs;

namespace SmartServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BoundingBoxController : ControllerBase
    {
        private readonly IBoundingBoxService _service;

        public BoundingBoxController(IBoundingBoxService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all bounding boxes for a specific media/image
        /// </summary>
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

        /// <summary>
        /// Get a single bounding box by ID
        /// </summary>
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

        /// <summary>
        /// Create a new bounding box
        /// </summary>
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

        /// <summary>
        /// Update an existing bounding box
        /// </summary>
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

        /// <summary>
        /// Delete a bounding box
        /// </summary>
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
    }
}
