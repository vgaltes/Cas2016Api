using System.Web.Http;
using Cas2016.Api.Configuration;
using Cas2016.Api.Repositories;

namespace Cas2016.Api.Controllers
{
    public class SessionsController : ApiController
    {
        private SessionRepository _sessionsRepository;

        public SessionsController()
        {
            var configurationProvider = new ConfigurationProvider();
            _sessionsRepository = new SessionRepository(configurationProvider.DbConnectionString);
        }

        public IHttpActionResult Get()
        {
            var sessions = _sessionsRepository.GetAll();

            return Ok(sessions);
        }
    }
}