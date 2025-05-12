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
    [Route("api/V1/[controller]")]
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
                return StatusCode(500, ResponseAPI<string>.Error($"An error occurred: {ex.Message}",500));
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
                    return StatusCode(204,ResponseAPI<string>.Error("No Posts Available",204));
                }
                return Ok(ResponseAPI<IEnumerable<PostDto>>.Success(posts, "Posts received successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseAPI<string>.Error($"An error occurred: {ex.Message}", 500));
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
                return StatusCode(500, ResponseAPI<string>.Error($"An error occurred: {ex.Message}", 500));
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
                    return NotFound(ResponseAPI<string>.Error("Post not found.",404));

                // Get current user ID and roles from token
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

                if (userId == null || roles.Count == 0)
                    return Unauthorized(ResponseAPI<string>.Error("Invalid token data.",401));

                //Check if user is the creator OR an admin
                if (roles.Contains("admin") || post.CharityId == userId)
                {
                    await _unitOfWork.postRepository.DeletePostAsync(id);
                    await _unitOfWork.SaveAsync();
                    return Ok(ResponseAPI<string>.Success("Post deleted successfully."));
                }

                return StatusCode(403, ResponseAPI<string>.Error("You are not allowed to delete this post.",403));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseAPI<string>.Error($"An error occurred: {ex.Message}", 500));
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
                    return NoContent();
                }

                return Ok(ResponseAPI<IEnumerable<PostDto>>.Success(posts, "Posts retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseAPI<string>.Error($"An error occurred: {ex.Message}", 500));
            }
        }


        [Authorize(Roles = "admin,charity")]
        [HttpPut("update-post/{id:int}")]
        public async Task<IActionResult> UpdatePost(int id,[FromForm] PostUpdateDto model)
        {
            try
            {
                if (model == null || id == 0)
                {
                    return BadRequest(ResponseAPI<string>.Error("Invalid post data."));
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();


                var post = await _unitOfWork.postRepository.GetByIdAsync(id);
                if (post == null)
                    return NotFound(ResponseAPI<string>.Error("Post not found."));

                if (!(roles.Contains("admin") || post.CharityId == userId))
                    return StatusCode(403, ResponseAPI<string>.Error("You are not allowed to update this post.",403));

                var updated = await _unitOfWork.postRepository.UpdatePostAsync(id,model);
                if (!updated)
                    return StatusCode(500, ResponseAPI<string>.Error("Failed to update post.",500));

                return Ok(ResponseAPI<string>.Success("Post updated successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseAPI<string>.Error($"An error occurred: {ex.Message}", 500));
            }
        }



    }
}
