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
    public class LabelClassRepository : ILabelClassRepository
    {
        private readonly string _connectionString;

        public LabelClassRepository(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.json");

            _connectionString = connectionString;
        }

        public async Task<List<LabelClassDto>> GetAllAsync()
        {
            var classes = new List<LabelClassDto>();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand(@"
                        SELECT
                            [labelclass_id],
                            [name],
                            [color]
                        FROM [Labelclass]
                        ORDER BY [labelclass_id]", connection);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            classes.Add(new LabelClassDto
                            {
                                LabelClassId = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Color = reader.GetString(2)
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("SQL error in GetAllAsync(): " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error in GetAllAsync(): " + ex.Message, ex);
            }

            return classes;
        }

        public async Task<LabelClassDto?> GetByIdAsync(int labelClassId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand(@"
                        SELECT
                            [labelclass_id],
                            [name],
                            [color]
                        FROM [Labelclass]
                        WHERE [labelclass_id] = @LabelClassId", connection);

                    command.Parameters.AddWithValue("@LabelClassId", labelClassId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new LabelClassDto
                            {
                                LabelClassId = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Color = reader.GetString(2)
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

        public async Task<List<LabelClassDto>> GetByProjectIdAsync(int projectId)
        {
            var classes = new List<LabelClassDto>();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand(@"
                SELECT DISTINCT
                    pl.[LabelName]
                FROM [ProjectLabel] pl
                WHERE pl.[ProjectId] = @ProjectId", connection);  // ✅ REMOVED ORDER BY

                    command.Parameters.AddWithValue("@ProjectId", projectId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        int id = 0;
                        while (await reader.ReadAsync())
                        {
                            classes.Add(new LabelClassDto
                            {
                                LabelClassId = ++id,
                                Name = reader.GetString(0),
                                Color = "#FF5733"  // Default color
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("SQL error in GetByProjectIdAsync(): " + ex.Message, ex);
            }

            return classes;
        }
    }
}
