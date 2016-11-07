using System.Collections.Generic;
using System.Data.SqlClient;
using Cas2016.Api.Models;

namespace Cas2016.Api.Repositories
{
    public class SpeakerRepository : ISpeakerRepository
    {
        private readonly string _targetConnectionString;

        public SpeakerRepository(string connectionString)
        {
            _targetConnectionString = connectionString;
        }

        public IEnumerable<SpeakerModel> GetAll()
        {
            var speakers = new List<SpeakerModel>();

            using (var connection = new SqlConnection(_targetConnectionString))
            {
                var command = new SqlCommand(
                    "SELECT * FROM Speakers;",
                    connection);

                connection.Open();

                var reader = command.ExecuteReader();

                if (reader.HasRows)
                    while (reader.Read())
                    {
                        var speaker = GetSpeakerFrom(reader);

                        speakers.Add(speaker);
                    }
                reader.Close();

                foreach (var speaker in speakers)
                {
                    speaker.Sessions = GetSessionsFor(speaker.Id, connection);
                }
                connection.Close();
            }

            return speakers;
        }

        public SpeakerModel Get(int sessionId)
        {
            using (var connection = new SqlConnection(_targetConnectionString))
            {
                var command = new SqlCommand(
                    "SELECT * FROM Speakers WHERE Id = @ID;",
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
                var speaker = GetSpeakerFrom(reader);
                reader.Close();

                speaker.Sessions = GetSessionsFor(speaker.Id, connection);
                connection.Close();

                return speaker;
            }
        }

        private static SpeakerModel GetSpeakerFrom(SqlDataReader reader)
        {
            var speaker = new SpeakerModel
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                TwitterProfile = reader.IsDBNull(3) ? null : reader.GetString(3),
                LinkedinProfile = reader.IsDBNull(4) ? null : reader.GetString(4),
                Website = reader.IsDBNull(5) ? null : reader.GetString(5),
                Biography = reader.GetString(6),
                Image = reader.GetString(7),
                City = reader.GetString(8),
                Country = reader.GetString(9),
                Links = new List<LinkModel>()
            };
            return speaker;
        }

        private IEnumerable<MinimalSessionModel> GetSessionsFor(int speakerId, SqlConnection connection)
        {
            var sessionsIdCommand =
                new SqlCommand("SELECT sp.SessionId, s.Title from SessionsSpeakers sp " +
                               "JOIN Sessions s on sp.SessionId = s.Id " +
                               "WHERE SpeakerId = @SpeakerId", connection);

            var speakerParameter = new SqlParameter
            {
                ParameterName = "@SpeakerId",
                Value = speakerId
            };

            sessionsIdCommand.Parameters.Add(speakerParameter);
            var sessionReader = sessionsIdCommand.ExecuteReader();

            var sessions = new List<MinimalSessionModel>();
            while (sessionReader.Read())
            {
                sessions.Add(new MinimalSessionModel
                {
                    Id = sessionReader.GetInt32(0),
                    Title = sessionReader.GetString(1),
                    Links = new List<LinkModel>()
                });
            }
            sessionReader.Close();
            return sessions;
        }
    }
}