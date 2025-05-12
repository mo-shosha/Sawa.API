using SAWA.core.DTO;
using SAWA.core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAWA.core.Interfaces
{
    public interface IDonationRepository:IGenericRepository<Donation>
    {
        Task AddDonationsAsync(MonetaryDonationRequestDto donation,string UserId);
        Task AddDonationsAsync(ItemDonationRequestDto donation, string UserId);
        Task<IEnumerable<DonationDto>> GetDonationsByUserIdAsync(string userId);
        Task<IEnumerable<DonationDto>> GetDonationsByCharityIdAsync(string charityId);
        Task<DonationDto> UpdateDonationStatusAsync(DonationUpdateStatusDto updateStatusDto);
    }

}
