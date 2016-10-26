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
            InitializeDatabaseWithSessions(numberOfSessions);

            using (var server = TestServer.Create<Startup>())
            {
                var response = await server.HttpClient.GetAsync("/Sessions");

                response.IsSuccessStatusCode.Should().BeTrue();
                var content = await response.Content.ReadAsAsync<IList<SessionModel>>();

                content.Should().HaveCount(numberOfSessions);
            }
        }

        private void InitializeDatabaseWithSessions(int numberOfSessions)
        {
            

        }
    }
}