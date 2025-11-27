using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Model.BBDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelingTool.DAL.BBRepositories
{
    public class BoundingBoxRepository : IBoundingBoxRepository
    {
        private readonly string _connectionString;

        public BoundingBoxRepository(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.json");

            _connectionString = connectionString;
        }

        public async Task<List<BoundingBoxDto>> GetByMediaIdAsync(int mediaId)
        {
            var boundingBoxes = new List<BoundingBoxDto>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand(@"
                    SELECT 
                        bb.bounding_box_id,
                        bb.media_id,
                        bb.labelclass_id,
                        lc.name AS class_name,
                        bb.x1,
                        bb.y1,
                        bb.x2,
                        bb.y2
                    FROM Boundingbox bb
                    INNER JOIN Labelclass lc ON bb.labelclass_id = lc.labelclass_id
                    WHERE bb.media_id = @MediaId
                    ORDER BY bb.bounding_box_id", connection);

                command.Parameters.AddWithValue("@MediaId", mediaId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        boundingBoxes.Add(new BoundingBoxDto
                        {
                            BoundingBoxId = reader.GetInt32(0),
                            MediaId = reader.GetInt32(1),
                            LabelClassId = reader.GetInt32(2),
                            ClassName = reader.GetString(3),
                            X1 = reader.GetFloat(4),
                            Y1 = reader.GetFloat(5),
                            X2 = reader.GetFloat(6),
                            Y2 = reader.GetFloat(7)
                        });
                    }
                }
            }

            return boundingBoxes;
        }

        public async Task<BoundingBoxDto> GetByIdAsync(int boundingBoxId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand(@"
                    SELECT 
                        bb.bounding_box_id,
                        bb.media_id,
                        bb.labelclass_id,
                        lc.name AS class_name,
                        bb.x1,
                        bb.y1,
                        bb.x2,
                        bb.y2
                    FROM Boundingbox bb
                    INNER JOIN Labelclass lc ON bb.labelclass_id = lc.labelclass_id
                    WHERE bb.bounding_box_id = @BoundingBoxId", connection);

                command.Parameters.AddWithValue("@BoundingBoxId", boundingBoxId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new BoundingBoxDto
                        {
                            BoundingBoxId = reader.GetInt32(0),
                            MediaId = reader.GetInt32(1),
                            LabelClassId = reader.GetInt32(2),
                            ClassName = reader.GetString(3),
                            X1 = reader.GetFloat(4),
                            Y1 = reader.GetFloat(5),
                            X2 = reader.GetFloat(6),
                            Y2 = reader.GetFloat(7)
                        };
                    }
                }
            }

            return null;
        }

        public async Task<int> CreateAsync(CreateBoundingBoxRequest request)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand(@"
                    INSERT INTO Boundingbox (media_id, labelclass_id, x1, y1, x2, y2)
                    OUTPUT INSERTED.bounding_box_id
                    VALUES (@MediaId, @LabelClassId, @X1, @Y1, @X2, @Y2)",
                    connection);

                command.Parameters.AddWithValue("@MediaId", request.MediaId);
                command.Parameters.AddWithValue("@LabelClassId", request.LabelClassId);
                command.Parameters.AddWithValue("@X1", request.X1);
                command.Parameters.AddWithValue("@Y1", request.Y1);
                command.Parameters.AddWithValue("@X2", request.X2);
                command.Parameters.AddWithValue("@Y2", request.Y2);

                var newId = (int)await command.ExecuteScalarAsync();
                return newId;
            }
        }

        public async Task UpdateAsync(UpdateBoundingBoxRequest request)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand(@"
                    UPDATE Boundingbox 
                    SET 
                        labelclass_id = @LabelClassId,
                        x1 = @X1,
                        y1 = @Y1,
                        x2 = @X2,
                        y2 = @Y2
                    WHERE bounding_box_id = @BoundingBoxId",
                    connection);

                command.Parameters.AddWithValue("@BoundingBoxId", request.BoundingBoxId);
                command.Parameters.AddWithValue("@LabelClassId", request.LabelClassId);
                command.Parameters.AddWithValue("@X1", request.X1);
                command.Parameters.AddWithValue("@Y1", request.Y1);
                command.Parameters.AddWithValue("@X2", request.X2);
                command.Parameters.AddWithValue("@Y2", request.Y2);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteAsync(int boundingBoxId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand(@"
                    DELETE FROM Boundingbox 
                    WHERE bounding_box_id = @BoundingBoxId",
                    connection);

                command.Parameters.AddWithValue("@BoundingBoxId", boundingBoxId);

                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
