using AutoMapper;
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
    public class HelpRequestRepository : GenericRepository<HelpRequest>, IHelpRequestRepository
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly IFileManagementService _fileManagementService;
        private readonly UserManager<AppUser> _userManager;

        public HelpRequestRepository(AppDbContext db, IMapper mapper, IFileManagementService fileManagementService, UserManager<AppUser> userManager) : base(db)
        {
            _db = db;
            _mapper = mapper;
            _fileManagementService = fileManagementService;
            _userManager = userManager;
        }

        public async Task<HelpRequestDto> AddHelpRequestAsync(HelpRequestCreateDto createDto,string UserId)
        {
            var charity = await GetUserByUserNameAsync(createDto.CharityUserName);
            if (charity == null)
            {
                throw new KeyNotFoundException($"Charity with username {createDto.CharityUserName} not found.");
            }
            var helpRequest = _mapper.Map<HelpRequest>(createDto);

            helpRequest.CharityId = charity.Id;
            helpRequest.UserId = UserId;
            helpRequest.CreatedAt = DateTime.UtcNow;
            helpRequest.Status = HelpRequestStatus.Pending;

            await _db.HelpRequests.AddAsync(helpRequest);


            await _db.SaveChangesAsync();

            if (createDto.Photos != null && createDto.Photos.Count > 0)
            {
                var photoUrls = await _fileManagementService.AddImagesAsync(createDto.Photos,"HelpRequest");
                foreach (var imageUrl in photoUrls)
                {
                    var photo = new Photo
                    {
                        ImageUrl = imageUrl,
                        HelpRequestId = helpRequest.Id
                    };
                    helpRequest.Photos.Add(photo);
                }

                await _db.SaveChangesAsync();

            }

            
            await _db.SaveChangesAsync();

            var result = _mapper.Map<HelpRequestDto>(helpRequest);

            return result;
        }

        public async Task<IEnumerable<HelpRequestCharityResponse>> GetAllCharityHelpRequest(string charityId)
        {
            var helpRequests = await _db.HelpRequests
                .Include(hr => hr.User)
                .Include(hr => hr.Charity)
                .Where(hr => hr.CharityId == charityId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<HelpRequestCharityResponse>>(helpRequests);
        }

        public async Task<IEnumerable<HelpRequestUserResponse>> GetAllUserHelpRequest(string userId)
        {
            var helpRequests = await _db.HelpRequests
                .Include(hr => hr.User)
                .Include(hr => hr.Charity)
                .Where(hr => hr.UserId == userId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<HelpRequestUserResponse>>(helpRequests);
        }


        public async Task<HelpRequestDto> UpdateHelpRequestStatusAsync(HelpRequestUpdateStatusDto updateStatusDto)
        {
            var helpRequest = await _db.HelpRequests
                .FirstOrDefaultAsync(hr => hr.Id == updateStatusDto.RequestId);

            if (helpRequest == null)
            {
                throw new KeyNotFoundException("Help request not found.");
            }

            helpRequest.Status = updateStatusDto.NewStatus;

            _db.HelpRequests.Update(helpRequest);
            await _db.SaveChangesAsync();

            var result = _mapper.Map<HelpRequestDto>(helpRequest);
            return result;
        }

        private async Task<AppUser> GetUserByUserNameAsync(string username)
        {
            return await _userManager.Users
                                     .Where(u => u.UserName == username)
                                     .FirstOrDefaultAsync();
        }
    }
}

