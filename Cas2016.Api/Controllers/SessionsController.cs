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
            }

            return Ok(sessionsWithSelfLinks);
        }

        [Route("{sessionId:int}", Name = "Session")]
        public IHttpActionResult Get(int sessionId)
        {
            var session = _sessionsRepository.Get(sessionId);

            var sessionWithSelfLink = AddSelfLinkTo(session);

            sessionWithSelfLink.Speakers = sessionWithSelfLink.Speakers.Select(AddSelfLinkTo);
            sessionWithSelfLink.Tags = sessionWithSelfLink.Tags.Select(AddSelfLinkTo);

            return Ok(sessionWithSelfLink);
        }

        [Route("{sessionDate:datetime}")]
        public IHttpActionResult Get(DateTime sessionDate)
        {
            var sessions = _sessionsRepository.GetAll().Where(s => s.StartTime.Date == sessionDate.Date);

            var sessionsWithSelfLinks = sessions.Select(AddSelfLinkTo);

            foreach (var sessionWithSelfLink in sessionsWithSelfLinks)
            {
                sessionWithSelfLink.Speakers = sessionWithSelfLink.Speakers.Select(AddSelfLinkTo);
                sessionWithSelfLink.Tags = sessionWithSelfLink.Tags.Select(AddSelfLinkTo);
            }

            return Ok(sessionsWithSelfLinks);
        }

        [Route("tags/{name}", Name = "Tag")]
        public IHttpActionResult Get(string name)
        {
            return Ok();
        }

        private SessionModel AddSelfLinkTo(SessionModel session)
        {
            var selfLink = ModelFactory.CreateLink(Url, "self", "Session", new {sessionId = session.Id});
            session.Links = new List<LinkModel> {selfLink};

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
    }
}