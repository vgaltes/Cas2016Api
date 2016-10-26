using System.Collections.Generic;
using System.Data.SqlClient;
using Cas2016.Api.Models;

namespace Cas2016.Api.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private readonly string _targetConnectionString;

        public SessionRepository(string connectionString)
        {
            _targetConnectionString = connectionString;
        }

        public IEnumerable<SessionModel> GetAll()
        {
            var sessions = new List<SessionModel>();

            using (var connection = new SqlConnection(_targetConnectionString))
            {
                var command = new SqlCommand(
                    "SELECT * FROM Sessions;",
                    connection);

                connection.Open();

                var reader = command.ExecuteReader();

                if (reader.HasRows)
                    while (reader.Read())
                    {
                        var session = new SessionModel
                        {
                            Id = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            Description = reader.GetString(2),
                            Duration = reader.GetInt32(3),
                            StartTime = reader.GetDateTime(4),
                            EndTime = reader.GetDateTime(5)
                        };

                        sessions.Add(session);
                    }
                reader.Close();
                connection.Close();
            }

            return sessions;
        }

        public SessionModel Get(int sessionId)
        {
            using (var connection = new SqlConnection(_targetConnectionString))
            {
                var command = new SqlCommand(
                    "SELECT * FROM Sessions WHERE Id = @ID;",
                    connection);

                var param = new SqlParameter
                {
                    ParameterName = "@Id",
                    Value = sessionId
                };

                command.Parameters.Add(param);
                connection.Open();

                var reader = command.ExecuteReader();

                reader.Read();
                var session = new SessionModel
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Description = reader.GetString(2),
                    Duration = reader.GetInt32(3),
                    StartTime = reader.GetDateTime(4),
                    EndTime = reader.GetDateTime(5),
                    Links = new List<LinkModel>()
                };
                reader.Close();
                connection.Close();

                return session;
            }
        }
    }
}