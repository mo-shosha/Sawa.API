using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SAWA.API.Healper;
using SAWA.core.DTO;
using SAWA.core.IServices;

namespace SAWA.API.Controllers.V1
{
    [Route("api/V1/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAuthService _adminService;

        public AdminController(IAuthService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("pending-charities")]
        public async Task<IActionResult> GetPendingCharities()
        {
            try
            {
                var result = await _adminService.GetPendingCharitiesAsync();
                return Ok(ResponseAPI<object>.Success(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseAPI<string>.Error(ex.Message));
            }
        }


        [HttpGet("charities")]
        public async Task<IActionResult> GetAllCharities()
        {
            try
            {
                var result = await _adminService.GetAllCharitiesAsyncForAdmin();
                return Ok(ResponseAPI<object>.Success(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseAPI<string>.Error(ex.Message));
            }
        }

       

        [HttpPut("accept-charity/{id}")]
        public async Task<IActionResult> AcceptCharity(string id)
        {
            try
            {
                var success = await _adminService.AcceptCharityAsync(id);
                if (!success)
                    return NotFound(ResponseAPI<string>.Error("Charity not found or not in the 'Charity' role."));

                return Ok(ResponseAPI<string>.Success("Charity approved."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseAPI<string>.Error(ex.Message));
            }
        }


        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _adminService.GetAllUsersAsync();
                return Ok(ResponseAPI<object>.Success(users));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseAPI<string>.Error(ex.Message,500));
            }
        }


        [HttpDelete("user/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var success = await _adminService.DeleteUserAsync(id);
                if (!success)
                    return NotFound(ResponseAPI<string>.Error("User not found.",404));

                return Ok(ResponseAPI<string>.Success("User deleted."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseAPI<string>.Error(ex.Message));
            }
        }


        [HttpPut("user-role")]
        public async Task<IActionResult> UpdateUserRole([FromBody] UpdateUserRoleDto dto)
        {
            try
            {
                var success = await _adminService.UpdateUserRoleAsync(dto);
                if (!success)
                    return BadRequest(ResponseAPI<string>.Error("Role update failed."));

                return Ok(ResponseAPI<string>.Success("Role updated successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseAPI<string>.Error(ex.Message));
            }
        }
    
    
    }
}
