using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
    public class PostRepository : GenericRepository<Post>, IPostRepository
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly IFileManagementService _fileManagementService;
        public PostRepository(AppDbContext db,IMapper mapper,IFileManagementService fileManagementService) : base(db)
        {
            _db = db;
            _mapper = mapper;
            _fileManagementService = fileManagementService;
        }

        public async Task CreatePostAsync(PostCreateDto model,string CharityId)
        {
            var post = _mapper.Map<Post>(model);
            post.CreatedAt = DateTime.Now;
            post.CharityId = CharityId; 
            await _db.Posts.AddAsync(post);
            await _db.SaveChangesAsync(); 
            if (model.Photos != null && model.Photos.Any())
            {
                var photoUrls = await _fileManagementService.AddImagesAsync(model.Photos, "Posts");

                foreach (var imageUrl in photoUrls)
                {
                    var photo = new Photo
                    {
                        ImageUrl = imageUrl,
                        PostId = post.Id  
                    };
                    post.Photos.Add(photo);
                }

                await _db.SaveChangesAsync();
            }

            
            await _db.SaveChangesAsync(); 
        }

        public async Task<List<PostDto>> GetAllPostsWithPhotosAndCommentsAsync()
        {
            var posts = await _db.Posts
                    .Include(p => p.Photos)
                    .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                    .Include(p => p.Charity)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();
            if (posts == null)
                return null;
            return _mapper.Map<List<PostDto>>(posts);
        }

        public async Task<List<PostDto>> GetCharityPostsWithPhotosAndCommentsAsync(string userName)
        {
            var post = await _db.Posts
                .Include(p => p.Photos)
                .Include(p => p.Comments)
                .ThenInclude(c => c.User)
                .Include(p => p.Charity)
                .Where(p => p.Charity.UserName == userName)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            if (post == null)
                return null;

            return _mapper.Map<List<PostDto>>(post);
        }


        public async Task<PostDto> GetPostWithPhotosAndCommentsAsync(int id)
        {
            var post = await _db.Posts
                .Include(p => p.Photos)
                .Include(p => p.Comments)
                .Include(p => p.Charity)
                .FirstOrDefaultAsync(p => p.Id == id);  

            if (post == null)
                return null; 

            return _mapper.Map<PostDto>(post);
        }

        public async Task<bool> DeletePostAsync(int id)
        {
            var post = await _db.Posts
                .Include(p => p.Photos)
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
                return false;

            foreach (var photo in post.Photos)
            {
                 _fileManagementService.DeleteImageAsync(photo.ImageUrl);
            }

            _db.Photos.RemoveRange(post.Photos);

            _db.Comments.RemoveRange(post.Comments);

            _db.Posts.Remove(post);

            await _db.SaveChangesAsync();
            return true;
        }


    }
}
