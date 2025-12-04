using LabelingTool.Model;
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
    public class ProjectRepository : IProjectRepository
    {
        private readonly string _connectionString;

        public ProjectRepository(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.json");

            _connectionString = connectionString;
        }

        public async Task<ProjectDto?> GetByIdAsync(int projectId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand(@"
                        SELECT
                            [Id],
                            [UserId],
                            [Title],
                            [Description],
                            [CreatedAt]
                        FROM [Project]
                        WHERE [Id] = @ProjectId", connection);

                    command.Parameters.AddWithValue("@ProjectId", projectId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new ProjectDto
                            {
                                ProjectId = reader.GetInt32(0),
                                UserId = reader.IsDBNull(1) ? (int?)null : reader.GetInt32(1),
                                Name = reader.GetString(2),
                                Description = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                CreatedDate = reader.GetDateTime(4)
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

        public async Task<int> CreateAsync(CreateProjectRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                    throw new ArgumentException("Project name cannot be empty");

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var command = new SqlCommand(@"
                        INSERT INTO [Project] ([UserId], [Title], [Description], [CreatedAt])
                        OUTPUT INSERTED.[Id]
                        VALUES (@UserId, @Title, @Description, @CreatedAt)", connection);

                    command.Parameters.AddWithValue("@UserId", DBNull.Value); // No user for now
                    command.Parameters.AddWithValue("@Title", request.Name);
                    command.Parameters.AddWithValue("@Description", request.Description ?? "");
                    command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                    var result = await command.ExecuteScalarAsync();
                    if (result == null)
                        throw new Exception("Failed to insert project - database returned null");

                    int projectId = (int)result;
                    Console.WriteLine($"[DB] Created project with ID: {projectId}");

                    return projectId;
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
        public void AddLabel(int projectId, string labelName)
        {
            try
            {
                Console.WriteLine($"[REPO] Adding label: ProjectId={projectId}, LabelName={labelName}");

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(
                        "INSERT INTO ProjectLabel (ProjectId, LabelName) VALUES (@ProjectId, @LabelName)",
                        connection);
                    command.Parameters.AddWithValue("@ProjectId", projectId);
                    command.Parameters.AddWithValue("@LabelName", labelName);
                    command.ExecuteNonQuery();

                    Console.WriteLine($"[REPO] ✅ Label saved successfully");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[REPO] ❌ Error in AddLabel(): {ex.Message}");
                throw new Exception("Error in AddLabel(): " + ex.Message, ex);
            }
        }
    }
}
