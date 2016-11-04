using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Cas2016.Api.Models;
using Cas2016.Api.Repositories;

namespace Cas2016.Api.Controllers
{
    [RoutePrefix("sessions")]
    public class SessionsController : ApiController
    {
        private readonly ISessionRepository _sessionsRepository;

        public SessionsController(ISessionRepository sessionRepository)
        {
            _sessionsRepository = sessionRepository;
        }

        [Route("", Name = "Sessions")]
        public IHttpActionResult Get()
        {
            var sessions = _sessionsRepository.GetAll();

            var sessionsWithSelfLinks = sessions.Select(AddSelfLinkTo);

            foreach (var sessionWithSelfLink in sessionsWithSelfLinks)
            {
                sessionWithSelfLink.Speakers = sessionWithSelfLink.Speakers.Select(AddSelfLinkTo);
                sessionWithSelfLink.Tags = sessionWithSelfLink.Tags.Select(AddSelfLinkTo);
                sessionWithSelfLink.Room = AddSelfLinkTo(sessionWithSelfLink.Room);
            }

            return Ok(sessionsWithSelfLinks);
        }

        [Route("{sessionId:int}", Name = "Session")]
        public IHttpActionResult Get(int sessionId)
        {
            var session = _sessionsRepository.Get(sessionId);

            var sessionWithSelfLink = AddSelfLinkTo(session);
            sessionWithSelfLink = AddDateLinkTo(sessionWithSelfLink);
            sessionWithSelfLink = AddHourLinkTo(sessionWithSelfLink);

            sessionWithSelfLink.Speakers = sessionWithSelfLink.Speakers.Select(AddSelfLinkTo);
            sessionWithSelfLink.Tags = sessionWithSelfLink.Tags.Select(AddSelfLinkTo);
            sessionWithSelfLink.Room = AddSelfLinkTo(sessionWithSelfLink.Room);

            return Ok(sessionWithSelfLink);
        }

        [Route("{sessionDate:datetime}", Name = "Date")]
        public IHttpActionResult Get(DateTime sessionDate)
        {
            var sessions = _sessionsRepository.GetAll().Where(s => s.StartTime.Date == sessionDate.Date);

            var sessionsWithSelfLinks = sessions.Select(AddSelfLinkTo);

            foreach (var sessionWithSelfLink in sessionsWithSelfLinks)
            {
                sessionWithSelfLink.Speakers = sessionWithSelfLink.Speakers.Select(AddSelfLinkTo);
                sessionWithSelfLink.Tags = sessionWithSelfLink.Tags.Select(AddSelfLinkTo);
                sessionWithSelfLink.Room = AddSelfLinkTo(sessionWithSelfLink.Room);
            }

            return Ok(sessionsWithSelfLinks);
        }

        [Route("{sessionDate:datetime}/{hour:int}", Name = "Hour")]
        public IHttpActionResult Get(DateTime sessionDate, int hour)
        {
            var sessions = _sessionsRepository.GetAll().Where(s => s.StartTime.Date == sessionDate.Date && s.StartTime.Hour <= hour && s.EndTime.Hour >= hour);

            var sessionsWithSelfLinks = sessions.Select(AddSelfLinkTo);

            foreach (var sessionWithSelfLink in sessionsWithSelfLinks)
            {
                sessionWithSelfLink.Speakers = sessionWithSelfLink.Speakers.Select(AddSelfLinkTo);
                sessionWithSelfLink.Tags = sessionWithSelfLink.Tags.Select(AddSelfLinkTo);
                sessionWithSelfLink.Room = AddSelfLinkTo(sessionWithSelfLink.Room);
            }

            return Ok(sessionsWithSelfLinks);
        }

        [Route("tags/{name}", Name = "Tag")]
        public IHttpActionResult Get(string name)
        {
            var allSessions = _sessionsRepository.GetAll();

            var sessionsWithTag = allSessions.Where(s => s.Tags.Any(t => t.Name == name));

            var sessionsWithSelfLinks = sessionsWithTag.Select(AddSelfLinkTo);
            

            foreach (var sessionWithSelfLink in sessionsWithSelfLinks)
            {
                sessionWithSelfLink.Speakers = sessionWithSelfLink.Speakers.Select(AddSelfLinkTo);
                sessionWithSelfLink.Tags = sessionWithSelfLink.Tags.Select(AddSelfLinkTo);
                sessionWithSelfLink.Room = AddSelfLinkTo(sessionWithSelfLink.Room);
            }

            return Ok(sessionsWithSelfLinks);
        }

        private SessionModel AddSelfLinkTo(SessionModel session)
        {
            var selfLink = ModelFactory.CreateLink(Url, "self", "Session", new {sessionId = session.Id});
            session.Links = new List<LinkModel> {selfLink};

            return session;
        }

        private string GetDate(DateTime date)
        {
            return $"{date.Year}-{date.Month}-{date.Day}";
        }

        private SessionModel AddDateLinkTo(SessionModel session)
        {
            var dateLink = ModelFactory.CreateLink(Url, "date", "Date", new { sessionDate = GetDate(session.StartTime.Date) });
            session.Links.Add(dateLink);

            return session;
        }

        private SessionModel AddHourLinkTo(SessionModel session)
        {
            var hourLink = ModelFactory.CreateLink(Url, "hour", "Hour", new { sessionDate = GetDate(session.StartTime.Date), hour = session.StartTime.Hour });
            session.Links.Add(hourLink);

            return session;
        }

        private MinimalSpeakerModel AddSelfLinkTo(MinimalSpeakerModel speaker)
        {
            var selfLink = ModelFactory.CreateLink(Url, "self", "Speaker", new { speakerId = speaker.Id });
            speaker.Links = new List<LinkModel> { selfLink };

            return speaker;
        }

        private TagModel AddSelfLinkTo(TagModel tag)
        {
            var selfLink = ModelFactory.CreateLink(Url, "self", "Tag", new { name = tag.Name });
            tag.Links = new List<LinkModel> { selfLink };

            return tag;
        }

        private RoomModel AddSelfLinkTo(RoomModel room)
        {
            var selfLink = ModelFactory.CreateLink(Url, "self", "Room", new { roomId = room.Id });
            room.Links = new List<LinkModel> { selfLink };

            return room;
        }
    }
}