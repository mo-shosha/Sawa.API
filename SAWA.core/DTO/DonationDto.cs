using Microsoft.AspNetCore.Http;
using SAWA.core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAWA.core.DTO
{
    public record DonationDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? Amount { get; set; }
    }

    public record MonetaryDonationRequestDto
    {
        public string CharityUserName { get; set; }
        public int Amount { get; set; } 
    }


    public class ItemDonationRequestDto
    {
        public string CharityUserName { get; set; }
        public string Description { get; set; }
        public IFormFileCollection Photos { get; set; }
        public string Address { get; set; }

        public string Phone { get; set; }
    }

    public record DonationUpdateStatusDto
    {
        public int DonationId { get; set; }
        public string NewStatus { get; set; }
    }
}
