using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAWA.API.Healper;
using SAWA.core.IServices;

namespace SAWA.API.Controllers.V1
{
    [Route("api/V1/[controller]")]
    [ApiController]
    public class CharityController : ControllerBase
    {
        private readonly IAuthService _authService;
        public CharityController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("charities")]
        public async Task<IActionResult> GetAllCharities()
        {
            try
            {
                var charities = await _authService.GetAllCharitiesAsync();
                return Ok(ResponseAPI<object>.Success(charities, "Charities retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseAPI<string>.Error(ex.InnerException?.Message ?? ex.Message, 500));
            }
        }


        [HttpGet("get-by-username/{username}")]
        public async Task<IActionResult> GetCharityByUsername(string username)
        {
            try
            {
                var charity = await _authService.GetCharityByUserNameAsync(username);

                if (charity == null)
                    return NotFound(ResponseAPI<string>.Error("Charity not found."));

                return Ok(ResponseAPI<object>.Success(charity, "Charity retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseAPI<string>.Error(ex.InnerException?.Message ?? ex.Message, 500));
            }
        }





    }
}
