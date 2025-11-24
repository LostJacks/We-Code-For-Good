using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;


namespace LabelingTool.DAL
{
	public class Database
	{
		private readonly string _connectionString;

		public Database(string connectionString)
		{
			_connectionString = connectionString;
		}

		public void TestConnection()
		{
			try
			{
				using var connection = new SqlConnection(_connectionString);
				connection.Open();
			}
			catch (SqlException ex)
			{
				throw new Exception("Database connection failed: " , ex);
			}
			catch (Exception ex)
			{
				throw new Exception("Unexpected error: " , ex);
			}
		}

		public SqlConnection CreateOpenConnection()
		{
			var connection = new SqlConnection(_connectionString);
			connection.Open();
			return connection;
		}
	}
}
	



