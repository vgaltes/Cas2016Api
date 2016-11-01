using System.Collections.Generic;

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
}