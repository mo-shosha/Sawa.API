using Microsoft.AspNetCore.Mvc;
using SAWA.core.DTO;
using SAWA.core.IServices;
using SAWA.API.Healper;

namespace SAWA.API.Controllers.V1
{
    [Route("api/V1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ResponseAPI<string>.Error("Invalid model data."));

                var result = await _authService.LoginAsync(model);

                if (result == null)
                    return Unauthorized(ResponseAPI<string>.Error("Invalid email or password.", 401));

                return Ok(ResponseAPI<object>.Success(result, "Login successful."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseAPI<string>.Error($"Server error: {ex.Message}", 500));
            }
        }

        [HttpPost("RegisterCharity")]
        public async Task<IActionResult> RegisterCharity([FromForm] CharityRegisterDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ResponseAPI<string>.Error("Invalid model data."));

                var result = await _authService.RegisterCharityAsync(model);

                if (result == null)
                    return BadRequest(ResponseAPI<string>.Error("Error registering charity. Please check your details and try again."));

                return Ok(ResponseAPI<string>.Success(null, "Charity registered successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseAPI<string>.Error($"Server error: {ex.Message}", 500));
            }
        }

        [HttpPost("RegisterUser")]
        public async Task<IActionResult> RegisterUser([FromForm] UserRegisterDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ResponseAPI<string>.Error("Invalid model data."));

                var result = await _authService.RegisterUserAsync(model);

                if (result == null)
                    return BadRequest(ResponseAPI<string>.Error("Error registering user. Please check your details and try again."));

                return Ok(ResponseAPI<object>.Success(result, "User registered successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseAPI<string>.Error($"Server error: {ex.Message}", 500));
            }
        }
    }
}
