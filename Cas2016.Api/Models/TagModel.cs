using System.Collections.Generic;
using System.Web.Http.Routing;

namespace Cas2016.Api.Models
{
    public class TagModel
    {
        public string Name { get; set; }
        public ICollection<LinkModel> Links { get; set; }
    }

    public static class TagModelExtensions
    {
        public static void AddSelfLink(this TagModel tag, UrlHelper url)
        {
            var selfLink = ModelFactory.CreateLink(url, "self", "Tag", new { name = tag.Name });
            tag.Links = new List<LinkModel> { selfLink };
        }
    }
}