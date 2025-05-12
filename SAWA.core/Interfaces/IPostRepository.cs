using SAWA.core.DTO;
using SAWA.core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAWA.core.Interfaces
{
    public interface IPostRepository:IGenericRepository<Post>
    {
        Task CreatePostAsync(PostCreateDto model,string CharityId);
        Task<List<PostDto>> GetAllPostsWithPhotosAndCommentsAsync();
        Task<PostDto> GetPostWithPhotosAndCommentsAsync(int id);
        Task<List<PostDto>> GetCharityPostsWithPhotosAndCommentsAsync(string UserName);
        Task<bool> UpdatePostAsync(int id,PostUpdateDto model);
        Task<bool> DeletePostAsync(int id);
    }
}
