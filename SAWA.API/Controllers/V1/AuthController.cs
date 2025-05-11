using Microsoft.AspNetCore.Mvc;
using SAWA.core.DTO;
using SAWA.core.IServices;
using SAWA.API.Healper;
using Microsoft.AspNetCore.Authorization;

namespace SAWA.API.Controllers.V1
{
    [Route("api/V1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model data during login attempt.");
                    return BadRequest(ResponseAPI<string>.Error("Invalid model data."));
                }

                var result = await _authService.LoginAsync(model);

                if (result == null)
                {
                    _logger.LogWarning("Invalid login attempt with email: {Email}", model.Email);
                    return Unauthorized(ResponseAPI<string>.Error("Invalid email or password.", 401));
                }

                _logger.LogInformation("User logged in successfully: {Email}", model.Email);
                return Ok(ResponseAPI<object>.Success(result, "Login successful."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login.");
                return StatusCode(500, ResponseAPI<string>.Error($"Server error: {ex.InnerException?.Message ?? ex.Message}", 500));
            }
        }

        [HttpPost("LoginWithGoogle")]
        public async Task<IActionResult> LoginWithGoogle([FromBody] GoogleLoginDto model)
        {
            if (string.IsNullOrEmpty(model.AccessToken))
            {
                _logger.LogWarning("Access token is missing during Google login attempt.");
                return Unauthorized(ResponseAPI<string>.Error("Access token is required.", 400));
            }

            var user = await _authService.LoginWithGoogleAsync(model.AccessToken);

            if (user == null)
            {
                _logger.LogWarning("No user found with Google account for access token: {AccessToken}", model.AccessToken);
                return Unauthorized(ResponseAPI<string>.Error("No user found with this Google account. Please register.", 401));
            }

            _logger.LogInformation("User logged in successfully with Google account.");
            return Ok(ResponseAPI<object>.Success(user, "Login successful."));
        }

        [HttpPost("RegisterCharity")]
        public async Task<IActionResult> RegisterCharity([FromForm] CharityRegisterDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model data during charity registration.");
                    return BadRequest(ResponseAPI<string>.Error("Invalid model data."));
                }

                var result = await _authService.RegisterCharityAsync(model);

                if (result == null || result != "Success")
                {
                    _logger.LogWarning("Error occurred while registering charity: {CharityName}", model.CharityName);
                    return BadRequest(ResponseAPI<string>.Error($"Error registering charity. {result}"));
                }

                _logger.LogInformation("Charity registered successfully: {CharityName}", model.CharityName);
                return Ok(ResponseAPI<string>.Success(null, "Charity registered successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during charity registration.");
                return StatusCode(500, ResponseAPI<string>.Error($"Server error: {ex.InnerException?.Message ?? ex.Message}", 500));
            }
        }

        [HttpPost("RegisterUser")]
        public async Task<IActionResult> RegisterUser([FromForm] UserRegisterDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model data during user registration.");
                    return BadRequest(ResponseAPI<string>.Error("Invalid model data."));
                }

                var result = await _authService.RegisterUserAsync(model);
                if (result == null || result != "Success")
                {
                    _logger.LogWarning("Error occurred while registering user: {Email}", model.Email);
                    return BadRequest(ResponseAPI<string>.Error($"Error registering charity. {result}"));
                }

                _logger.LogInformation("User registered successfully: {Email}", model.Email);
                return Ok(ResponseAPI<object>.Success(result, "User registered successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during user registration.");
                return StatusCode(500, ResponseAPI<string>.Error($"Server error: {ex.InnerException?.Message ?? ex.Message}", 500));
            }
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            try
            {
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Invalid email confirmation request. UserId or Token is missing.");
                    return BadRequest(ResponseAPI<string>.Error("Invalid email confirmation request."));
                }

                var user = await _authService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User not found for email confirmation. UserId: {UserId}", userId);
                    return NotFound(ResponseAPI<string>.Error("User not found."));
                }

                var result = await _authService.ConfirmEmailAsync(user, token);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Email confirmation failed for user: {UserId}", userId);
                    return BadRequest(ResponseAPI<string>.Error("Email confirmation failed."));
                }

                _logger.LogInformation("Email confirmed successfully for user: {UserId}", userId);
                return Ok(ResponseAPI<string>.Success("Email confirmed successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during email confirmation.");
                return BadRequest(ResponseAPI<string>.Error(ex.InnerException?.Message ?? ex.Message));
            }
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var user = await _authService.GetCurrentUserAsync(User);
                if (user == null)
                    return NotFound(ResponseAPI<string>.Error("User not found."));

                return Ok(ResponseAPI<object>.Success(user, "Current user retrieved successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get current user failed");
                return StatusCode(500, ResponseAPI<string>.Error(ex.InnerException?.Message ?? ex.Message, 500));
            }
        }

        

    }
}

