using System;
using System.Collections.Generic;

namespace Cas2016.Api.Models
{
    public class SessionModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public IEnumerable<MinimalSpeakerModel> Speakers { get; set; }

        public ICollection<LinkModel> Links { get; set; }
        public IEnumerable<TagModel> Tags { get; set; }
        public RoomModel Room { get; set; }
    }
}