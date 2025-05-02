using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAWA.core.Models
{
    public class Photo :BaseEntity<int>
    {
        public string ImageUrl { get; set; }
        public string? UserId { get; set; }
        public AppUser User { get; set; } 
        
        public int? BranchId { get; set; }
        public Branch Branch { get; set; }

        public int? PostId { get; set; }
        public Post Post { get; set; }

        public int? HelpRequestId { get; set; }
        public HelpRequest HelpRequest { get; set; }

        public int? DonationId { get; set; }
        public Donation Donation { get; set; }

    }
}

