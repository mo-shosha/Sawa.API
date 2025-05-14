using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAWA.core.DTO
{
    public record BranchDto
    {
        public int Id { get; init; }
        public string PhoneNumber { get; init; }
        public string Description { get; init; }
        public string Address { get; init; }
        public List<PhotoDto> photos { get; init; }
    }
    public record BranchCreateDto
    {
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public IFormFileCollection? Photos { get; set; }

    }

    public record BranchUpdateDto
    {
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public IFormFileCollection? NewPhotos { get; set; }
    }


}
