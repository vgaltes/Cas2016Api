using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Cas2016.Api.Models;
using Cas2016.Api.Repositories;

namespace Cas2016.Api.Controllers
{
    [RoutePrefix("Rooms")]
    public class RoomsController : ApiController
    {
        private readonly IRoomRepository _roomRepository;

        public RoomsController(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        [Route("", Name = "Rooms")]
        public IHttpActionResult Get()
        {
            var rooms = _roomRepository.GetAll();

            var roomsWithSelfLink = rooms.Select(AddSelfLinkTo);

            return Ok(roomsWithSelfLink);
        }

        [Route("{roomId}", Name = "Room")]
        public IHttpActionResult Get(int roomId)
        {
            var room = _roomRepository.Get(roomId);

            var roomWithSelfLink = AddSelfLinkTo(room);

            return Ok(roomWithSelfLink);
        }

        private RoomModel AddSelfLinkTo(RoomModel room)
        {
            var selfLink = ModelFactory.CreateLink(Url, "self", "Room", new { roomId = room.Id });
            room.Links = new List<LinkModel> { selfLink };

            return room;
        }
    }
}