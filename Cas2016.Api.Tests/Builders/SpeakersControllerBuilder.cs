using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Routing;
using Cas2016.Api.Controllers;
using Cas2016.Api.Models;
using Cas2016.Api.Repositories;
using Moq;

namespace Cas2016.Api.Tests.Builders
{
    internal class SpeakersControllerBuilder
    {
        private readonly Mock<ISpeakerRepository> _speakerRepository;
        private IEnumerable<SpeakerModel> _speakersReturned;
        private Mock<UrlHelper> _urlHelper;

        public SpeakersControllerBuilder()
        {
            _urlHelper = new Mock<UrlHelper>();
            _speakerRepository = new Mock<ISpeakerRepository>();
            _speakersReturned = new List<SpeakerModel>();
        }

        public SpeakersControllerBuilder With(Mock<UrlHelper> urlHelper)
        {
            _urlHelper = urlHelper;
            return this;
        }

        public SpeakersControllerBuilder Returning(IEnumerable<SpeakerModel> speakersReturnedByRepo)
        {
            _speakerRepository.Setup(r => r.GetAll()).Returns(speakersReturnedByRepo);
            _speakerRepository.Setup(r => r.Get(It.IsAny<int>()))
                .Returns((int id) => _speakersReturned.Single(s => s.Id == id));
            _speakersReturned = speakersReturnedByRepo;

            return this;
        }

        public SpeakersController Build()
        {
            var speakersController = new SpeakersController(_speakerRepository.Object) {Url = _urlHelper.Object};

            return speakersController;
        }
    }
}