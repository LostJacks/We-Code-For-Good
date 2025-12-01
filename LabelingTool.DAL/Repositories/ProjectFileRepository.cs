using LabelingTool.DAL.Interfaces;
using LabelingTool.Model;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelingTool.DAL.Repositories
{
    public class ProjectFileRepository : IProjectFileRepository
    {
        private readonly string _connectionString;

        public ProjectFileRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public async Task AddFileAsync(int projectId, string filePath, string fileType)
        {
            try
            {
                Console.WriteLine($"[REPO] Inserting: ProjectId={projectId}, FilePath={filePath}, FileType={fileType}");

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    string query = @"
                INSERT INTO ProjectFile (ProjectId, FilePath, FileType)
                VALUES (@projectId, @filePath, @fileType)
            ";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@projectId", projectId);
                        cmd.Parameters.AddWithValue("@filePath", filePath);
                        cmd.Parameters.AddWithValue("@fileType", fileType);

                        await cmd.ExecuteNonQueryAsync();
                        Console.WriteLine($"[REPO] ✅ Insert successful!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[REPO] ❌ Insert failed: {ex.Message}");
                Console.WriteLine($"[REPO] Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<List<ProjectFile>> GetFilesByProjectIdAsync(int projectId)
        {
            var files = new List<ProjectFile>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                string query = @"
                SELECT Id, ProjectId, FilePath, FileType 
                FROM ProjectFile 
                WHERE ProjectId = @projectId
            ";

                Console.WriteLine($"[GET] Querying files for ProjectId: {projectId}");

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@projectId", projectId);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        int rowCount = 0;
                        while (await reader.ReadAsync())
                        {
                            rowCount++;
                            Console.WriteLine($"[GET] Found file #{rowCount}: {reader["FilePath"]}");
                            files.Add(new ProjectFile
                            {
                                Id = (int)reader["Id"],
                                ProjectId = (int)reader["ProjectId"],
                                FilePath = reader["FilePath"].ToString(),
                                FileType = reader["FileType"].ToString()
                            });
                        }
                        Console.WriteLine($"[GET] Total rows found: {rowCount}");
                    }
                }
            }

            Console.WriteLine($"[GET] Returning {files.Count} files");
            return files;
        }
    }
}
