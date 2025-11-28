using System.Collections.Generic;
using System.Threading.Tasks;
using LabelingTool.DAL.Interfaces;
using Microsoft.Data.SqlClient;
using Model.BBDTOs;

namespace LabelingTool.DAL.BBRepositories
{
	public class BoundingBoxRepository : IBoundingBoxRepository
	{
		private readonly Database _database;

		public BoundingBoxRepository(Database database)
		{
			_database = database;
		}
		public async Task<List<BoundingBoxDto>> GetByMediaIdAsync(int mediaId)
		{
			var boxes = new List<BoundingBoxDto>();

			using var connection = _database.CreateOpenConnection();

			var sql = @"
                SELECT 
                    b.boundingbox_id,
                    b.media_id,
                    b.labelclass_id,
                    l.name AS class_name,
                    b.x1, b.y1, b.x2, b.y2
                FROM Boundingbox b
                JOIN Labelclass l ON b.labelclass_id = l.labelclass_id
                WHERE b.media_id = @MediaId;";

			using var command = new SqlCommand(sql, connection);
			command.Parameters.AddWithValue("@MediaId", mediaId);

			using var reader = await command.ExecuteReaderAsync();
			while (await reader.ReadAsync())
			{
				var dto = new BoundingBoxDto
				{
					BoundingBoxId = reader.GetInt32(0),
					MediaId = reader.GetInt32(1),
					LabelClassId = reader.GetInt32(2),
					ClassName = reader.GetString(3),
					X1 = (float)reader.GetDouble(4),
					Y1 = (float)reader.GetDouble(5),
					X2 = (float)reader.GetDouble(6),
					Y2 = (float)reader.GetDouble(7)
				};

				boxes.Add(dto);
			}

			return boxes;
		}

		public async Task<BoundingBoxDto?> GetByIdAsync(int boundingBoxId)
		{
			using var connection = _database.CreateOpenConnection();

			var sql = @"
                SELECT 
                    b.boundingbox_id,
                    b.media_id,
                    b.labelclass_id,
                    l.name AS class_name,
                    b.x1, b.y1, b.x2, b.y2
                FROM Boundingbox b
                JOIN Labelclass l ON b.labelclass_id = l.labelclass_id
                WHERE b.boundingbox_id = @Id;";

			using var command = new SqlCommand(sql, connection);
			command.Parameters.AddWithValue("@Id", boundingBoxId);

			using var reader = await command.ExecuteReaderAsync();
			if (!await reader.ReadAsync())
			{
				return null;
			}

			return new BoundingBoxDto
			{
				BoundingBoxId = reader.GetInt32(0),
				MediaId = reader.GetInt32(1),
				LabelClassId = reader.GetInt32(2),
				ClassName = reader.GetString(3),
				X1 = (float)reader.GetDouble(4),
				Y1 = (float)reader.GetDouble(5),
				X2 = (float)reader.GetDouble(6),
				Y2 = (float)reader.GetDouble(7)
			};
		}

		public async Task<int> CreateAsync(CreateBoundingBoxRequest request)
		{
			using var connection = _database.CreateOpenConnection();

			var sql = @"
                INSERT INTO Boundingbox (media_id, labelclass_id, x1, y1, x2, y2)
                OUTPUT INSERTED.boundingbox_id
                VALUES (@MediaId, @LabelClassId, @X1, @Y1, @X2, @Y2);";

			using var command = new SqlCommand(sql, connection);
			command.Parameters.AddWithValue("@MediaId", request.MediaId);
			command.Parameters.AddWithValue("@LabelClassId", request.LabelClassId);
			command.Parameters.AddWithValue("@X1", request.X1);
			command.Parameters.AddWithValue("@Y1", request.Y1);
			command.Parameters.AddWithValue("@X2", request.X2);
			command.Parameters.AddWithValue("@Y2", request.Y2);

			var result = await command.ExecuteScalarAsync();
			return (int)result!;
		}

		public async Task UpdateAsync(UpdateBoundingBoxRequest request)
		{
			using var connection = _database.CreateOpenConnection();

			var sql = @"
                UPDATE Boundingbox
                SET labelclass_id = @LabelClassId,
                    x1 = @X1,
                    y1 = @Y1,
                    x2 = @X2,
                    y2 = @Y2
                WHERE boundingbox_id = @Id;";

			using var command = new SqlCommand(sql, connection);
			command.Parameters.AddWithValue("@Id", request.BoundingBoxId);
			command.Parameters.AddWithValue("@LabelClassId", request.LabelClassId);
			command.Parameters.AddWithValue("@X1", request.X1);
			command.Parameters.AddWithValue("@Y1", request.Y1);
			command.Parameters.AddWithValue("@X2", request.X2);
			command.Parameters.AddWithValue("@Y2", request.Y2);

			await command.ExecuteNonQueryAsync();
		}

	
		public async Task DeleteAsync(int boundingBoxId)
		{
			using var connection = _database.CreateOpenConnection();

			var sql = @"DELETE FROM Boundingbox WHERE boundingbox_id = @Id;";

			using var command = new SqlCommand(sql, connection);
			command.Parameters.AddWithValue("@Id", boundingBoxId);

			await command.ExecuteNonQueryAsync();
		}
	}
}
