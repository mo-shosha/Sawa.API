using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAWA.core.Models
{
    public class Post : BaseEntity<int>
    {
        public string CharityId { get; set; }
        public AppUser Charity { get; set; } // The charity posting the content
        public List<Photo> Photos { get; set; } = new List<Photo>();
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
