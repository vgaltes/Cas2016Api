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
    public class SpeakersTests
    {
        [Test]
        public void SessionShouldIncludeAllSessionData()
        {
            const int numberOfSpeakers = 5;

            var speakersReturnedByRepo = new Fixture().CreateMany<SpeakerModel>(numberOfSpeakers);

            var speakersController = new SpeakersControllerBuilder().Returning(speakersReturnedByRepo).Build();

            var response = speakersController.Get();

            var okResult =
                (OkNegotiatedContentResult<IEnumerable<SpeakerModel>>) response;

            okResult.Content.Should().HaveCount(numberOfSpeakers);
            okResult.Content.ShouldBeEquivalentTo(speakersReturnedByRepo);
        }

        [Test]
        public void SessionShouldIncludeLinkToSelf()
        {
            const int speakerId = 1;

            var speakerReturnedByRepo =
                new Fixture().Build<SpeakerModel>().With(s => s.Links, new List<LinkModel>()).With(s => s.Id, 1).Create();

            var urlHelper = new Mock<UrlHelper>();

            var speakersController =
                new SpeakersControllerBuilder().With(urlHelper).Returning(new[] { speakerReturnedByRepo }).Build();


            var response = speakersController.Get(speakerId);

            var okResult = (OkNegotiatedContentResult<SpeakerModel>)response;

            okResult.Content.Links.Should().HaveCount(1);

            urlHelper.Verify(
                h => h.Link("Speaker", It.Is<object>(o => o.GetPropertyValue<int>("speakerId") == speakerId)));
        }

        [Test]
        public void SessionsReturnedBySessionsShouldIncludeLinkToSelf()
        {
            const int numberOfSessions = 5;

            var speakersReturnedByRepo = new Fixture().CreateMany<SpeakerModel>(numberOfSessions);
            var urlHelper = new Mock<UrlHelper>();

            var speakersController = new SpeakersControllerBuilder()
                .Returning(speakersReturnedByRepo)
                .With(urlHelper)
                .Build();

            var response = speakersController.Get();

            var okResult =
                (OkNegotiatedContentResult<IEnumerable<SpeakerModel>>)response;

            okResult.Content.All(s => s.Links.Any(l => l.Rel == "self")).Should().BeTrue();
            okResult.Content.All(s =>
            {
                urlHelper.Verify(
                    h => h.Link("Speaker", It.Is<object>(o => o.GetPropertyValue<int>("speakerId") == s.Id)));
                return true;
            }
            ).Should().BeTrue();
        }
    }
}