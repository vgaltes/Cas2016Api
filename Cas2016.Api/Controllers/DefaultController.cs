using System.Web.Http;
using Cas2016.Api.Models;

namespace Cas2016.Api.Controllers
{
    [RoutePrefix("")]
    public class DefaultController : ApiController
    {
        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok(new
            {
                Links = new[]
                {
                    ModelFactory.CreateLink(Url, "sessions", "Sessions", new {}),
                    ModelFactory.CreateLink(Url, "speakers", "Speakers", new {}),
                    ModelFactory.CreateLink(Url, "rooms", "Rooms", new {}),
                    ModelFactory.CreateLink(Url, "tags", "Tags", new {})
                }
            });
        }
    }
}