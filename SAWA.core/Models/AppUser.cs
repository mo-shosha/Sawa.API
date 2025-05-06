using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAWA.core.Models
{
    public class AppUser: IdentityUser
    {
        //for user and charity
        public string FullName { get; set; }
        public string Address { get; set; }
        public string? ProfilePhotoURL { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        //for charity only
        public bool? IsApproved { get; set; }

        public string? Description { get; set; }

        public string? WallpaperPhotoURL { get; set; }
        public string? DocumentURL { get; set; }


        public ICollection<Donation> DonationsGiven { get; set; }  
        public ICollection<Donation> DonationsReceived { get; set; }

        public ICollection<HelpRequest> MyHelpRequests { get; set; } = new List<HelpRequest>();

        public ICollection<HelpRequest> ProvidedHelpRequests { get; set; } = new List<HelpRequest>();

        public ICollection<Comment> Comments { get; set; }
        public ICollection<Report> Reports { get; set; }
        public ICollection<Branch> Branches { get; set; }
        public ICollection<Post> Posts { get; set; }
    }
}
