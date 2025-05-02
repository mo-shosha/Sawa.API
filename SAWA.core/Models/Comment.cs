using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAWA.core.Models
{
    public class Comment:BaseEntity<int>
    {
        public string UserId { get; set; }
        public AppUser User { get; set; } // The user who commented
        public int PostId { get; set; }
        public Post Post { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
