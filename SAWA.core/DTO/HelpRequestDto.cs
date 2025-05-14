using Microsoft.AspNetCore.Http;
using SAWA.core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAWA.core.DTO
{
    public record HelpRequestDto
    {
        public int Id { get; set; }
        public string UserFullName { get; set; }
        public string CharityName { get; set; }
        public string Description { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public List<string> PhotoUrls { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
    }

    public record HelpRequestCreateDto
    {
        public string CharityUserName { get; set; }
        public string Description { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public IFormFileCollection Photos { get; set; }
    }

    public record HelpRequestUpdateStatusDto
    {
        public int RequestId { get; set; }
        public string NewStatus { get; set; }
    }

    public record HelpRequestResponse
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public List<string> PhotoUrls { get; set; } = new();
    }
    public record HelpRequestUserResponse: HelpRequestResponse // return in control panels for user
    {
        public string CharityName { get; set; }
    }

    public record HelpRequestCharityResponse: HelpRequestResponse // return in control panels for Charity
    {
        public string UserName { get; set; }
        
    }

}
