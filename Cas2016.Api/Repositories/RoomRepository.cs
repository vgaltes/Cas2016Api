using System.Collections.Generic;
using System.Data.SqlClient;
using Cas2016.Api.Models;

namespace Cas2016.Api.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly string _targetConnectionString;

        public RoomRepository(string connectionString)
        {
            _targetConnectionString = connectionString;
        }

        public IEnumerable<RoomModel> GetAll()
        {
            var rooms = new List<RoomModel>();

            using (var connection = new SqlConnection(_targetConnectionString))
            {
                var command = new SqlCommand(
                    "SELECT * FROM Rooms;",
                    connection);

                connection.Open();

                var reader = command.ExecuteReader();

                if (reader.HasRows)
                    while (reader.Read())
                    {
                        var room = GetRoomFrom(reader);

                        rooms.Add(room);
                    }
                reader.Close();
                connection.Close();
            }

            return rooms;
        }

        public RoomModel Get(int roomId)
        {
            using (var connection = new SqlConnection(_targetConnectionString))
            {
                var command = new SqlCommand(
                    "SELECT * FROM Rooms WHERE Id = @ID;",
                    connection);

                var param = new SqlParameter
                {
                    ParameterName = "@Id",
                    Value = roomId
                };

                command.Parameters.Add(param);
                connection.Open();

                var reader = command.ExecuteReader();

                reader.Read();
                var room = GetRoomFrom(reader);
                reader.Close();
                connection.Close();

                return room;
            }
        }

        private static RoomModel GetRoomFrom(SqlDataReader reader)
        {
            var room = new RoomModel
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Capacity = reader.GetInt32(2)
            };
            return room;
        }
    }
}