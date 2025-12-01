using LabelingTool.Model;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using LabelingTool.DAL.Interfaces;

namespace LabelingTool.DAL.Repositories
{
	public class ProjectRepository : IProjectsRepository
	{
		private readonly Database _database;

		public ProjectRepository(Database database)
		{
			_database = database;
		}

		public void AddFile(int projectId, ProjectFile file, int frameNumber)
		{
			using var connection = _database.CreateOpenConnection();
			using var transaction = connection.BeginTransaction();

			try
			{
				var sqlMedia = @"INSERT INTO [Media] ([ProjectId], [frame_number], [file_path], [width], [height])
                    OUTPUT INSERTED.[media_id]
                    VALUES (@projectId, @frame_number, @file_path, @width, @height)";
                using var mediaCommand = new SqlCommand(sqlMedia, connection, transaction);
				mediaCommand.Parameters.AddWithValue("@frame_number", frameNumber);
				mediaCommand.Parameters.AddWithValue("@file_path", file.FilePath);
				int mediaId = (int)mediaCommand.ExecuteScalar();
				var sqlProjectMedia = @"INSERT INTO Project_Media (project_id, media_id)
									VALUES (@project_id, @media_id)";
				using var projectMediaCommand = new SqlCommand(sqlProjectMedia, connection, transaction);
				projectMediaCommand.Parameters.AddWithValue("@project_id", projectId);
				projectMediaCommand.Parameters.AddWithValue("@media_id", mediaId);
				projectMediaCommand.ExecuteNonQuery();
				transaction.Commit();
			}
			catch
			{
				transaction.Rollback();
				throw;
			}

		}

		public void AddLabel(int projectId, string labelName)
		{
			try
			{
				using var connection = _database.CreateOpenConnection();

				var sql = @"INSERT INTO [Labelclass] ([name], [color])
                    VALUES (@name, @color)";

                using var command = new SqlCommand(sql, connection);
				command.Parameters.AddWithValue("@name", labelName);
				command.Parameters.AddWithValue("@color", "red");


				command.ExecuteNonQuery();
			}
			catch (SqlException ex)
			{
				throw new Exception("SQL error in AddLabel(): " + ex.Message, ex);
			}
			catch (Exception ex)
			{
				throw new Exception("Unexpected error in AddLabel(): " + ex.Message, ex);
			}
		}


		public int Create(Project project)
		{
			try
			{
				using var connection = _database.CreateOpenConnection();

				var sql = @"INSERT INTO [Project] ([UserId], [Title], [Description], [CreatedAt])
                    OUTPUT INSERTED.[Id]
                    VALUES (@UserId, @Title, @Description, @CreatedAt)";

                using var command = new SqlCommand(sql, connection);

				command.Parameters.AddWithValue("@UserId", project.UserId);
				command.Parameters.AddWithValue("@Title", project.Title);
				command.Parameters.AddWithValue("@Description", project.Description ?? "");
				command.Parameters.AddWithValue("@CreatedAt", project.CreatedAt);

				var result = command.ExecuteScalar();
				return (int)result;
			}
			catch (SqlException ex)
			{
				throw new Exception("SQL error in Create(): " + ex.Message, ex);
			}
			catch (Exception ex)
			{
				throw new Exception("Unexpected error in Create(): " + ex.Message, ex);
			}
		}


		public List<Project> GetAll()
		{
			var projects = new List<Project>();

			try
			{
				using var connection = _database.CreateOpenConnection();
				using var command = new SqlCommand(
                @"SELECT [Id], [UserId], [Title], [Description], [CreatedAt] FROM [Project]", connection);

                using var reader = command.ExecuteReader();

				while (reader.Read())
				{
					var project = new Project
					{
						Id = reader.GetInt32(0),
						UserId = reader.GetInt32(1),
						Title = reader.GetString(2),
						Description = reader.IsDBNull(3) ? "" : reader.GetString(3),
						CreatedAt = reader.GetDateTime(4)
					};

					projects.Add(project);
				}
			}
			catch  (SqlException ex)
			{
				throw new Exception("SQL error in GetAll(): " + ex.Message, ex);

			}
			catch (Exception ex)
			{
				throw new Exception("Unexpected error in GetAll(): " + ex.Message, ex);

			}
			return projects;

		}

		public Project GetById(int id)
		{
			try
			{
				using var connection = _database.CreateOpenConnection();

				var sql = @"SELECT [Id], [UserId], [Title], [Description], [CreatedAt]
                    FROM [Project]
                    WHERE [Id] = @id";

                using var command = new SqlCommand(sql, connection);
				command.Parameters.AddWithValue("@id", id);

				using var reader = command.ExecuteReader();

				if (reader.Read())
				{
					return new Project
					{
						Id = reader.GetInt32(0),
						UserId = reader.GetInt32(1),
						Title = reader.GetString(2),
						Description = reader.IsDBNull(3) ? "" : reader.GetString(3),
						CreatedAt = reader.GetDateTime(4)
					};
				}
				return null;
			}
			catch (SqlException ex)
			{
				throw new Exception("SQL error in GetById(): " + ex.Message, ex);
			}
			catch (Exception ex)
			{
				throw new Exception("Unexpected error in GetById(): " + ex.Message, ex);
			}
		}


		public List<Project> GetProjectsByUserId(int userId)
		{
			var projects = new List<Project>();

			try
			{
				using var connection = _database.CreateOpenConnection();

				var sql = @"SELECT [Id], [UserId], [Title], [Description], [CreatedAt]
                    FROM [Project]
                    WHERE [UserId] = @UserId";

                using var command = new SqlCommand(sql, connection);
				command.Parameters.AddWithValue("@UserId", userId);

				using var reader = command.ExecuteReader();

				while (reader.Read())
				{
					var project = new Project
					{
						Id = reader.GetInt32(0),
						UserId = reader.GetInt32(1),
						Title = reader.GetString(2),
						Description = reader.IsDBNull(3) ? "" : reader.GetString(3),
						CreatedAt = reader.GetDateTime(4)
					};

					projects.Add(project);
				}
			}
			catch (SqlException ex)
			{
				throw new Exception("SQL error in GetProjectsByUserId(): " + ex.Message, ex);
			}
			catch (Exception ex)
			{
				throw new Exception("Unexpected error in GetProjectsByUserId(): " + ex.Message, ex);
			}

			return projects;
		}


	}
}

