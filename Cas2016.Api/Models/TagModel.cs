using System.Collections.Generic;

namespace Cas2016.Api.Models
{
    public class TagModel
    {
        public string Name { get; set; }
        public ICollection<LinkModel> Links { get; set; }
    }
}