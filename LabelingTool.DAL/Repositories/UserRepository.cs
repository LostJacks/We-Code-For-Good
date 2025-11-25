using System;
using System.Data;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using LabelingTool.Model;
using LabelingTool.DAL.Interfaces;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace LabelingTool.DAL.Repositories
{
	public class UserRepository : IUserRepository
	{
		private readonly Database _database;

		public UserRepository(Database database)
		{
			_database = database;
		}

		public AppUser? GetById(int id)
		{
			try
			{
				using var connection = _database.CreateOpenConnection();
				var sql = @"SELECT user_id, name, email, password 
                            FROM Users
                            WHERE user_id = @id";

				using var command = new SqlCommand(sql, connection);
				command.Parameters.AddWithValue("@id", id);

				using var reader = command.ExecuteReader();

				if (reader.Read())
				{
					return new AppUser
					{
						Id = reader.GetInt32(0),
						Name = reader.GetString(1),
						Email = reader.GetString(2),
						PasswordHash = reader.GetString(3)
					};
				}

				return null;
			}
			catch (SqlException ex)
			{
				throw new Exception("SQL error in UserRepository.GetById(): " + ex.Message, ex);
			}
			catch (Exception ex)
			{
				throw new Exception("Unexpected error in UserRepository.GetById(): " + ex.Message, ex);
			}
		}

		public AppUser? GetByEmail(string email)
		{
			try
			{
				using var connection = _database.CreateOpenConnection();
				var sql = @"SELECT user_id, name, email, password 
                            FROM Users
                            WHERE email = @Email";

				using var command = new SqlCommand(sql, connection);
				command.Parameters.AddWithValue("@Email", email);

				using var reader = command.ExecuteReader();

				if (reader.Read())
				{
					return new AppUser
					{
						Id = reader.GetInt32(0),
						Name = reader.GetString(1),
						Email = reader.GetString(2),
						PasswordHash = reader.GetString(3)
					};
				}

				return null;
			}
			catch (SqlException ex)
			{
				throw new Exception("SQL error in UserRepository.GetByEmail(): " + ex.Message, ex);
			}
			catch (Exception ex)
			{
				throw new Exception("Unexpected error in UserRepository.GetByEmail(): " + ex.Message, ex);
			}
		}

		public bool EmailExists(string email)
		{
			try
			{
				using var connection = _database.CreateOpenConnection();
				var sql = @"SELECT COUNT(1)
                            FROM Users
                            WHERE email = @Email";

				using var command = new SqlCommand(sql, connection);
				command.Parameters.AddWithValue("@Email", email);

				var result = (int)command.ExecuteScalar();
				return result > 0;
			}
			catch (SqlException ex)
			{
				throw new Exception("SQL error in UserRepository.EmailExists(): " + ex.Message, ex);
			}
			catch (Exception ex)
			{
				throw new Exception("Unexpected error in UserRepository.EmailExists(): " + ex.Message, ex);
			}
		}

		public int Create(AppUser user)
		{
			try
			{
				using var connection = _database.CreateOpenConnection();
				var sql = @"
                    INSERT INTO Users (name, email, password)
                    OUTPUT INSERTED.user_id
                    VALUES (@Name, @Email, @Password)";

				using var command = new SqlCommand(sql, connection);
				command.Parameters.AddWithValue("@Name", user.Name);
				command.Parameters.AddWithValue("@Email", user.Email);
				command.Parameters.AddWithValue("@Password", user.PasswordHash);

				var result = command.ExecuteScalar();
				return (int)result;
			}
			catch (SqlException ex)
			{
				throw new Exception("SQL error in UserRepository.Create(): " + ex.Message, ex);
			}
			catch (Exception ex)
			{
				throw new Exception("Unexpected error in UserRepository.Create(): " + ex.Message, ex);
			}
		}
		public void SaveRefreshToken(int userId, string refreshToken, DateTime expiresAt)
		{
			try
			{
				using var connection = _database.CreateOpenConnection();

				var sql = @"UPDATE Users
            SET refresh_token = @token,
            refresh_token_expires_at = @expiresAt
            WHERE user_id = @userId";

				using var command = new SqlCommand(sql, connection);
				command.Parameters.AddWithValue("@token", refreshToken);
				command.Parameters.AddWithValue("@expiresAt", expiresAt);
				command.Parameters.AddWithValue("@userId", userId);

				command.ExecuteNonQuery();
			}
			catch (SqlException ex)
			{
				throw new Exception("SQL error in UserRepository.SaveRefreshToken(): " + ex.Message, ex);
			}
			catch (Exception ex)
			{
				throw new Exception("Unexpected error in UserRepository.SaveRefreshToken(): " + ex.Message, ex);
			}
		}
		public AppUser? GetByRefreshToken(string refreshToken)
		{
			try
			{
				using var connection = _database.CreateOpenConnection();

				var sql = @"SELECT user_id, name, email, password, refresh_token_expires_at
            FROM Users
            WHERE refresh_token = @token";

				using var command = new SqlCommand(sql, connection);
				command.Parameters.AddWithValue("@token", refreshToken);

				using var reader = command.ExecuteReader();

				if (reader.Read())
				{
					var expiresAt = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4);

					if (expiresAt.HasValue && expiresAt.Value < DateTime.UtcNow)
						return null;

					return new AppUser
					{
						Id = reader.GetInt32(0),
						Name = reader.GetString(1),
						Email = reader.GetString(2),
						PasswordHash = reader.GetString(3)
					};
				}

				return null;
			}
			catch (SqlException ex)
			{
				throw new Exception("SQL error in UserRepository.GetByRefreshToken(): " + ex.Message, ex);
			}
			catch (Exception ex)
			{
				throw new Exception("Unexpected error in UserRepository.GetByRefreshToken(): " + ex.Message, ex);
			}
		}


	}
}
