using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAWA.core.Models
{

    public enum DonationStatus
    {
        Pending,
        Approved,
        Cancelled
    }

    public enum DonationType
    {
        Monetary,  
        Product,  
    }
    public class Donation:BaseEntity<int>
    {
        public string UserId { get; set; }
        public AppUser User { get; set; } // The donor
        public string CharityId { get; set; }
        public AppUser Charity { get; set; } // The charity recipient
        public string Description { get; set; }
        public DonationType Type { get; set; }
        public DonationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Amount { get; set; }

        // Data for donor to receive product, not required if monetary
        public string? Address { get; set; }

        public List<Photo> Photos { get; set; } = new List<Photo>();
        public string? Phone { get; set; }
    }
}
