using System.Collections.Generic;

namespace Cas2016.Api.Models
{
    public class MinimalSpeakerModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<LinkModel> Links { get; set; }
    }
}