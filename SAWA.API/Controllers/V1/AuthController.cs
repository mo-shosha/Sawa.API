using Microsoft.AspNetCore.Mvc;
using SAWA.core.DTO;
using SAWA.core.IServices;
using SAWA.API.Healper;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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

                if (result.Success == false)
                {
                    _logger.LogWarning("Invalid login attempt with email: {Email}", model.Email);
                    return Unauthorized(ResponseAPI<string>.Error("Invalid email or password.", 401));
                }

                _logger.LogInformation("User logged in successfully: {Email}", model.Email);
                return Ok(ResponseAPI<object>.Success(result.User, $"{result.Message}"));
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
                return Ok(ResponseAPI<string>.Success("Charity registered successfully."));
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
                    return Content(GetHtmlResponse("Invalid email confirmation request."), "text/html");
                }

                var user = await _authService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User not found for email confirmation. UserId: {UserId}", userId);
                    return Content(GetHtmlResponse("User not found."), "text/html");
                }

                var result = await _authService.ConfirmEmailAsync(user, token);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Email confirmation failed for user: {UserId}", userId);
                    return Content(GetHtmlResponse("Email confirmation failed."), "text/html");
                }

                _logger.LogInformation("Email confirmed successfully for user: {UserId}", userId);
                return Content(GetHtmlResponse("Email confirmed successfully."), "text/html");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during email confirmation.");
                return Content(GetHtmlResponse(ex.InnerException?.Message ?? ex.Message), "text/html");
            }
        }

        private string GetHtmlResponse(string message)
        {
            return $@"
                <!DOCTYPE html>
                <html lang='en'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Email Confirmation - HopeGivers</title>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            background-color: #f7f7f7;
                            color: #333;
                            margin: 0;
                            padding: 0;
                            display: flex;
                            justify-content: center;
                            align-items: center;
                            height: 100vh;
                        }}
                        .container {{
                            background-color: #ffffff;
                            padding: 30px;
                            border-radius: 10px;
                            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                            width: 100%;
                            max-width: 600px;
                            text-align: center;
                        }}
                        .header {{
                            font-size: 36px;
                            font-weight: bold;
                            color: #f4a261;
                            margin-bottom: 20px;
                        }}
                        .tagline {{
                            font-size: 18px;
                            color: #6c757d;
                            margin-bottom: 30px;
                        }}
                        .message {{
                            font-size: 16px;
                            margin-top: 20px;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>HopeGivers</div>
                        <div class='tagline'>Empowering Hope, One Click at a Time</div>
                        <p class='message'>{message}</p>
                    </div>
                </body>
                </html>
                ";
        }


        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var user = await _authService.GetCurrentUserAsync(User);
                if (user == null)
                    return NotFound(ResponseAPI<string>.Error("User not found.", 404));

                return Ok(ResponseAPI<object>.Success(user, "Current user retrieved successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get current user failed");
                return StatusCode(500, ResponseAPI<string>.Error(ex.InnerException?.Message ?? ex.Message, 500));
            }
        }

        [Authorize]
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromForm] UserUpdateDto model)
        {
            try
            {
                var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var result = await _authService.UpdateProfileAsync(model, UserId);
                if (!result)
                {
                    return BadRequest(ResponseAPI<string>.Error("Failed to update profile. Please check your input or try again."));
                }
                return Ok(ResponseAPI<string>.Success("Profile updated successfully."));
            }
            catch (Exception ex)
            {

                return StatusCode(500, ResponseAPI<string>.Error(ex.InnerException?.Message ?? ex.Message, 500));
            }
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ResponseAPI<string>.Error("Invalid input."));

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(ResponseAPI<string>.Error("Unauthorized.", 401));

                var result = await _authService.ChangePasswordAsync(userId, dto);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return BadRequest(ResponseAPI<string>.Error($"{errors}"));
                }

                return Ok(ResponseAPI<string>.Success("Password changed successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseAPI<string>.Error("An unexpected error occurred. Please try again later."));
            }
        }

    }
}

