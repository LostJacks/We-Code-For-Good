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
				var sql = @"SELECT [Id], [UserName], [FirstName], [LastName], [Email], [PasswordHash]
        FROM [Users]
        WHERE [Id] = @id";

                using var command = new SqlCommand(sql, connection);
				command.Parameters.AddWithValue("@id", id);

				using var reader = command.ExecuteReader();

				if (reader.Read())
				{
					return new AppUser
					{
						Id = reader.GetInt32(0),
						UserName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
						FirstName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
						LastName = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
						Email = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
						PasswordHash = reader.IsDBNull(5) ? string.Empty : reader.GetString(5)
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
				var sql = @"SELECT [Id], [UserName], [FirstName], [LastName], [Email], [PasswordHash]
                    FROM [Users]
                    WHERE [Email] = @Email";

                using var command = new SqlCommand(sql, connection);
				command.Parameters.AddWithValue("@Email", email);

				using var reader = command.ExecuteReader();

				if (reader.Read())
				{
					return new AppUser
					{
						Id = reader.GetInt32(0),
						UserName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
						FirstName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
						LastName = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
						Email = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
						PasswordHash = reader.IsDBNull(5) ? string.Empty : reader.GetString(5)
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
                    FROM [Users]
                    WHERE [Email] = @Email";

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
            INSERT INTO [Users] ([UserName], [FirstName], [LastName], [Email], [PasswordHash])
            OUTPUT INSERTED.[Id]
            VALUES (@UserName, @FirstName, @LastName, @Email, @PasswordHash)";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@UserName", user.UserName);
                command.Parameters.AddWithValue("@FirstName", user.FirstName ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@LastName", user.LastName ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);

                var result = command.ExecuteScalar();
                if (result == null)
                    throw new Exception("Failed to create user - database returned null");

                return (int)result;
            }
            catch (SqlException ex)
            {
                throw new Exception("SQL error in UserRepository.Create(): " + ex.Message, ex);
            }
        }


        public void SaveRefreshToken(int userId, string refreshToken, DateTime expiresAt)
		{
			try
			{
				using var connection = _database.CreateOpenConnection();

				var sql = @"UPDATE [Users]
                    SET [refresh_token] = @token,
                        [refresh_token_expires_at] = @expiresAt
                    WHERE [Id] = @userId";

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

				var sql = @"SELECT [Id], [UserName], [FirstName], [LastName], [Email], [PasswordHash], [refresh_token_expires_at]
                    FROM [Users]
                    WHERE [refresh_token] = @token";

                using var command = new SqlCommand(sql, connection);
				command.Parameters.AddWithValue("@token", refreshToken);

				using var reader = command.ExecuteReader();

				if (reader.Read())
				{
					var expiresAt = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6);

					if (expiresAt.HasValue && expiresAt.Value < DateTime.UtcNow)
						return null;

					return new AppUser
					{
						Id = reader.GetInt32(0),
						UserName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
						FirstName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
						LastName = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
						Email = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
						PasswordHash = reader.IsDBNull(5) ? string.Empty : reader.GetString(5)
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
