using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAWA.core.Models
{
    public class Branch : BaseEntity<int>
    {
        public string CharityId { get; set; }  // This refers to the User who is the charity
        public AppUser Charity { get; set; }   // The User (Charity) to which the branch belongs
        public string Description { get; set; }
        public string Address { get; set; }
        public List<Photo> Photos { get; set; } = new List<Photo>();
        public DateTime CreatedAt { get; set; }
    }
}
