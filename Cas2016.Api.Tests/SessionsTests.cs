using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Http.Routing;
using Cas2016.Api.Controllers;
using Cas2016.Api.Models;
using Cas2016.Api.Repositories;
using Cas2016.Api.Tests.Builders;
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

            var sessionsReturnedByRepo = new Fixture().CreateMany<SessionModel>(numberOfSessions);

            var sessionsController = new SessionsControllerBuilder().Returning(sessionsReturnedByRepo).Build();

            var response = sessionsController.Get();

            var okResult =
                (OkNegotiatedContentResult<IEnumerable<SessionModel>>) response;

            okResult.Content.Should().HaveCount(numberOfSessions);
            okResult.Content.ShouldBeEquivalentTo(sessionsReturnedByRepo);
        }

        [Test]
        public void SessionsReturnedBySessionsShouldIncludeLinkToSelf()
        {
            const int numberOfSessions = 5;

            var sessionsReturnedByRepo = new Fixture().CreateMany<SessionModel>(numberOfSessions);
            var urlHelper = new Mock<UrlHelper>();

            var sessionsController = new SessionsControllerBuilder()
                .Returning(sessionsReturnedByRepo)
                .With(urlHelper)
                .Build();

            var response = sessionsController.Get();

            var okResult =
                (OkNegotiatedContentResult<IEnumerable<SessionModel>>)response;

            okResult.Content.All(s => s.Links.Any(l => l.Rel == "self")).Should().BeTrue();
            okResult.Content.All(s =>
                {
                    urlHelper.Verify(
                        h => h.Link("Sessions", It.Is<object>(o => o.GetPropertyValue<int>("sessionId") == s.Id)));
                    return true;
                }
            ).Should().BeTrue();
        }

        [Test]
        public void SessionShouldIncludeLinkToSelf()
        {
            const int sessionId = 1;

            var sessionReturnedByRepo =
                new Fixture().Build<SessionModel>().Without(s => s.Links).With(s => s.Id, 1).Create();

            var urlHelper = new Mock<UrlHelper>();

            var sessionsController = new SessionsControllerBuilder().With(urlHelper).Returning(new [] {sessionReturnedByRepo}).Build();


            var response = sessionsController.Get(sessionId);

            var okResult = (OkNegotiatedContentResult<SessionModel>) response;

            okResult.Content.Links.Should().HaveCount(1);

            urlHelper.Verify(
                h => h.Link("Sessions", It.Is<object>(o => o.GetPropertyValue<int>("sessionId") == sessionId)));
        }

        
    }
}