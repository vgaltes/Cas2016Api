using System.Collections.Generic;

namespace Cas2016.Api.Models
{
    public class MinimalSessionModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<LinkModel> Links { get; set; }
    }
}