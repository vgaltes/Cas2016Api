using System;
using System.Collections.Generic;

namespace Cas2016.Api.Models
{
    public class SessionModel : MinimalSessionModel
    {
        public string Description { get; set; }
        public int Duration { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public List<MinimalSpeakerModel> Speakers { get; set; }
        public List<TagModel> Tags { get; set; }
        public RoomModel Room { get; set; }
        public bool IsPlenary { get; set; }
    }

    
}