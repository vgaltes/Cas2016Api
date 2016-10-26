using System.Web.Http;

namespace Cas2016.Api.Controllers
{
    public class DefaultController : ApiController
    {
        public IHttpActionResult Get()
        {
            return Ok();
        }
    }
}