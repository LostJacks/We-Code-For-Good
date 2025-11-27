using LabelingTool.DAL.BBRepositories;
using Model.BBDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelingTool.BLL.BBServices
{
    public class BoundingBoxService : IBoundingBoxService
    {
        private readonly IBoundingBoxRepository _repository;

        public BoundingBoxService(IBoundingBoxRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<BoundingBoxDto>> GetByMediaIdAsync(int mediaId)
        {
            if (mediaId <= 0)
                throw new ArgumentException("Media ID must be positive", nameof(mediaId));

            return await _repository.GetByMediaIdAsync(mediaId);
        }

        public async Task<BoundingBoxDto> GetByIdAsync(int boundingBoxId)
        {
            if (boundingBoxId <= 0)
                throw new ArgumentException("Bounding box ID must be positive", nameof(boundingBoxId));

            return await _repository.GetByIdAsync(boundingBoxId);
        }

        public async Task<BoundingBoxDto> CreateAsync(CreateBoundingBoxRequest request)
        {
            // Validate coordinates
            if (request.X1 >= request.X2)
                throw new ArgumentException("X1 must be less than X2");

            if (request.Y1 >= request.Y2)
                throw new ArgumentException("Y1 must be less than Y2");

            var newId = await _repository.CreateAsync(request);
            return await _repository.GetByIdAsync(newId);
        }

        public async Task UpdateAsync(UpdateBoundingBoxRequest request)
        {
            // Validate coordinates
            if (request.X1 >= request.X2)
                throw new ArgumentException("X1 must be less than X2");

            if (request.Y1 >= request.Y2)
                throw new ArgumentException("Y1 must be less than Y2");

            await _repository.UpdateAsync(request);
        }

        public async Task DeleteAsync(int boundingBoxId)
        {
            if (boundingBoxId <= 0)
                throw new ArgumentException("Bounding box ID must be positive", nameof(boundingBoxId));

            await _repository.DeleteAsync(boundingBoxId);
        }
    }
}
