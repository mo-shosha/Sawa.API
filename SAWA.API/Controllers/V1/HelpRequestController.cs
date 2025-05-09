using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SAWA.API.Healper;
using SAWA.core.DTO;
using SAWA.core.Interfaces;
using System.Security.Claims;

namespace SAWA.API.Controllers.V1
{
    [Route("api/V1/[controller]")]
    [ApiController]
    public class HelpRequestController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;

        public HelpRequestController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        [HttpPost("CreateHelpRequest")]
        public async Task<IActionResult> CreateHelpRequest([FromForm] HelpRequestCreateDto createDto)
        {
            try
            {
                if (createDto == null)
                    return BadRequest(ResponseAPI<string>.Error("Invalid data."));

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

                if (userId == null || roles.Count == 0)
                    return Unauthorized(ResponseAPI<string>.Error("Invalid token data."));
                if (roles.Contains("Charity"))
                {
                    return StatusCode(403,ResponseAPI<string>.Error("Uou are not allow to make help request ",403));
                }
                var result = await _unitOfWork.helpRequestRepository.AddHelpRequestAsync(createDto, userId);
                return Ok(ResponseAPI<HelpRequestDto>.Success(result, "HelpRequest created successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseAPI<string>.Error($"An error occurred: {ex.Message}"));
            }
        }


        [Authorize]
        [HttpPut("UpdateHelpRequest")]
        public async Task<IActionResult> UpdateHelpRequest([FromBody] HelpRequestUpdateStatusDto updateStatusDt)
        {
            try
            {
                if (updateStatusDt == null)
                    return BadRequest(ResponseAPI<string>.Error("Invalid data."));

                if (updateStatusDt.RequestId <= 0)
                    return BadRequest(ResponseAPI<string>.Error($"Invalid Id : {updateStatusDt.RequestId}."));

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(ResponseAPI<string>.Error("Invalid user."));

                var helpRequest = await _unitOfWork.helpRequestRepository.GetByIdAsync(updateStatusDt.RequestId);
                if (helpRequest == null)
                    return NotFound(ResponseAPI<string>.Error("Help request not found."));

                bool isAdmin = roles.Contains("Admin");
                bool isOwner = helpRequest.UserId == userId;
                bool isCharityWhoAccepted = helpRequest.CharityId == userId;

                if (!isAdmin && !isOwner && !isCharityWhoAccepted)
                {
                    return StatusCode(403, ResponseAPI<string>.Error("You are not authorized to update this help request.", 403));
                }

                var result = await _unitOfWork.helpRequestRepository.UpdateHelpRequestStatusAsync(updateStatusDt);
                return Ok(ResponseAPI<HelpRequestDto>.Success(result, "HelpRequest status updated successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseAPI<string>.Error($"An error occurred: {ex.Message}"));
            }
        }

    }
}
