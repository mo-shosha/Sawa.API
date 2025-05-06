using AutoMapper;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SAWA.core.DTO;
using SAWA.core.Interfaces;
using SAWA.core.IServices;
using SAWA.core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAWA.infrastructure.Services
{
    public class AuthService : IAuthService
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IGenerateTokenServices _tokenService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IFileManagementService _fileManagementService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IEmailServices _emailService;
        public AuthService(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IGenerateTokenServices tokenService,
            RoleManager<IdentityRole> roleManager,
            IMapper mapper,
            IFileManagementService fileManagementService,
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IEmailServices emailService
            )
            
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _roleManager = roleManager;
            _mapper = mapper;
            _fileManagementService = fileManagementService;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<AppUser> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<AppUser> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<AppUserDto> LoginAsync(UserLoginDto model)
        {
            try
            {
                var user = await _userManager.Users
                                        .Where(u => u.Email == model.Email)
                                        .FirstOrDefaultAsync();
                if (user == null) return null;

                var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
                if (!result.Succeeded)
                    return null;

                var roles = await _userManager.GetRolesAsync(user);

                var token = await _tokenService.GetAndCreateToken(user);

                AppUserDto appUser = _mapper.Map<AppUserDto>(user);
                appUser.Roles = roles.ToList();
                appUser.Token = token;
                appUser.ExpireAt = DateTime.Now.AddHours(30);

                return appUser;
            }
            catch(Exception ex)
            {
                return null;
            }

        }

        public async Task LogoutAsync()
        {
            
        }

        public async Task<string> RegisterCharityAsync(CharityRegisterDto model)
        {
            try
            {
                AppUser newCharity = _mapper.Map<AppUser>(model);

                if (model.ProfilePhoto != null)
                {
                    newCharity.ProfilePhotoURL = await _fileManagementService.AddImagesAsync(model.ProfilePhoto, model.CharityName);
                }

                if (model.WallpaperPhoto != null)
                {
                    newCharity.WallpaperPhotoURL = await _fileManagementService.AddImagesAsync(model.WallpaperPhoto, model.CharityName);
                }

                var result = await _userManager.CreateAsync(newCharity, model.Password);

                if (!result.Succeeded)
                    return string.Join(", ", result.Errors.Select(e => e.Description));

                await _unitOfWork.SaveAsync();

                await _userManager.AddToRoleAsync(newCharity, "Charity");
                return "Success";
            }
            catch (Exception ex)
            {
                return $"An error occurred while registering the charity: {ex.Message}";
            }
        }


        public async Task<string> RegisterUserAsync(UserRegisterDto model)
        {
            try
            {
                AppUser newUser = _mapper.Map<AppUser>(model);

                if (model.ProfilePhoto != null)
                {
                    newUser.ProfilePhotoURL = await _fileManagementService.AddImagesAsync(model.ProfilePhoto, newUser.FullName);
                }

                var result = await _userManager.CreateAsync(newUser, model.Password);

                if (!result.Succeeded)
                    return string.Join(", ", result.Errors.Select(e => e.Description));


                await _unitOfWork.SaveAsync();  

                await _userManager.AddToRoleAsync(newUser, "User");

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                var confirmationLink = $"{_configuration["Token:Issuer"]}/api/V1/Auth/ConfirmEmail?userId={newUser.Id}&token={Uri.EscapeDataString(token)}";

                _ = Task.Run(async () =>
                {
                    await _emailService.SendEmailAsync(
                        newUser.Email,
                        "Confirm Your Email",
                        $@"
                        <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: auto; border: 1px solid #ddd; border-radius: 8px; padding: 20px;'>
                            <h2 style='color: #007bff;'>Confirm Your Email Address</h2>
                            <p>Hello {newUser.FullName},</p>
                            <p>Thank you for registering! Please confirm your email address to activate your account.</p>
                            <p style='text-align: center;'>
                                <a href='{confirmationLink}' style='display: inline-block; padding: 10px 20px; background-color: #007bff; color: #fff; text-decoration: none; border-radius: 5px; font-weight: bold;'>
                                    Confirm Email
                                </a>
                            </p>
                            <p>If you did not create an account, please ignore this email.</p>
                            <p>Thank you,<br/>The Team</p>
                            <hr style='margin-top: 20px; border: none; border-top: 1px solid #ddd;'/>
                            <p style='font-size: 12px; color: #888;'>This email was sent to {newUser.Email}. If you have any questions, contact us at support@example.com.</p>
                        </div>"
                    );
                });


                return "Success";
            }
            catch (Exception ex)
            {
                return $"An error occurred while registering the user: {ex.Message}";
            }
        }



        public async Task<IdentityResult> ConfirmEmailAsync(AppUser user, string token)
        {
            try
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (!result.Succeeded)
                {
                    throw new Exception("Email confirmation failed");
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error confirming email: {ex.Message}");
            }
        }


        public async Task<AppUserDto> LoginWithGoogleAsync(string accessToken)
        {
            try
            {
                var payload = await VerifyGoogleTokenAsync(accessToken);

                if (payload == null)
                {
                    return null;
                }
                var user = await _userManager.Users
                                              .Where(u => u.Email == payload.Email)
                                              .FirstOrDefaultAsync();

                if (user == null)
                {
                    return null;  
                }

                var roles = await _userManager.GetRolesAsync(user);
                var token = await _tokenService.GetAndCreateToken(user);

                AppUserDto appUser = _mapper.Map<AppUserDto>(user);
                appUser.Roles = roles.ToList();
                appUser.Token = token;
                appUser.ExpireAt = DateTime.Now.AddHours(30);

                return appUser;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async Task<GoogleJsonWebSignature.Payload> VerifyGoogleTokenAsync(string accessToken)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(accessToken);

                return payload; 
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
