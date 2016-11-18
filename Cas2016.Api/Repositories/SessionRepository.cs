﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Cas2016.Api.Models;

namespace Cas2016.Api.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private readonly string _targetConnectionString;
        private readonly ISpeakerRepository _speakerRepository;
        private readonly IRoomRepository _roomRepository;

        public SessionRepository(string connectionString, ISpeakerRepository speakerRepository,
            IRoomRepository roomRepository)
        {
            _targetConnectionString = connectionString;
            _speakerRepository = speakerRepository;
            _roomRepository = roomRepository;
        }

        public List<SessionModel> GetAll()
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
                        var session = GetSessionFrom(reader);

                        sessions.Add(session);
                    }
                reader.Close();

                foreach (var session in sessions)
                {
                    session.Speakers = GetSpeakersFor(session.Id, connection);
                }

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
                var session = GetSessionFrom(reader);
                reader.Close();

                var speakers = GetSpeakersFor(sessionId, connection);
                

                connection.Close();

                session.Speakers = speakers;
                return session;
            }
        }

        private SessionModel GetSessionFrom(SqlDataReader reader)
        {
            var session = new SessionModel
            {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                Description = reader.GetString(2),
                Duration = reader.GetInt32(3),
                StartTime = GetDateTimeUtc( reader, 4),
                EndTime = GetDateTimeUtc(reader, 5),
                Tags = reader.IsDBNull(6) ? new List<TagModel>() : GetTagsFrom(reader.GetString(6)),
                Room = _roomRepository.Get(reader.GetInt32(7)),
                IsPlenary = reader.GetBoolean(8)
            };
            return session;
        }

        public static DateTime GetDateTimeUtc(SqlDataReader reader, int field)
        {
            DateTime unspecified = reader.GetDateTime(field);
            return DateTime.SpecifyKind(unspecified, DateTimeKind.Utc);
        }

    private List<MinimalSpeakerModel> GetSpeakersFor(int sessionId, SqlConnection connection)
        {
            var speakersIdCommand =
                new SqlCommand("SELECT SpeakerId from SessionsSpeakers " +
                               "WHERE SessionId = @SessionId", connection);

            var sessionParameter = new SqlParameter
            {
                ParameterName = "@SessionId",
                Value = sessionId
            };

            speakersIdCommand.Parameters.Add(sessionParameter);
            var speakerReader = speakersIdCommand.ExecuteReader();

            var speakers = new List<MinimalSpeakerModel>();
            while (speakerReader.Read())
            {
                var speakerId = speakerReader.GetInt32(0);
                var speaker = _speakerRepository.Get(speakerId);
                speakers.Add(new MinimalSpeakerModel
                {
                    Id = speaker.Id,
                    Name = speaker.Name,
                    Links = new List<LinkModel>()
                });
            }
            speakerReader.Close();
            return speakers;
        }

        private List<TagModel> GetTagsFrom(string tags)
        {
            return tags.Split(new [] {";"}, StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).Select(t => new TagModel {Name = t}).ToList();
        }
    }
}