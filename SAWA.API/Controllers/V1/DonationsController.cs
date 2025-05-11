using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAWA.API.Healper;
using SAWA.core.DTO;
using SAWA.core.Interfaces;
using SAWA.core.IServices;
using SAWA.core.Models;
using Stripe;
using System.Security.Claims;

namespace SAWA.API.Controllers.V1
{
    [Route("api/V1/[controller]")]
    [ApiController]
    public class DonationsController : ControllerBase
    {
        private readonly IStripeService _stripeService;
        private readonly IUnitOfWork _unitOfWork;

        public DonationsController(IStripeService stripeService, IUnitOfWork unitOfWork)
        {
            _stripeService = stripeService;
            _unitOfWork = unitOfWork;
        }

        #region Helpers

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);
        private string GetUserEmail() => User.FindFirstValue(ClaimTypes.Email);

        #endregion

        [Authorize(Roles = "user")]
        [HttpPost("monetary")]
        public async Task<IActionResult> DonateMoney([FromBody] MonetaryDonationRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ResponseAPI<string>.Error("Invalid donation data."));

            var userId = GetUserId();
            var email = GetUserEmail();

            if (userId == null || email == null)
                return Unauthorized(ResponseAPI<string>.Error("Unauthorized user."));

            try
            {
                await _unitOfWork.donationRepository.AddDonationsAsync(dto, userId);

                var stripeDto = new DonationRequestDto
                {
                    DonorEmail = email,
                    AmountInCents = (long)(dto.Amount * 100)
                };

                var session = await _stripeService.CreateDonationSessionAsync(stripeDto);

                return Ok(ResponseAPI<object>.Success(new { url = session.Url }, "Stripe session created."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseAPI<string>.Error($"An error occurred: {ex.Message}"));
            }
        }

        [Authorize(Roles = "user")]
        [HttpPost("item")]
        public async Task<IActionResult> DonateItem([FromForm] ItemDonationRequestDto dto)
        {
            var userId = GetUserId();

            if (userId == null)
                return Unauthorized(ResponseAPI<string>.Error("Unauthorized user."));

            try
            {
                await _unitOfWork.donationRepository.AddDonationsAsync(dto, userId);
                return Ok(ResponseAPI<string>.Success("Item donation submitted successfully and pending approval."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseAPI<string>.Error($"An error occurred while submitting the item donation: {ex.Message}"));
            }
        }

        [Authorize(Roles = "charity")]
        [HttpGet("charity")]
        public async Task<IActionResult> GetCharityDonations()
        {
            var charityId = GetUserId();

            if (charityId == null)
                return Unauthorized(ResponseAPI<string>.Error("Unauthorized charity."));

            try
            {
                var donations = await _unitOfWork.donationRepository.GetDonationsByCharityIdAsync(charityId);
                return Ok(ResponseAPI<IEnumerable<DonationDto>>.Success(donations, "Charity donations retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseAPI<string>.Error($"An error occurred: {ex.Message}"));
            }
        }

        [Authorize(Roles = "user")]
        [HttpGet("user")]
        public async Task<IActionResult> GetUserDonations()
        {
            var userId = GetUserId();

            if (userId == null)
                return Unauthorized(ResponseAPI<string>.Error("Unauthorized user."));

            try
            {
                var donations = await _unitOfWork.donationRepository.GetDonationsByUserIdAsync(userId);
                return Ok(ResponseAPI<IEnumerable<DonationDto>>.Success(donations, "User donations retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseAPI<string>.Error($"An error occurred: {ex.Message}"));
            }
        }


       
    }
}
