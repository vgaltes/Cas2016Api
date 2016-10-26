using System.Threading.Tasks;
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
    }
}