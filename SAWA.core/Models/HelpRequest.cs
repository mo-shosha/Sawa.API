using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAWA.core.Models
{

    public enum HelpRequestStatus
    {
        Pending,
        Approved,
        Cancelled
    }
    public class HelpRequest : BaseEntity<int>
    {
        public string UserId { get; set; }
        public AppUser User { get; set; } // The user requesting help
        public string CharityId { get; set; }
        public AppUser Charity { get; set; } // The charity providing the help
        public string Description { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public List<Photo> Photos { get; set; } = new List<Photo>();
        public DateTime CreatedAt { get; set; }
        public HelpRequestStatus Status { get; set; }
    }
}
