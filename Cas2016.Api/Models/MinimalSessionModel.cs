using System.Collections.Generic;
using System.Web.Http.Routing;

namespace Cas2016.Api.Models
{
    public class MinimalSessionModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<LinkModel> Links { get; set; }
    }

    public static class MinimalSessionModelExtensions
    {   
        public static void AddSelfLink(this MinimalSessionModel session, UrlHelper url)
        {
            var selfLink = ModelFactory.CreateLink(url, "self", "Session", new { sessionId = session.Id });
            session.Links = new List<LinkModel> { selfLink };
        }
    }
}