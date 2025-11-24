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

		public void AddFile(int projectId, ProjectFile file)
		{
			using var connection = _database.CreateOpenConnection();
			using var transaction = connection.BeginTransaction();

			try
			{
				var sqlMedia = @"INSERT INTO Media (frame_number, file_path)
							OUTPUT INSERTED.media_id
							VALUES (@frame_number, @file_path)";
				using var mediaCommand = new SqlCommand(sqlMedia, connection, transaction);
				mediaCommand.Parameters.AddWithValue("@frame_number", 0); //TODO: set frame number properly
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

				var sql = @"INSERT INTO Labelclass (name, color)
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

				var sql = @"
            INSERT INTO Project (user_id, name, description, created_date)
            OUTPUT INSERTED.project_id
            VALUES (@userId, @name, @description, @created_date);";

				using var command = new SqlCommand(sql, connection);

				command.Parameters.AddWithValue("@userId", project.UserId);
				command.Parameters.AddWithValue("@name", project.Title);
				command.Parameters.AddWithValue("@description", project.Description ?? "");
				command.Parameters.AddWithValue("@created_date", project.CreatedAt);

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
				"SELECT project_id, user_id, name, description, created_date FROM Project", connection);

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

				var sql = @"
            SELECT project_id, user_id, name, description, created_date
            FROM Project
            WHERE project_id = @id";

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

				var sql = @"SELECT project_id, user_id, name, description, created_date
                    FROM Project
                    WHERE user_id = @UserId";

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

