using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Cas2016.Api.Models;
using FluentAssertions;
using Microsoft.Owin.Testing;
using NUnit.Framework;

namespace Cas2016.Api.Tests
{
    [TestFixture]
    public class ApiTests
    {
        [Test]
        public async Task DefaultRootShouldReturnOk()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var response = await server.HttpClient.GetAsync("/");

                response.IsSuccessStatusCode.Should().BeTrue();
            }
        }

        [Test]
        public async Task CallToSessionsShouldReturnTheSessions()
        {
            const int numberOfSessions = 5;

            using (var server = TestServer.Create<Startup>())
            {
                var response = await server.HttpClient.GetAsync("/Sessions");

                response.IsSuccessStatusCode.Should().BeTrue();
                var content = await response.Content.ReadAsAsync<IList<SessionModel>>();

                content.Should().HaveCount(numberOfSessions);
            }
        }

        [Test]
        public async Task CallToSessionShouldReturnTheSession()
        {
            const int sessionId = 1;

            using (var server = TestServer.Create<Startup>())
            {
                var response = await server.HttpClient.GetAsync($"/Sessions/{sessionId}");

                response.IsSuccessStatusCode.Should().BeTrue();
                var content = await response.Content.ReadAsAsync<SessionModel>();

                content.Id.Should().Be(sessionId);
                content.Speakers.Should().HaveCount(1);
            }
        }

        [Test]
        public async Task CallToSessionsWithADateShouldReturnTheSessionsOfThatDate()
        {
            const int expectedNumberOfSessions = 3;

            using (var server = TestServer.Create<Startup>())
            {
                var response = await server.HttpClient.GetAsync("/Sessions/2016-12-01");

                response.IsSuccessStatusCode.Should().BeTrue();
                var content = await response.Content.ReadAsAsync<IList<SessionModel>>();

                content.Should().HaveCount(expectedNumberOfSessions);
            }
        }

        [Test]
        public async Task CallToSpeakersShouldReturnTheSpeakers()
        {
            const int numberOfSpeakers = 2;

            using (var server = TestServer.Create<Startup>())
            {
                var response = await server.HttpClient.GetAsync("/Speakers");

                response.IsSuccessStatusCode.Should().BeTrue();
                var content = await response.Content.ReadAsAsync<IList<SpeakerModel>>();

                content.Should().HaveCount(numberOfSpeakers);
            }
        }

        [Test]
        public async Task CallToSpeakerShouldReturnTheSpeaker()
        {
            const int speakerId = 1;

            using (var server = TestServer.Create<Startup>())
            {
                var response = await server.HttpClient.GetAsync($"/Speakers/{speakerId}");

                response.IsSuccessStatusCode.Should().BeTrue();
                var content = await response.Content.ReadAsAsync<SpeakerModel>();

                content.Id.Should().Be(speakerId);
            }
        }

        [SetUp]
        public void SetUp()
        {
            var dbInitialiser = new DatabaseInitialiser();

            // ReSharper disable once ConvertToConstant.Local
            var shouldDropAndCreateDatabase = true;

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (!shouldDropAndCreateDatabase)
                return;

            dbInitialiser.Publish(true);

            var scriptsBasePath = AppDomain.CurrentDomain.BaseDirectory + @"\Scripts\";

            var scriptFilePaths = new[]
            {
                scriptsBasePath + "InsertSessions.sql",
                scriptsBasePath + "InsertSpeakers.sql"
            };

            dbInitialiser.Seed(scriptFilePaths);
        }
    }
}