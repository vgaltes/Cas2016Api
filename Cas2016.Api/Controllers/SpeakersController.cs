using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Cas2016.Api.Models;
using Cas2016.Api.Repositories;

namespace Cas2016.Api.Controllers
{
    [RoutePrefix("Speakers")]
    public class SpeakersController : ApiController
    {
        private readonly ISpeakerRepository _speakerRepository;

        public SpeakersController(ISpeakerRepository speakerRepository)
        {
            _speakerRepository = speakerRepository;
        }

        [Route("", Name = "Speakers")]
        public IHttpActionResult Get()
        {
            var speakers = _speakerRepository.GetAll();

            var speakersWithSelfLink = speakers.Select(AddSelfLinkTo);

            return Ok(speakersWithSelfLink);
        }

        [Route("{speakerId}", Name = "Speaker")]
        public IHttpActionResult Get(int speakerId)
        {
            var speaker = _speakerRepository.Get(speakerId);

            var speakerWithSelfLink = AddSelfLinkTo(speaker);

            return Ok(speakerWithSelfLink);
        }

        private SpeakerModel AddSelfLinkTo(SpeakerModel speaker)
        {
            var selfLink = ModelFactory.CreateLink(Url, "self", "Speaker", new { speakerId = speaker.Id });
            speaker.Links = new List<LinkModel> { selfLink };

            return speaker;
        }
    }
}