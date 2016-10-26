using System.Collections.Generic;
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

            return Ok(sessions);
        }

        public IHttpActionResult Get(int sessionId)
        {
            var session = _sessionsRepository.Get(sessionId);

            var selfLink = ModelFactory.CreateLink(Url, "self", "Sessions", new {sessionId = sessionId});
            session.Links = new List<LinkModel> {selfLink};

            return Ok(session);
        }
    }
}