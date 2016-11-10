using System;
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

            sessions.ForEach(s => s.AddSelfLink(Url));

            foreach (var sessionWithSelfLink in sessions)
            {
                EnrichSessionWithLinks(sessionWithSelfLink);
            }

            return Ok(sessions);
        }

        [Route("{sessionId:int}", Name = "Session")]
        public IHttpActionResult Get(int sessionId)
        {
            var session = _sessionsRepository.Get(sessionId);

            session.AddSelfLink(Url);
            session = AddDateLinkTo(session);
            session = AddHourLinkTo(session);

            EnrichSessionWithLinks(session);

            return Ok(session);
        }

        [Route("{sessionDate:datetime}", Name = "Date")]
        public IHttpActionResult Get(DateTime sessionDate)
        {
            var sessions = _sessionsRepository.GetAll().Where(s => s.StartTime.Date == sessionDate.Date).ToList();

            sessions.ForEach(s => s.AddSelfLink(Url));

            foreach (var sessionWithSelfLink in sessions)
            {
                EnrichSessionWithLinks(sessionWithSelfLink);
            }

            return Ok(sessions);
        }

        [Route("{sessionDate:datetime}/{hour:int}", Name = "Hour")]
        public IHttpActionResult Get(DateTime sessionDate, int hour)
        {
            var sessions =
                _sessionsRepository.GetAll()
                    .Where(
                        s => s.StartTime.Date == sessionDate.Date && s.StartTime.Hour <= hour && s.EndTime.Hour >= hour)
                    .ToList();

            sessions.ForEach(s => s.AddSelfLink(Url));

            foreach (var sessionWithSelfLink in sessions)
            {
                EnrichSessionWithLinks(sessionWithSelfLink);
            }

            return Ok(sessions);
        }

        [Route("{sessionDate:datetime}/{hour:int}/{room:int}")]
        public IHttpActionResult Get(DateTime sessionDate, int hour, int room)
        {
            var sessions =
                _sessionsRepository.GetAll()
                    .Where(
                        s => s.Room.Id == room 
                            && s.StartTime.Date == sessionDate.Date 
                            && s.StartTime.Hour <= hour 
                            && s.EndTime.Hour >= hour)
                    .ToList();

            sessions.ForEach(s => s.AddSelfLink(Url));

            foreach (var sessionWithSelfLink in sessions)
            {
                EnrichSessionWithLinks(sessionWithSelfLink);
            }

            return Ok(sessions);
        }

        [Route("tags/{name}", Name = "Tag")]
        public IHttpActionResult Get(string name)
        {
            var sessionsWithTag = _sessionsRepository.GetAll().Where(s => s.Tags.Any(t => t.Name == name)).ToList();

            sessionsWithTag.ForEach(s => s.AddSelfLink(Url));

            foreach (var sessionWithSelfLink in sessionsWithTag)
            {
                EnrichSessionWithLinks(sessionWithSelfLink);
            }

            return Ok(sessionsWithTag);
        }

        private void EnrichSessionWithLinks(SessionModel sessionWithSelfLink)
        {
            sessionWithSelfLink.Speakers.ForEach(s => s.AddSelfLink(Url));
            sessionWithSelfLink.Tags.ForEach(t => t.AddSelfLink(Url));
            sessionWithSelfLink.Room.AddSelfLink(Url);
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
    }
}