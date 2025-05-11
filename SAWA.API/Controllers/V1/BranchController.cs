using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using SAWA.API.Healper;
using SAWA.core.DTO;
using SAWA.core.Interfaces;
using SAWA.core.IServices;
using SAWA.core.Models;
using System.Security.Claims;

namespace SAWA.API.Controllers.V1
{
    [Route("api/V1/[controller]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;
        public BranchController(IUnitOfWork unitOfWork,IAuthService authService)
        {
            _unitOfWork = unitOfWork;
            _authService = authService;
        }

        [Authorize(Roles = "charity")]
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromForm] BranchCreateDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(ResponseAPI<string>.Error("Invalid data."));

                var CharityId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(CharityId))
                    return Unauthorized(ResponseAPI<string>.Error("Invalid token."));


                await _unitOfWork.branchesRepository.CreateBranchAsync(dto, CharityId);
                return Ok(ResponseAPI<string>.Success("Branch created successfully."));
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
                var branches = await _unitOfWork.branchesRepository.GetBranchesWithPhotosAsync();
                if (branches == null || !branches.Any())
                    return StatusCode(204, ResponseAPI<string>.Error("No branches available."));

                return Ok(ResponseAPI<IEnumerable<BranchDto>>.Success(branches, "Branches retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseAPI<string>.Error($"An error occurred: {ex.Message}",500));
            }
        }

        [HttpGet("GetCharityBranches/{UserName}")]
        public async Task<IActionResult> GetCharityBranches(string UserName)
        {
            try
            {
                var charity = await _authService.GetCharityByUserName(UserName);
                if (charity == null)
                    return NotFound(ResponseAPI<string>.Error("Charity not found."));

                var branches = await _unitOfWork.branchesRepository.GetCharitybranchsAsync(charity.Id);
                if (branches == null || !branches.Any())
                    return StatusCode(204, ResponseAPI<string>.Error("No branches available."));

                return Ok(ResponseAPI<IEnumerable<BranchDto>>.Success(branches, "Branches retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseAPI<string>.Error($"An error occurred: {ex.Message}", 500));
            }
        }


        [Authorize(Roles = "charity")]
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] BranchUpdateDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(ResponseAPI<string>.Error("Invalid data."));

                var branch = await _unitOfWork.branchesRepository.GetByIdAsync(id);

                // Get current user ID and roles from token
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

                if (userId == null || roles.Count == 0)
                    return Unauthorized(ResponseAPI<string>.Error("Invalid token data."));

                //Check if user is the creator OR an admin
                if (roles.Contains("admin") || branch.CharityId == userId)
                {
                    await _unitOfWork.branchesRepository.UpdateBranchAsync(id, dto);
                    return Ok(ResponseAPI<string>.Success("Branch updated successfully."));
                }
                return StatusCode(403, ResponseAPI<string>.Error("You are not allowed to Update this branch."));
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

                var branch =await _unitOfWork.branchesRepository.GetByIdAsync(id);


                // Get current user ID and roles from token
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

                if (userId == null || roles.Count == 0)
                    return Unauthorized(ResponseAPI<string>.Error("Invalid token data."));

                //Check if user is the creator OR an admin
                if (roles.Contains("admin") || branch.CharityId == userId)
                {
                    var result = await _unitOfWork.branchesRepository.DeleteBrachAsync(id);

                    if (!result)
                        return BadRequest(ResponseAPI<string>.Error("Failed to delete branch or its photos."));
                    return Ok(ResponseAPI<string>.Success("Branch deleted successfully."));
                }

                return StatusCode(403, ResponseAPI<string>.Error("You are not allowed to delete this branch."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseAPI<string>.Error($"An error occurred: {ex.Message}", 500));
            }
        }

        
    }

}
