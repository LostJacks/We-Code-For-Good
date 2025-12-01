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
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand(@"
                        SELECT
                            m.[media_id],
                            m.[ProjectId],
                            m.[frame_number],
                            m.[file_path],
                            COALESCE(m.[width], 1920) as width,
                            COALESCE(m.[height], 1080) as height
                        FROM [Media] m
                        WHERE m.[ProjectId] = @ProjectId
                        ORDER BY m.[frame_number]", connection);

                    command.Parameters.AddWithValue("@ProjectId", projectId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            mediaList.Add(new MediaDto
                            {
                                MediaId = reader.GetInt32(0),
                                ProjectId = reader.GetInt32(1),
                                FrameNumber = reader.GetInt32(2),
                                FilePath = reader.GetString(3),
                                Width = reader.GetInt32(4),
                                Height = reader.GetInt32(5)
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("SQL error in GetByProjectIdAsync(): " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error in GetByProjectIdAsync(): " + ex.Message, ex);
            }

            return mediaList;
        }

        public async Task<MediaDto?> GetByIdAsync(int mediaId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand(@"
                        SELECT
                            [media_id],
                            [ProjectId],
                            [frame_number],
                            [file_path],
                            COALESCE([width], 1920) as width,
                            COALESCE([height], 1080) as height
                        FROM [Media]
                        WHERE [media_id] = @MediaId", connection);

                    command.Parameters.AddWithValue("@MediaId", mediaId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new MediaDto
                            {
                                MediaId = reader.GetInt32(0),
                                ProjectId = reader.GetInt32(1),
                                FrameNumber = reader.GetInt32(2),
                                FilePath = reader.GetString(3),
                                Width = reader.GetInt32(4),
                                Height = reader.GetInt32(5)
                            };
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("SQL error in GetByIdAsync(): " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error in GetByIdAsync(): " + ex.Message, ex);
            }

            return null;
        }

        public async Task<int> CreateAsync(CreateMediaRequest request)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand(@"
                        INSERT INTO [Media] ([ProjectId], [frame_number], [file_path], [width], [height])
                        OUTPUT INSERTED.[media_id]
                        VALUES (@ProjectId, @FrameNumber, @FilePath, @Width, @Height)", connection);

                    command.Parameters.AddWithValue("@ProjectId", request.ProjectId);
                    command.Parameters.AddWithValue("@FrameNumber", request.FrameNumber);
                    command.Parameters.AddWithValue("@FilePath", request.FilePath);
                    command.Parameters.AddWithValue("@Width", request.Width);
                    command.Parameters.AddWithValue("@Height", request.Height);

                    var result = await command.ExecuteScalarAsync();
                    if (result == null)
                        throw new Exception("Failed to create media - database returned null");

                    var newId = (int)result;
                    Console.WriteLine($"[DB] Created media with ID: {newId}");

                    return newId;
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("SQL error in CreateAsync(): " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error in CreateAsync(): " + ex.Message, ex);
            }
        }
    }
}
