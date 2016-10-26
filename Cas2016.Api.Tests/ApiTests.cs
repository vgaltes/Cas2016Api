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
            }
        }

        [SetUp]
        public void RunBeforeAnyTestsInThisAssembly()
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
                scriptsBasePath + "InsertSessions.sql"
            };

            dbInitialiser.Seed(scriptFilePaths);
        }
    }
}