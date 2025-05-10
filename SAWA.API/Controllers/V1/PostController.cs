using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAWA.API.Healper;
using SAWA.API.Mapping;
using SAWA.core.DTO;
using SAWA.core.Interfaces;
using SAWA.infrastructure.Repositories;
using System.Security.Claims;

namespace SAWA.API.Controllers.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public PostController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize(Roles = "charity")]
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] PostCreateDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(ResponseAPI<string>.Error("Invalid data."));
                }

                if (dto.Photos == null || dto.Photos.Count == 0)
                {
                    return BadRequest(ResponseAPI<string>.Error("No photos provided."));
                }
                var CharityId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                await _unitOfWork.postRepository.CreatePostAsync(dto, CharityId);
                return Ok(ResponseAPI<string>.Success("Post created successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseAPI<string>.Error($"An error occurred: {ex.Message}"));
            }
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var posts = await _unitOfWork.postRepository.GetAllPostsWithPhotosAndCommentsAsync();
                if (posts == null)
                {
                    return StatusCode(204,ResponseAPI<string>.Error("No Posts Available"));
                }
                return Ok(ResponseAPI<IEnumerable<PostDto>>.Success(posts, "Posts received successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseAPI<string>.Error($"An error occurred: {ex.Message}"));
            }
        }


        [HttpGet("Get/{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (id == 0 || id == null)
                {
                    return BadRequest(ResponseAPI<string>.Error($"Invalid Id with value : {id}"));
                }
                var post = await _unitOfWork.postRepository.GetPostWithPhotosAndCommentsAsync(id);
                if (post == null)
                {
                    return BadRequest(ResponseAPI<string>.Error("Post Not found"));
                }
                return Ok(ResponseAPI<PostDto>.Success(post, "Post received successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseAPI<string>.Error($"An error occurred: {ex.Message}"));
            }
        }

        [Authorize(Roles = "admin,charity")]
        [HttpDelete("Delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id == 0 || id == null)
                {
                    return BadRequest(ResponseAPI<string>.Error($"Invalid Id with value : {id}"));
                }
                var post = await _unitOfWork.postRepository.GetByIdAsync(id);
                if (post == null)
                    return NotFound(ResponseAPI<string>.Error("Post not found."));

                // Get current user ID and roles from token
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

                if (userId == null || roles.Count == 0)
                    return Unauthorized(ResponseAPI<string>.Error("Invalid token data."));

                //Check if user is the creator OR an admin
                if (roles.Contains("admin") || post.CharityId == userId)
                {
                    await _unitOfWork.postRepository.DeletePostAsync(id);
                    await _unitOfWork.SaveAsync();
                    return Ok(ResponseAPI<string>.Success("Post deleted successfully."));
                }

                return StatusCode(403, ResponseAPI<string>.Error("You are not allowed to delete this post."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseAPI<string>.Error($"An error occurred: {ex.Message}"));
            }
        }


        [HttpGet("charity/{username}")]
        public async Task<IActionResult> GetCharityPosts(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    return BadRequest(ResponseAPI<string>.Error("Username is required."));
                }

                var posts = await _unitOfWork.postRepository.GetCharityPostsWithPhotosAndCommentsAsync(username);

                if (posts == null || !posts.Any())
                {
                    return NotFound(ResponseAPI<string>.Error($"No posts found for charity with username: {username}"));
                }

                return Ok(ResponseAPI<IEnumerable<PostDto>>.Success(posts, "Posts retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseAPI<string>.Error($"An error occurred: {ex.Message}"));
            }
        }


    }
}
