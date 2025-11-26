using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Model.GeneralDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelingTool.DAL.GeneralRepositories
{
    public class MediaRepository : IMediaRepository
    {
        private readonly string _connectionString;

        public MediaRepository(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.json");

            _connectionString = connectionString;
        }

        public async Task<List<MediaDto>> GetByProjectIdAsync(int projectId)
        {
            var mediaList = new List<MediaDto>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand(@"
                    SELECT 
                        m.media_id,
                        m.frame_number,
                        m.file_path,
                        -- Add width and height if they exist in your database
                        COALESCE(m.width, 1920) as width,
                        COALESCE(m.height, 1080) as height
                    FROM Media m
                    INNER JOIN Project_Media pm ON m.media_id = pm.media_id
                    WHERE pm.project_id = @ProjectId
                    ORDER BY m.frame_number", connection);

                command.Parameters.AddWithValue("@ProjectId", projectId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        mediaList.Add(new MediaDto
                        {
                            MediaId = reader.GetInt32(0),
                            FrameNumber = reader.GetInt32(1),
                            FilePath = reader.GetString(2),
                            Width = reader.GetInt32(3),
                            Height = reader.GetInt32(4)
                        });
                    }
                }
            }

            return mediaList;
        }

        public async Task<MediaDto?> GetByIdAsync(int mediaId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand(@"
                    SELECT 
                        media_id,
                        frame_number,
                        file_path,
                        COALESCE(width, 1920) as width,
                        COALESCE(height, 1080) as height
                    FROM Media
                    WHERE media_id = @MediaId", connection);

                command.Parameters.AddWithValue("@MediaId", mediaId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new MediaDto
                        {
                            MediaId = reader.GetInt32(0),
                            FrameNumber = reader.GetInt32(1),
                            FilePath = reader.GetString(2),
                            Width = reader.GetInt32(3),
                            Height = reader.GetInt32(4)
                        };
                    }
                }
            }

            return null;
        }

        public async Task<int> CreateAsync(CreateMediaRequest request)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand(@"
                    INSERT INTO Media (frame_number, file_path, width, height)
                    OUTPUT INSERTED.media_id
                    VALUES (@FrameNumber, @FilePath, @Width, @Height)", connection);

                command.Parameters.AddWithValue("@FrameNumber", request.FrameNumber);
                command.Parameters.AddWithValue("@FilePath", request.FilePath);
                command.Parameters.AddWithValue("@Width", request.Width);
                command.Parameters.AddWithValue("@Height", request.Height);

                var result = await command.ExecuteScalarAsync();
                if (result == null)
                    throw new Exception("Failed to create media - database returned null");

                var newId = (int)result;
                return newId;
            }
        }
    }
}
