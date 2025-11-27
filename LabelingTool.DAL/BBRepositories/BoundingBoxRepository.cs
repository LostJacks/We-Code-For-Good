using Microsoft.Extensions.Configuration;
using Model.BBDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LabelingTool.DAL.BBRepositories
{
    public class BoundingBoxRepository : IBoundingBoxRepository
    {
        // In-memory mock DB
        private static readonly List<BoundingBoxDto> _mockBoxes = new();
        private static int _nextId = 1;

        // Simulate label class lookup
        private static readonly Dictionary<int, string> _mockLabelClasses = new()
        {
            { 1, "T-shirt" },
            { 2, "Blazer" },
            { 3, "Boots" },
            { 4, "Cardigan" },
            { 5, "Backpack" },
            { 6, "Gloves" }
        };

        private static readonly Dictionary<int, string> _mockColors = new()
        {
            { 1, "#e53935" }, // red
            { 2, "#2196f3" }, // blue
            { 3, "#4caf50" }, // green
            { 4, "#ff9800" }, // orange
            { 5, "#9c27b0" }, // purple
            { 6, "#009688" }  // teal
        };

        public BoundingBoxRepository(IConfiguration configuration)
        {
            Console.WriteLine("[Mock DB] BoundingBoxRepository initialized (no real DB)");
        }

        // =====================================================
        // GET ALL BOXES FOR A MEDIA ID
        // =====================================================
        public async Task<List<BoundingBoxDto>> GetByMediaIdAsync(int mediaId)
        {
            await Task.Delay(50); // simulate DB latency

            var boxes = _mockBoxes
                .Where(b => b.MediaId == mediaId)
                .Select(b => MapDto(b))
                .ToList();

            Console.WriteLine($"[Mock DB] Returned {boxes.Count} boxes for media {mediaId}");
            return boxes;
        }

        // =====================================================
        // GET ONE BOX BY ID
        // =====================================================
        public async Task<BoundingBoxDto> GetByIdAsync(int boundingBoxId)
        {
            await Task.Delay(30);
            var box = _mockBoxes.FirstOrDefault(b => b.BoundingBoxId == boundingBoxId);
            if (box == null)
            {
                Console.WriteLine($"[Mock DB] Box {boundingBoxId} not found");
                return null;
            }

            Console.WriteLine($"[Mock DB] Found box #{boundingBoxId} ({box.ClassName})");
            return MapDto(box);
        }

        // =====================================================
        // CREATE NEW BOX
        // =====================================================
        public async Task<int> CreateAsync(CreateBoundingBoxRequest request)
        {
            await Task.Delay(40);

            var newBox = new BoundingBoxDto
            {
                BoundingBoxId = _nextId++,
                MediaId = request.MediaId,
                LabelClassId = request.LabelClassId,
                ClassName = _mockLabelClasses.ContainsKey(request.LabelClassId)
                    ? _mockLabelClasses[request.LabelClassId]
                    : "Unlabeled",
                X1 = request.X1,
                Y1 = request.Y1,
                X2 = request.X2,
                Y2 = request.Y2
            };

            _mockBoxes.Add(newBox);
            Console.WriteLine($"[Mock DB] Created box #{newBox.BoundingBoxId} for Media {request.MediaId} ({newBox.ClassName})");
            return newBox.BoundingBoxId;
        }

        // =====================================================
        // UPDATE EXISTING BOX
        // =====================================================
        public async Task UpdateAsync(UpdateBoundingBoxRequest request)
        {
            await Task.Delay(40);
            var existing = _mockBoxes.FirstOrDefault(b => b.BoundingBoxId == request.BoundingBoxId);
            if (existing == null)
            {
                Console.WriteLine($"[Mock DB] Tried to update non-existent box {request.BoundingBoxId}");
                return;
            }

            existing.LabelClassId = request.LabelClassId;
            existing.ClassName = _mockLabelClasses.ContainsKey(request.LabelClassId)
                ? _mockLabelClasses[request.LabelClassId]
                : "Unlabeled";
            existing.X1 = request.X1;
            existing.Y1 = request.Y1;
            existing.X2 = request.X2;
            existing.Y2 = request.Y2;

            Console.WriteLine($"[Mock DB] Updated box #{request.BoundingBoxId} → {existing.ClassName}");
        }

        // =====================================================
        // DELETE BOX
        // =====================================================
        public async Task DeleteAsync(int boundingBoxId)
        {
            await Task.Delay(30);
            _mockBoxes.RemoveAll(b => b.BoundingBoxId == boundingBoxId);
            Console.WriteLine($"[Mock DB] Deleted box #{boundingBoxId}");
        }

        // =====================================================
        // MAP DTO — always return full data with name + color
        // =====================================================
        private BoundingBoxDto MapDto(BoundingBoxDto b)
        {
            return new BoundingBoxDto
            {
                BoundingBoxId = b.BoundingBoxId,
                MediaId = b.MediaId,
                LabelClassId = b.LabelClassId,
                ClassName = _mockLabelClasses.ContainsKey(b.LabelClassId)
                    ? _mockLabelClasses[b.LabelClassId]
                    : "Unlabeled",
                X1 = b.X1,
                Y1 = b.Y1,
                X2 = b.X2,
                Y2 = b.Y2
            };
        }
    }
}
