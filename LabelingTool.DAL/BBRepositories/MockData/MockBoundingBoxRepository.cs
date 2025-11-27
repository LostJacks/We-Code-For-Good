using LabelingTool.DAL.Interfaces;
using Model.BBDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelingTool.DAL.BBRepositories.MockData
{
    public class MockBoundingBoxRepository : IBoundingBoxRepository
    {
        private readonly List<BoundingBoxDto> _boundingBoxes = new List<BoundingBoxDto>();
        private int _nextId = 1;

        public MockBoundingBoxRepository()
        {
            // Seed test data
            _boundingBoxes.Add(new BoundingBoxDto
            {
                BoundingBoxId = _nextId++,
                MediaId = 1,
                LabelClassId = 0,
                ClassName = "person",
                X1 = 100.0f,
                Y1 = 150.0f,
                X2 = 400.0f,
                Y2 = 600.0f
            });

            _boundingBoxes.Add(new BoundingBoxDto
            {
                BoundingBoxId = _nextId++,
                MediaId = 1,
                LabelClassId = 1,
                ClassName = "car",
                X1 = 800.0f,
                Y1 = 300.0f,
                X2 = 1200.0f,
                Y2 = 650.0f
            });

            _boundingBoxes.Add(new BoundingBoxDto
            {
                BoundingBoxId = _nextId++,
                MediaId = 2,
                LabelClassId = 0,
                ClassName = "person",
                X1 = 150.0f,
                Y1 = 200.0f,
                X2 = 500.0f,
                Y2 = 700.0f
            });

            _boundingBoxes.Add(new BoundingBoxDto
            {
                BoundingBoxId = _nextId++,
                MediaId = 2,
                LabelClassId = 2,
                ClassName = "dog",
                X1 = 1000.0f,
                Y1 = 400.0f,
                X2 = 1300.0f,
                Y2 = 850.0f
            });

            _boundingBoxes.Add(new BoundingBoxDto
            {
                BoundingBoxId = _nextId++,
                MediaId = 3,
                LabelClassId = 1,
                ClassName = "car",
                X1 = 500.0f,
                Y1 = 250.0f,
                X2 = 1100.0f,
                Y2 = 600.0f
            });
        }

        public Task<List<BoundingBoxDto>> GetByMediaIdAsync(int mediaId)
        {
            var boxes = _boundingBoxes
                .Where(b => b.MediaId == mediaId)
                .ToList();

            return Task.FromResult(boxes);
        }

        public Task<BoundingBoxDto?> GetByIdAsync(int boundingBoxId)
        {
            var box = _boundingBoxes.FirstOrDefault(b => b.BoundingBoxId == boundingBoxId);
            return Task.FromResult(box);
        }

        public Task<int> CreateAsync(CreateBoundingBoxRequest request)
        {
            var newBox = new BoundingBoxDto
            {
                BoundingBoxId = _nextId++,
                MediaId = request.MediaId,
                LabelClassId = request.LabelClassId,
                ClassName = $"Class_{request.LabelClassId}",
                X1 = request.X1,
                Y1 = request.Y1,
                X2 = request.X2,
                Y2 = request.Y2
            };

            _boundingBoxes.Add(newBox);

            Console.WriteLine($"[MOCK] Created bounding box with ID: {newBox.BoundingBoxId}");

            return Task.FromResult(newBox.BoundingBoxId);
        }

        public Task UpdateAsync(UpdateBoundingBoxRequest request)
        {
            var existingBox = _boundingBoxes.FirstOrDefault(b => b.BoundingBoxId == request.BoundingBoxId);

            if (existingBox != null)
            {
                existingBox.LabelClassId = request.LabelClassId;
                existingBox.X1 = request.X1;
                existingBox.Y1 = request.Y1;
                existingBox.X2 = request.X2;
                existingBox.Y2 = request.Y2;

                Console.WriteLine($"[MOCK] Updated bounding box ID: {request.BoundingBoxId}");
            }

            return Task.CompletedTask;
        }

        public Task DeleteAsync(int boundingBoxId)
        {
            var box = _boundingBoxes.FirstOrDefault(b => b.BoundingBoxId == boundingBoxId);

            if (box != null)
            {
                _boundingBoxes.Remove(box);
                Console.WriteLine($"[MOCK] Deleted bounding box ID: {boundingBoxId}");
            }

            return Task.CompletedTask;
        }
    }
}
