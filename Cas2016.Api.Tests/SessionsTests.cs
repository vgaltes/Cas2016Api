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

            var sessionsReturnedByRepo = new Fixture().CreateMany<SessionModel>(numberOfSessions).ToList();

            var sessionsController = new SessionsControllerBuilder().Returning(sessionsReturnedByRepo).Build();

            var response = sessionsController.Get();

            var okResult =
                (OkNegotiatedContentResult<List<SessionModel>>) response;

            okResult.Content.Should().HaveCount(numberOfSessions);
            okResult.Content.ShouldBeEquivalentTo(sessionsReturnedByRepo);
        }

        [Test]
        public void SessionShouldIncludeLinkToSelf()
        {
            const int sessionId = 1;

            var sessionReturnedByRepo =
                new Fixture().Build<SessionModel>().With(s => s.Links, new List<LinkModel>()).With(s => s.Id, 1).Create();

            var urlHelper = new Mock<UrlHelper>();

            var sessionsController =
                new SessionsControllerBuilder().With(urlHelper).Returning(new[] {sessionReturnedByRepo}.ToList()).Build();


            var response = sessionsController.Get(sessionId);

            var okResult = (OkNegotiatedContentResult<SessionModel>) response;

            okResult.Content.Links.Count.Should().BeGreaterOrEqualTo(1);

            urlHelper.Verify(
                h => h.Link("Session", It.Is<object>(o => o.GetPropertyValue<int>("sessionId") == sessionId)));
        }

        [Test]
        public void SessionsReturnedBySessionsShouldIncludeLinkToSelf()
        {
            const int numberOfSessions = 5;

            var sessionsReturnedByRepo = new Fixture().CreateMany<SessionModel>(numberOfSessions).ToList();
            var urlHelper = new Mock<UrlHelper>();

            var sessionsController = new SessionsControllerBuilder()
                .Returning(sessionsReturnedByRepo)
                .With(urlHelper)
                .Build();

            var response = sessionsController.Get();

            var okResult =
                (OkNegotiatedContentResult<List<SessionModel>>) response;

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
                .With(s => s.Links, new List<LinkModel>())
                .With(s => s.Id, speakerId)
                .Create();

            var sessionsReturnedByRepo = new Fixture()
                .Build<SessionModel>()
                .With(s => s.Links, new List<LinkModel>()).With(s => s.Id, 1)
                .With(s => s.Speakers, new[] { author }.ToList())
                .CreateMany(numberOfSessions)
                .ToList();

            var urlHelper = new Mock<UrlHelper>();

            var sessionsController = new SessionsControllerBuilder()
                .With(urlHelper).Returning(sessionsReturnedByRepo).Build();

            var response = sessionsController.Get();

            var okResult =
                (OkNegotiatedContentResult<List<SessionModel>>)response;

            okResult.Content.All(s => s.Speakers.Count() == 1).Should().BeTrue();

            urlHelper.Verify(
                h => h.Link("Speaker", It.Is<object>(o => o.GetPropertyValue<int>("speakerId") == speakerId)),
                Times.Exactly(numberOfSessions));
        }

        [Test]
        public void SessionsByDateShouldIncludeListOfAuthors()
        {
            const int numberOfSessions = 5;
            const int speakerId = 1;

            var author = new Fixture().Build<MinimalSpeakerModel>()
                .With(s => s.Links, new List<LinkModel>())
                .With(s => s.Id, speakerId)
                .Create();

            var sessionsReturnedByRepo = new Fixture()
                .Build<SessionModel>()
                .With(s => s.Links, new List<LinkModel>()).With(s => s.Id, 1)
                .With(s => s.Speakers, new[] { author }.ToList())
                .With(s => s.StartTime, DateTime.Now)
                .CreateMany(numberOfSessions)
                .ToList();

            var urlHelper = new Mock<UrlHelper>();

            var sessionsController = new SessionsControllerBuilder()
                .With(urlHelper).Returning(sessionsReturnedByRepo).Build();

            var response = sessionsController.Get(DateTime.Today);

            var okResult =
                (OkNegotiatedContentResult<List<SessionModel>>)response;

            okResult.Content.All(s => s.Speakers.Count() == 1).Should().BeTrue();

            urlHelper.Verify(
                h => h.Link("Speaker", It.Is<object>(o => o.GetPropertyValue<int>("speakerId") == speakerId)),
                Times.Exactly(numberOfSessions));
        }

        [Test]
        public void SessionShouldIncludeListOfAuthors()
        {
            const int sessionId = 1;
            const int speakerId = 1;

            var author = new Fixture().Build<MinimalSpeakerModel>()
                .With(s => s.Links, new List<LinkModel>())
                .With(s => s.Id, speakerId)
                .Create();

            var sessionReturnedByRepo =
                new Fixture().Build<SessionModel>()
                .With(s => s.Links, new List<LinkModel>()).With(s => s.Id, 1)
                .With(s => s.Speakers, new [] {author}.ToList())
                .Create();

            var urlHelper = new Mock<UrlHelper>();

            var sessionsController =
                new SessionsControllerBuilder().With(urlHelper).Returning(new[] { sessionReturnedByRepo }.ToList()).Build();

            var response = sessionsController.Get(sessionId);

            var okResult = (OkNegotiatedContentResult<SessionModel>)response;

            okResult.Content.Speakers.Should().HaveCount(1);
            okResult.Content.Speakers.First().Links.Should().HaveCount(1);
            okResult.Content.Speakers.First().Links.First().Rel.Should().Be("self");
            urlHelper.Verify(h => h.Link("Speaker", It.Is<object>(o => o.GetPropertyValue<int>("speakerId") == speakerId)));
        }

        [Test]
        public void SessionsByDateShouldIncludeAllSessionsOfThatDay()
        {
            const int todaySessionCount = 3;
            const int tomorrowSessionCount = 4;

            var sessionsOfToday = new Fixture().Build<SessionModel>()
                .With(s => s.StartTime, DateTime.Now)
                .CreateMany(todaySessionCount)
                .ToList();

            var sessionsOfTomorrow = new Fixture().Build<SessionModel>()
                .With(s => s.StartTime, DateTime.Now.AddDays(1))
                .CreateMany(tomorrowSessionCount)
                .ToList();

            var sessionsController = new SessionsControllerBuilder()
                .Returning(sessionsOfToday.Union(sessionsOfTomorrow).ToList())
                .Build();

            var response = sessionsController.Get(DateTime.Now);


            var okResult = (OkNegotiatedContentResult<List<SessionModel>>)response;

            okResult.Content.Should().HaveCount(todaySessionCount);
        }

        [Test]
        public void SessionsByDateShouldIncludeLinkToSelf()
        {
            const int numberOfSessions = 5;

            var sessionsReturnedByRepo =
                new Fixture().Build<SessionModel>()
                .With(s => s.StartTime, DateTime.Now)
                .CreateMany(numberOfSessions)
                .ToList();

            var urlHelper = new Mock<UrlHelper>();

            var sessionsController = new SessionsControllerBuilder()
                .Returning(sessionsReturnedByRepo)
                .With(urlHelper)
                .Build();

            var response = sessionsController.Get(DateTime.Now);

            var okResult =
                (OkNegotiatedContentResult<List<SessionModel>>)response;

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
        public void SessionsReturnedBySessionsShouldIncludeTags()
        {
            const int numberOfSessions = 5;
            const string tagName = "agile";

            var tag = new Fixture().Build<TagModel>()
                .Without(t => t.Links)
                .With(t => t.Name, tagName)
                .Create();

            var sessionsReturnedByRepo = new Fixture()
                .Build<SessionModel>()
                .With(s => s.Links, new List<LinkModel>()).With(s => s.Id, 1)
                .With(s => s.Tags, new[] { tag }.ToList())
                .CreateMany(numberOfSessions)
                .ToList();

            var urlHelper = new Mock<UrlHelper>();

            var sessionsController = new SessionsControllerBuilder()
                .With(urlHelper).Returning(sessionsReturnedByRepo).Build();

            var response = sessionsController.Get();

            var okResult =
                (OkNegotiatedContentResult<List<SessionModel>>)response;

            okResult.Content.All(s => s.Tags.Count() == 1).Should().BeTrue();

            urlHelper.Verify(
                h => h.Link("Tag", It.Is<object>(o => o.GetPropertyValue<string>("name") == tagName)),
                Times.Exactly(numberOfSessions));
        }

        [Test]
        public void SessionsByDateShouldIncludeTags()
        {
            const int numberOfSessions = 5;
            const string tagName = "agile";

            var tag = new Fixture().Build<TagModel>()
                .Without(t => t.Links)
                .With(t => t.Name, tagName)
                .Create();

            var sessionsReturnedByRepo = new Fixture()
                .Build<SessionModel>()
                .With(s => s.Links, new List<LinkModel>()).With(s => s.Id, 1)
                .With(s => s.Tags, new[] { tag }.ToList())
                .With(s => s.StartTime, DateTime.Now)
                .CreateMany(numberOfSessions)
                .ToList();

            var urlHelper = new Mock<UrlHelper>();

            var sessionsController = new SessionsControllerBuilder()
                .With(urlHelper).Returning(sessionsReturnedByRepo).Build();

            var response = sessionsController.Get(DateTime.Today);

            var okResult =
                (OkNegotiatedContentResult<List<SessionModel>>)response;

            okResult.Content.All(s => s.Tags.Count() == 1).Should().BeTrue();

            urlHelper.Verify(
                h => h.Link("Tag", It.Is<object>(o => o.GetPropertyValue<string>("name") == tagName)),
                Times.Exactly(numberOfSessions));
        }

        [Test]
        public void SessionShouldIncludeListOfTags()
        {
            const int sessionId = 1;
            const string tagName = "agile";

            var tag = new Fixture().Build<TagModel>()
                .Without(t => t.Links)
                .With(t => t.Name, tagName)
                .Create();

            var sessionReturnedByRepo =
                new Fixture().Build<SessionModel>()
                .With(s => s.Links, new List<LinkModel>()).With(s => s.Id, 1)
                .With(s => s.Tags, new[] { tag }.ToList())
                .Create();

            var urlHelper = new Mock<UrlHelper>();

            var sessionsController =
                new SessionsControllerBuilder().With(urlHelper).Returning(new[] { sessionReturnedByRepo }.ToList()).Build();

            var response = sessionsController.Get(sessionId);

            var okResult = (OkNegotiatedContentResult<SessionModel>)response;

            okResult.Content.Tags.Should().HaveCount(1);
            urlHelper.Verify(
                h => h.Link("Tag", It.Is<object>(o => o.GetPropertyValue<string>("name") == tagName)),
                Times.Once);
        }

        [Test]
        public void TagShouldReturnAllSessionsWithThatTag()
        {
            const int numberOfSessions = 5;

            var sessionsReturnedByRepo = new Fixture()
                .Build<SessionModel>()
                .With(s => s.Links, new List<LinkModel>()).With(s => s.Id, 1)
                .CreateMany(numberOfSessions)
                .ToList();

            var sessionsController = new SessionsControllerBuilder()
                .Returning(sessionsReturnedByRepo).Build();

            var response = sessionsController.Get(sessionsReturnedByRepo.First().Tags.First().Name);

            var okResult =
                (OkNegotiatedContentResult<List<SessionModel>>)response;

            okResult.Content.Should().HaveCount(1);

        }
    }
}