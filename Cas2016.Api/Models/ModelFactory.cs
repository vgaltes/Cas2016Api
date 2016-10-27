using System.Web.Http.Routing;

namespace Cas2016.Api.Models
{
    public static class ModelFactory
    {
        public static LinkModel CreateLink(UrlHelper urlHelper, string linkName, 
            string routeName, object routeValues, string method = "GET", 
            bool isTemplated = false)
        {
            return new LinkModel
            {
                Href = urlHelper.Link(routeName, routeValues),
                Rel = linkName,
                Method = method,
                IsTemplated = isTemplated
            };
        }
    }
}