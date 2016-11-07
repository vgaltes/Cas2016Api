using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Cas2016.Api.Models;
using Cas2016.Api.Repositories;

namespace Cas2016.Api.Controllers
{
    [RoutePrefix("rooms")]
    public class RoomsController : ApiController
    {
        private readonly IRoomRepository _roomRepository;
        private readonly ISessionRepository _sessionRepository;

        public RoomsController(IRoomRepository roomRepository, ISessionRepository sessionRepository)
        {
            _roomRepository = roomRepository;
            _sessionRepository = sessionRepository;
        }

        [Route("", Name = "Rooms")]
        public IHttpActionResult Get()
        {
            var rooms = _roomRepository.GetAll();

            rooms.ForEach(r => r.AddSelfLink(Url));

            var sessions = _sessionRepository.GetAll();

            foreach (var room in rooms)
            {
                room.Sessions = sessions.Where(s => s.Room.Id == room.Id).Select(ConvertToMinimalSessionModel);
                room.Sessions = room.Sessions.Select(AddSelfLinkTo);
            }

            return Ok(rooms);
        }

        [Route("{roomId}", Name = "Room")]
        public IHttpActionResult Get(int roomId)
        {
            var room = _roomRepository.Get(roomId);

            var roomWithSelfLink = AddSelfLinkTo(room);

            var sessions = _sessionRepository.GetAll();
            room.Sessions = sessions.Where(s => s.Room.Id == roomId).Select(ConvertToMinimalSessionModel);
            room.Sessions = room.Sessions.Select(AddSelfLinkTo);
            return Ok(roomWithSelfLink);
        }

        private MinimalSessionModel ConvertToMinimalSessionModel(SessionModel session)
        {
            return new MinimalSessionModel
            {
                Id = session.Id,
                Title = session.Title
            };
        }

        private RoomModel AddSelfLinkTo(RoomModel room)
        {
            var selfLink = ModelFactory.CreateLink(Url, "self", "Room", new { roomId = room.Id });
            room.Links = new List<LinkModel> { selfLink };

            return room;
        }

        private MinimalSessionModel AddSelfLinkTo(MinimalSessionModel session)
        {
            var selfLink = ModelFactory.CreateLink(Url, "self", "Session", new { sessionId = session.Id });
            session.Links = new List<LinkModel> { selfLink };

            return session;
        }
    }
}