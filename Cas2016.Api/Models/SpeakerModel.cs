using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cas2016.Api.Models
{
    public class SpeakerModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TwitterProfile { get; set; }
        public string LinkedinProfile { get; set; }
        public string Website { get; set; }
        public string Biography { get; set; }
        public string Image { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public ICollection<LinkModel> Links { get; set; }
    }
}