using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAWA.core.DTO
{

    public record DonationRequestDto
    {
        public string DonorEmail { get; set; }
        public long AmountInCents { get; set; } 
    }



}
