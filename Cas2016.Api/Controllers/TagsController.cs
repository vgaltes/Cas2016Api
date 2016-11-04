using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Cas2016.Api.Models;
using Cas2016.Api.Repositories;

namespace Cas2016.Api.Controllers
{
    [RoutePrefix("tags")]
    public class TagsController : ApiController
    {
        private readonly ISessionRepository _sessionRepository;

        public TagsController(ISessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        [Route("", Name = "Tags")]
        public IHttpActionResult Get()
        {
            var sessions = _sessionRepository.GetAll();

            var tags = sessions.SelectMany(s => s.Tags).Distinct(new TagEqualityComparer());

            var tagsWithSelfLink = tags.Select(AddSelfLinkTo);

            return Ok(tagsWithSelfLink);
        }

        private TagModel AddSelfLinkTo(TagModel tag)
        {
            var selfLink = ModelFactory.CreateLink(Url, "self", "Tag", new { name = tag.Name });
            tag.Links = new List<LinkModel> { selfLink };

            return tag;
        }
    }

    internal class TagEqualityComparer : IEqualityComparer<TagModel>
    {
        public bool Equals(TagModel x, TagModel y)
        {
            return x.Name == y.Name;
        }

        public int GetHashCode(TagModel obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}