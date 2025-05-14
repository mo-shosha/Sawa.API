using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAWA.API.Healper;
using SAWA.core.DTO;
using SAWA.core.Interfaces;
using SAWA.infrastructure.Repositories;
using System.Security.Claims;

[Route("api/V1/[controller]")]
[ApiController]
public class CommentController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public CommentController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

    [Authorize(Roles = "user")]
    [HttpPost("CreateComment")]
    public async Task<IActionResult> CreateComment([FromForm] CommentCreateDto model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string userId = GetUserId();
            var post = await _unitOfWork.postRepository.GetPostWithPhotosAndCommentsAsync(model.PostId);
            if (post == null)
            {
                return BadRequest("post not found");
            }
            var result = await _unitOfWork.CommentRepository.CreateCommentAsync(model, userId);

            if (result != "Success")
            {
                return StatusCode(500, new { Message = "Failed to create comment" });
            }

            return Ok(new { Message = "Comment created successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseAPI<string>.Error($"An error occurred: {ex.Message}", 500));
        }

    }

    [Authorize]
    [HttpDelete("DeleteComment/{id:int}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        if (id == 0 || id == null)
        {
            return BadRequest(ResponseAPI<string>.Error($"Invalid Id with value : {id}"));
        }
        try
        {
            var comment = await _unitOfWork.CommentRepository.GetByIdAsync(id);
            if (comment == null)
                return NotFound(ResponseAPI<string>.Error("Comment not found."));

            var userId = GetUserId();
            var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            if (comment.UserId != userId && !roles.Contains("admin"))
            {
                return StatusCode(403, ResponseAPI<string>.Error("You are not allowed to delete this comment.", 403));
            }
            var commentReports = await _unitOfWork.reportRepository
                        .GetAllAsync();

            var reportsToDelete = commentReports.Where(r => r.TargetId == id).ToList();

            foreach (var report in reportsToDelete)
            {
                await _unitOfWork.reportRepository.DeleteAsync(report.Id);
            }
            await _unitOfWork.SaveAsync();


            await _unitOfWork.CommentRepository.DeleteAsync(id);
            return Ok(ResponseAPI<string>.Success("Comment deleted successfully."));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseAPI<string>.Error($"An error occurred: {ex.Message}", 500));
        }
    }


    [Authorize(Roles = "user")]
    [HttpPut("EditComment/{id:int}")]
    public async Task<IActionResult> EditComment(CommentUpdateDto model, int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ResponseAPI<string>.Error("Invalid data."));
        }

        try
        {
            var comment = await _unitOfWork.CommentRepository.GetByIdAsync(id);
            if (comment == null)
            {
                return NotFound(ResponseAPI<string>.Error("Comment not found."));
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            if (comment.UserId != userId && !roles.Contains("admin"))
            {
                return StatusCode(403, ResponseAPI<string>.Error("You are not allowed to edit this comment."));
            }

            comment.Content = model.Content;

            await _unitOfWork.CommentRepository.UpdateAsync(comment);

            return Ok(ResponseAPI<string>.Success("Comment updated successfully."));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ResponseAPI<string>.Error($"An error occurred: {ex.Message}", 500));
        }
    }

}
