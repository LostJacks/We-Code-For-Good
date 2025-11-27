using Model.GeneralDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelingTool.DAL.GeneralRepositories.MockData
{
    public class MockMediaRepository : IMediaRepository
    {
        private readonly List<MediaDto> _media;

        public MockMediaRepository()
        {
            _media = new List<MediaDto>
            {
                new MediaDto
                {
                    MediaId = 1,
                    FilePath = "frame_001.jpg",
                    Width = 1920,
                    Height = 1080,
                    FrameNumber = 1
                },
                new MediaDto
                {
                    MediaId = 2,
                    FilePath = "frame_002.jpg",
                    Width = 1920,
                    Height = 1080,
                    FrameNumber = 2
                },
                new MediaDto
                {
                    MediaId = 3,
                    FilePath = "frame_003.jpg",
                    Width = 1920,
                    Height = 1080,
                    FrameNumber = 3
                }
            };
        }

        public Task<List<MediaDto>> GetByProjectIdAsync(int projectId)
        {
            // For mock, return all media for project ID 1
            if (projectId == 1)
            {
                return Task.FromResult(_media);
            }
            return Task.FromResult(new List<MediaDto>());
        }

        public Task<MediaDto?> GetByIdAsync(int mediaId)
        {
            var media = _media.FirstOrDefault(m => m.MediaId == mediaId);
            return Task.FromResult(media);
        }

        public Task<int> CreateAsync(CreateMediaRequest request)
        {
            var newId = _media.Max(m => m.MediaId) + 1;
            var media = new MediaDto
            {
                MediaId = newId,
                FilePath = request.FilePath,
                Width = request.Width,
                Height = request.Height,
                FrameNumber = request.FrameNumber
            };
            _media.Add(media);
            return Task.FromResult(newId);
        }
    }
}
