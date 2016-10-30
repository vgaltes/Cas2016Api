using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;
using System.Web.Http.Routing;
using Cas2016.Api.Models;
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
        public void SessionShouldIncludeLinkToSelf()
        {
            const int sessionId = 1;

            var sessionReturnedByRepo =
                new Fixture().Build<SessionModel>().Without(s => s.Links).With(s => s.Id, 1).Create();

            var urlHelper = new Mock<UrlHelper>();

            var sessionsController =
                new SessionsControllerBuilder().With(urlHelper).Returning(new[] {sessionReturnedByRepo}).Build();


            var response = sessionsController.Get(sessionId);

            var okResult = (OkNegotiatedContentResult<SessionModel>) response;

            okResult.Content.Links.Should().HaveCount(1);

            urlHelper.Verify(
                h => h.Link("Session", It.Is<object>(o => o.GetPropertyValue<int>("sessionId") == sessionId)));
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
                (OkNegotiatedContentResult<IEnumerable<SessionModel>>) response;

            okResult.Content.All(s => s.Links.Any(l => l.Rel == "self")).Should().BeTrue();
            okResult.Content.All(s =>
                {
                    urlHelper.Verify(
                        h => h.Link("Session", It.Is<object>(o => o.GetPropertyValue<int>("sessionId") == s.Id)));
                    return true;
                }
            ).Should().BeTrue();
        }

        [Test]
        public void SessionsReturnedBySessionsShouldIncludeListOfAuthors()
        {
            const int numberOfSessions = 5;
            const int speakerId = 1;

            var author = new Fixture().Build<MinimalSpeakerModel>()
                .Without(s => s.Links)
                .With(s => s.Id, speakerId)
                .Create();

            var sessionsReturnedByRepo = new Fixture()
                .Build<SessionModel>()
                .Without(s => s.Links).With(s => s.Id, 1)
                .With(s => s.Speakers, new[] { author })
                .CreateMany(numberOfSessions);

            var urlHelper = new Mock<UrlHelper>();

            var sessionsController = new SessionsControllerBuilder()
                .With(urlHelper).Returning(sessionsReturnedByRepo).Build();

            var response = sessionsController.Get();

            var okResult =
                (OkNegotiatedContentResult<IEnumerable<SessionModel>>)response;

            okResult.Content.All(s => s.Speakers.Count() == 1);

            urlHelper.Verify(
                h => h.Link("Speakers", It.Is<object>(o => o.GetPropertyValue<int>("speakerId") == speakerId)),
                Times.Exactly(numberOfSessions));
        }

        [Test]
        public void SessionsByDateShouldIncludeListOfAuthors()
        {
            const int numberOfSessions = 5;
            const int speakerId = 1;

            var author = new Fixture().Build<MinimalSpeakerModel>()
                .Without(s => s.Links)
                .With(s => s.Id, speakerId)
                .Create();

            var sessionsReturnedByRepo = new Fixture()
                .Build<SessionModel>()
                .Without(s => s.Links).With(s => s.Id, 1)
                .With(s => s.Speakers, new[] { author })
                .With(s => s.StartTime, DateTime.Now)
                .CreateMany(numberOfSessions);

            var urlHelper = new Mock<UrlHelper>();

            var sessionsController = new SessionsControllerBuilder()
                .With(urlHelper).Returning(sessionsReturnedByRepo).Build();

            var response = sessionsController.Get(DateTime.Today);

            var okResult =
                (OkNegotiatedContentResult<IEnumerable<SessionModel>>)response;

            okResult.Content.All(s => s.Speakers.Count() == 1);

            urlHelper.Verify(
                h => h.Link("Speakers", It.Is<object>(o => o.GetPropertyValue<int>("speakerId") == speakerId)),
                Times.Exactly(numberOfSessions));
        }

        [Test]
        public void SessionShouldIncludeListOfAuthors()
        {
            const int sessionId = 1;
            const int speakerId = 1;

            var author = new Fixture().Build<MinimalSpeakerModel>()
                .Without(s => s.Links)
                .With(s => s.Id, speakerId)
                .Create();

            var sessionReturnedByRepo =
                new Fixture().Build<SessionModel>()
                .Without(s => s.Links).With(s => s.Id, 1)
                .With(s => s.Speakers, new [] {author})
                .Create();

            var urlHelper = new Mock<UrlHelper>();

            var sessionsController =
                new SessionsControllerBuilder().With(urlHelper).Returning(new[] { sessionReturnedByRepo }).Build();

            var response = sessionsController.Get(sessionId);

            var okResult = (OkNegotiatedContentResult<SessionModel>)response;

            okResult.Content.Speakers.Should().HaveCount(1);
            okResult.Content.Speakers.First().Links.Should().HaveCount(1);
            okResult.Content.Speakers.First().Links.First().Rel.Should().Be("self");
            urlHelper.Verify(h => h.Link("Speakers", It.Is<object>(o => o.GetPropertyValue<int>("speakerId") == speakerId)));
        }

        [Test]
        public void SessionsByDateShouldIncludeAllSessionsOfThatDay()
        {
            const int todaySessionCount = 3;
            const int tomorrowSessionCount = 4;

            var sessionsOfToday = new Fixture().Build<SessionModel>()
                .With(s => s.StartTime, DateTime.Now)
                .CreateMany(todaySessionCount);

            var sessionsOfTomorrow = new Fixture().Build<SessionModel>()
                .With(s => s.StartTime, DateTime.Now.AddDays(1))
                .CreateMany(tomorrowSessionCount);

            var sessionsController = new SessionsControllerBuilder()
                .Returning(sessionsOfToday.Union(sessionsOfTomorrow))
                .Build();

            var response = sessionsController.Get(DateTime.Now);


            var okResult = (OkNegotiatedContentResult<IEnumerable<SessionModel>>)response;

            okResult.Content.Should().HaveCount(todaySessionCount);
        }

        [Test]
        public void SessionsByDateShouldIncludeLinkToSelf()
        {
            const int numberOfSessions = 5;

            var sessionsReturnedByRepo =
                new Fixture().Build<SessionModel>()
                .With(s => s.StartTime, DateTime.Now)
                .CreateMany(numberOfSessions);

            var urlHelper = new Mock<UrlHelper>();

            var sessionsController = new SessionsControllerBuilder()
                .Returning(sessionsReturnedByRepo)
                .With(urlHelper)
                .Build();

            var response = sessionsController.Get(DateTime.Now);

            var okResult =
                (OkNegotiatedContentResult<IEnumerable<SessionModel>>)response;

            okResult.Content.All(s => s.Links.Any(l => l.Rel == "self")).Should().BeTrue();
            okResult.Content.All(s =>
            {
                urlHelper.Verify(
                    h => h.Link("Session", It.Is<object>(o => o.GetPropertyValue<int>("sessionId") == s.Id)));
                return true;
            }
            ).Should().BeTrue();
        }
    }
}