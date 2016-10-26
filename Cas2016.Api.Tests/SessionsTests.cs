using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Http.Routing;
using Cas2016.Api.Controllers;
using Cas2016.Api.Models;
using Cas2016.Api.Repositories;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Cas2016.Api.Tests
{
    [TestFixture]
    public class SessionsTests
    {
        public static void SetupControllerForTests(ApiController controller, string url)
        {
        }

        [Test]
        public void SessionShouldIncludeAllSessionData()
        {
            const int numberOfSessions = 5;

            var sessionRepository = new Mock<ISessionRepository>();
            var sessionsReturnedByRepo = new Fixture().CreateMany<SessionModel>(numberOfSessions);
            sessionRepository.Setup(r => r.GetAll()).Returns(sessionsReturnedByRepo);

            var sessionsController = new SessionsController(sessionRepository.Object);

            var response = sessionsController.Get();

            var okResult =
                (OkNegotiatedContentResult<IEnumerable<SessionModel>>) response;

            okResult.Content.Should().HaveCount(numberOfSessions);
            okResult.Content.ShouldBeEquivalentTo(sessionsReturnedByRepo);
        }

        [Test]
        public void SessionShouldIncludeLinkToSelf()
        {
            const int sessionId = 1;

            var sessionReturnedByRepo = new Fixture().Build<SessionModel>().Without(s => s.Links).Create();
            var sessionRepository = new Mock<ISessionRepository>();
            sessionRepository.Setup(r => r.Get(sessionId)).Returns(sessionReturnedByRepo);

            var sessionsController = new SessionsController(sessionRepository.Object);

            var urlHelper = new Mock<UrlHelper>();
            sessionsController.Url = urlHelper.Object;

            var response = sessionsController.Get(sessionId);

            var okResult = (OkNegotiatedContentResult<SessionModel>) response;

            okResult.Content.Links.Should().HaveCount(1);

            urlHelper.Verify(
                h => h.Link("Sessions", It.Is<object>(o => o.GetPropertyValue<int>("sessionId") == sessionId)));
        }
    }
}