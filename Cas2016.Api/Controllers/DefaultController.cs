using System.Web.Http;

namespace Cas2016.Api.Controllers
{
    [RoutePrefix("")]
    public class DefaultController : ApiController
    {
        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok();
        }
    }
}