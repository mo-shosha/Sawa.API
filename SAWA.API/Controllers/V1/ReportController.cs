using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAWA.API.Healper;
using SAWA.core.DTO;
using SAWA.core.Interfaces;
using System.Security.Claims;

namespace SAWA.API.Controllers.V1
{
    [Route("api/V1/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        [HttpPost("Create")]
        public async Task<IActionResult> CreateReport([FromBody] ReportCreateDto reportDto)
        {
            try
            {
                if (reportDto == null)
                    return BadRequest(ResponseAPI<string>.Error("Invalid report data."));

                // Get current user ID and roles from token
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

                if (userId == null || roles.Count == 0)
                    return Unauthorized(ResponseAPI<string>.Error("Invalid token data.", 401));

                await _unitOfWork.reportRepository.CreateReportAsync(reportDto, userId);

                return Ok(ResponseAPI<string>.Success("Report created successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseAPI<string>.Error($"An error occurred while creating the report: {ex.Message}", 500));
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet("All")]
        public async Task<IActionResult> GetAllReports()
        {
            try
            {
                var reports = await _unitOfWork.reportRepository.GetAllReportsAsync();
                return Ok(ResponseAPI<IEnumerable<ReportDto>>.Success(reports));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseAPI<string>.Error($"An error occurred while retrieving reports: {ex.Message}", 500));
            }
        }
    }
}
