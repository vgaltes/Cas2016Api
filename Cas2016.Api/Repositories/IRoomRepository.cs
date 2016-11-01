using System.Collections.Generic;
using Cas2016.Api.Models;

namespace Cas2016.Api.Repositories
{
    public interface IRoomRepository
    {
        IEnumerable<RoomModel> GetAll();
        RoomModel Get(int roomId);
    }
}