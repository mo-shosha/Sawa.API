using AutoMapper;
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
    public class BranchesRepository : GenericRepository<Branch>, IBranchesRepository
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly IFileManagementService _fileManagementService;
        public BranchesRepository(AppDbContext db, IMapper mapper, IFileManagementService fileManagementService) : base(db)
        {
            _db = db;
            _mapper = mapper;
            _fileManagementService = fileManagementService;
        }

        public async Task CreateBranchAsync(BranchCreateDto model,string CharityId)
        {
            var branch = _mapper.Map<Branch>(model);
            branch.CreatedAt = DateTime.UtcNow;
            branch.CharityId = CharityId;
            await _db.Branches.AddAsync(branch);

            if (model.Photos != null && model.Photos.Any())
            {
                var photoUrls = await _fileManagementService.AddImagesAsync(model.Photos, "branches");

                foreach (var imageUrl in photoUrls)
                {
                    var photo = new Photo
                    {
                        ImageUrl = imageUrl,
                        BranchId = branch.Id
                    };
                    branch.Photos.Add(photo);
                }

            }

            await _db.SaveChangesAsync();
        }

        public async Task<bool> DeleteBrachAsync(int id)
        {
            var branch = await _db.Branches
                .Include(b => b.Photos)
                .FirstOrDefaultAsync(b => b.Id == id);
            if (branch == null)
                throw new Exception("Branch not found");

            if (branch.Photos != null && branch.Photos.Any())
            {
                foreach (var photo in branch.Photos)
                {
                    _fileManagementService.DeleteImageAsync(photo.ImageUrl);
                }

                _db.Photos.RemoveRange(branch.Photos);
            }
            _db.Branches.Remove(branch);
            _db.SaveChanges();
            return true;

        }

        public async Task<IEnumerable<BranchDto>> GetBranchesWithPhotosAsync()
        {
            var branches = await _db.Branches
                .Include(b => b.Photos)
                .Include(b => b.Charity)
                .ToListAsync();

            return _mapper.Map<List<BranchDto>>(branches);
        }

        public async Task<IEnumerable<BranchDto>> GetCharitybranchsAsync(string CharityId)
        {
            var branches = await _db.Branches
                .Include(b => b.Photos)
                .Include(b => b.Charity)
                .Where(b => b.CharityId == CharityId)
                .ToListAsync();

            return _mapper.Map<List<BranchDto>>(branches);
        }

        public async Task UpdateBranchAsync(int branchId, BranchUpdateDto model)
        {
            var branch = await _db.Branches
                .Include(b => b.Photos)
                .FirstOrDefaultAsync(b => b.Id == branchId);

            if (branch == null)
                throw new Exception("Branch not found");

            _mapper.Map(model, branch);

            if (branch.Photos != null && branch.Photos.Any())
            {
                foreach (var photo in branch.Photos)
                {
                     _fileManagementService.DeleteImageAsync(photo.ImageUrl);  
                }

                _db.Photos.RemoveRange(branch.Photos);  
            }
            if (model.NewPhotos != null && model.NewPhotos.Any())
            {
                var photoUrls = await _fileManagementService.AddImagesAsync(model.NewPhotos, "branches");
                foreach (var url in photoUrls)
                {
                    branch.Photos.Add(new Photo
                    {
                        ImageUrl = url,
                        BranchId = branch.Id
                    });
                }
            }

            await _db.SaveChangesAsync();
        }

    }
}
