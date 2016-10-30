﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Cas2016.Api.Models;
using Cas2016.Api.Repositories;

namespace Cas2016.Api.Controllers
{
    public class SessionsController : ApiController
    {
        private readonly ISessionRepository _sessionsRepository;

        public SessionsController(ISessionRepository sessionRepository)
        {
            _sessionsRepository = sessionRepository;
        }

        public IHttpActionResult Get()
        {
            var sessions = _sessionsRepository.GetAll();

            var sessionsWithSelfLinks = sessions.Select(AddSelfLinkTo);

            return Ok(sessionsWithSelfLinks);
        }

        public IHttpActionResult Get(int sessionId)
        {
            var session = _sessionsRepository.Get(sessionId);

            var sessionWithSelfLink = AddSelfLinkTo(session);

            foreach (var speaker in sessionWithSelfLink.Speakers)
            {
                AddSelfLinkTo(speaker);
            }

            sessionWithSelfLink.Speakers = sessionWithSelfLink.Speakers.Select(AddSelfLinkTo);

            return Ok(sessionWithSelfLink);
        }

        private SessionModel AddSelfLinkTo(SessionModel session)
        {
            var selfLink = ModelFactory.CreateLink(Url, "self", "Sessions", new {sessionId = session.Id});
            session.Links = new List<LinkModel> {selfLink};

            return session;
        }

        private MinimalSpeakerModel AddSelfLinkTo(MinimalSpeakerModel speaker)
        {
            var selfLink = ModelFactory.CreateLink(Url, "self", "Speakers", new { speakerId = speaker.Id });
            speaker.Links = new List<LinkModel> { selfLink };

            return speaker;
        }
    }
}