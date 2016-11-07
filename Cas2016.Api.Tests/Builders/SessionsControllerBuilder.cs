using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Routing;
using Cas2016.Api.Controllers;
using Cas2016.Api.Models;
using Cas2016.Api.Repositories;
using Moq;

namespace Cas2016.Api.Tests.Builders
{
    internal class SessionsControllerBuilder
    {
        private readonly Mock<ISessionRepository> _sessionRepository;
        private IEnumerable<SessionModel> _sessionsReturned;
        private Mock<UrlHelper> _urlHelper;

        public SessionsControllerBuilder()
        {
            _urlHelper = new Mock<UrlHelper>();
            _sessionRepository = new Mock<ISessionRepository>();
            _sessionsReturned = new List<SessionModel>();
        }

        public SessionsControllerBuilder With(Mock<UrlHelper> urlHelper)
        {
            _urlHelper = urlHelper;
            return this;
        }

        public SessionsControllerBuilder Returning(List<SessionModel> sessionsReturnedByRepo)
        {
            _sessionRepository.Setup(r => r.GetAll()).Returns(sessionsReturnedByRepo);
            _sessionRepository.Setup(r => r.Get(It.IsAny<int>()))
                .Returns((int id) => _sessionsReturned.Single(s => s.Id == id));
            _sessionsReturned = sessionsReturnedByRepo;

            return this;
        }

        public SessionsController Build()
        {
            var sessionsController = new SessionsController(_sessionRepository.Object) {Url = _urlHelper.Object};

            return sessionsController;
        }
    }
}