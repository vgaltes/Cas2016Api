using System.Collections.Generic;
using System.Web.Http.Routing;

namespace Cas2016.Api.Models
{
    public class MinimalSpeakerModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public ICollection<LinkModel> Links { get; set; }
    }

    public static class MinimalSpeakerModelExtensions
    {
        public static void AddSelfLink(this MinimalSpeakerModel speaker, UrlHelper url)
        {
            var selfLink = ModelFactory.CreateLink(url, "self", "Speaker", new { speakerId = speaker.Id });
            speaker.Links.Add(selfLink);
        }
    }
}