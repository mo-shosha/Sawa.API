using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SAWA.core.DTO;
using SAWA.core.Interfaces;
using SAWA.core.IServices;
using SAWA.core.Models;
using SAWA.infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAWA.infrastructure.Repositories
{
    public class DonationRepository : GenericRepository<Donation>, IDonationRepository
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly IFileManagementService _fileManagementService;
        private readonly UserManager<AppUser> _userManager;

        public DonationRepository(
            AppDbContext db,
            IMapper mapper,
            IFileManagementService fileManagementService,
            UserManager<AppUser> userManager) : base(db)
        {
            _db = db;
            _mapper = mapper;
            _fileManagementService = fileManagementService;
            _userManager = userManager;
        }

        public async Task AddDonationsAsync(MonetaryDonationRequestDto dto, string UserId)
        {
            var charity = await _userManager.FindByNameAsync(dto.CharityUserName);
            if (charity == null) throw new Exception("Charity not found.");

            var donation = _mapper.Map<Donation>(dto);
            donation.CharityId = charity.Id;
            donation.CreatedAt = DateTime.UtcNow;
            donation.Status = DonationStatus.Pending;
            donation.Type = DonationType.Monetary;
            donation.UserId = UserId;

            await _db.Donations.AddAsync(donation);
            await _db.SaveChangesAsync();
        }

        public async Task AddDonationsAsync(ItemDonationRequestDto dto, string UserId)
        {
            var charity = await _userManager.FindByNameAsync(dto.CharityUserName);
            if (charity == null) throw new Exception("Charity not found.");

            var donation = _mapper.Map<Donation>(dto);
            donation.CharityId = charity.Id;
            donation.CreatedAt = DateTime.UtcNow;
            donation.Status = DonationStatus.Pending;
            donation.Type = DonationType.Product;
            donation.UserId = UserId;

            await _db.Donations.AddAsync(donation);
            await _db.SaveChangesAsync();

            if (dto.Photos != null && dto.Photos.Any())
            {
                var photoUrls = await _fileManagementService.AddImagesAsync(dto.Photos, "donation");

                foreach (var imageUrl in photoUrls)
                {
                    var photo = new Photo
                    {
                        ImageUrl = imageUrl,
                        DonationId = donation.Id
                    };
                    donation.Photos.Add(photo);
                }

                await _db.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<DonationDto>> GetDonationsByCharityIdAsync(string charityId)
        {
            var donations = await _db.Donations
                .Where(d => d.CharityId == charityId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<DonationDto>>(donations);
        }

        public async Task<IEnumerable<DonationDto>> GetDonationsByUserIdAsync(string userId)
        {
            var donations = await _db.Donations
                .Where(d => d.UserId == userId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<DonationDto>>(donations);
        }

        public async Task<DonationDto> UpdateDonationStatusAsync(DonationUpdateStatusDto updateStatusDto)
        {
            var donation = _db.Donations
                    .FirstOrDefault(d => d.Id == updateStatusDto.DonationId);
            if (donation == null)
            {
                throw new KeyNotFoundException("Donation not found.");
            }
            if (!Enum.TryParse<DonationStatus>(updateStatusDto.NewStatus, true, out var newStatus))
            {
                throw new ArgumentException($"Invalid donation status: {updateStatusDto.NewStatus}");
            }

            donation.Status = newStatus;
            _db.Donations.Update(donation);
            await _db.SaveChangesAsync();

            var result = _mapper.Map<DonationDto>(donation);
            return result;
        }
    }

}
