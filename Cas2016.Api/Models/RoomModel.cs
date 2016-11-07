using System.Collections.Generic;
using System.Web.Http.Routing;

namespace Cas2016.Api.Models
{
    public class RoomModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public ICollection<LinkModel> Links { get; set; }
        public IEnumerable<MinimalSessionModel> Sessions { get; set; }
    }

    public static class RoomModelExtensions
    {
        public static void AddSelfLink(this RoomModel room, UrlHelper url)
        {
            var selfLink = ModelFactory.CreateLink(url, "self", "Room", new { roomId = room.Id });
            room.Links = new List<LinkModel> { selfLink };
        }
    }
}