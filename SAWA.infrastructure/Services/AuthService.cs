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
using System.Security.Claims;

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
            await _signInManager.SignOutAsync();
        }

        public async Task<string> RegisterCharityAsync(CharityRegisterDto model)
        {
            try
            {
                AppUser newCharity = _mapper.Map<AppUser>(model);

                if (await _userManager.FindByNameAsync(model.CharityName) != null)
                {
                    return "Error: The charity name already exists. Please choose a different name.";
                }

                if (await _userManager.FindByEmailAsync(model.Email) != null)
                {
                    return "Error: The email address is already associated with another account.";
                }

                //var existingUserWithPhone = _userManager.Users.FirstOrDefault(u => u.PhoneNumber == model.PhoneNumber);
                //if (!string.IsNullOrEmpty(model.PhoneNumber) && existingUserWithPhone != null)
                //{
                //    return "Error: The phone number is already associated with another account.";
                //}

                if (model.Document != null)
                {
                    newCharity.DocumentURL = await _fileManagementService.AddImagesAsync(model.Document, model.CharityName);
                }

                //if (model.WallpaperPhoto != null)
                //{
                //    newCharity.WallpaperPhotoURL = await _fileManagementService.AddImagesAsync(model.WallpaperPhoto, model.CharityName);
                //}

                newCharity.UserName = model.CharityName.Replace(" ", "");

                var result = await _userManager.CreateAsync(newCharity, model.Password);

                if (!result.Succeeded)
                    return string.Join(", ", result.Errors.Select(e => e.Description));

                

                await _unitOfWork.SaveAsync();

                await _userManager.AddToRoleAsync(newCharity, "Charity");

                await SendConfirmationEmailAsync(newCharity);

                return "Success";
            }
            catch (Exception ex)
            {
                return $"An error occurred while registering the charity: {ex.Message}";
            }
        }

        public async Task<bool> UpdateProfileAsync(UserUpdateDto model, string UserId)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null) return false;

            if (model.ProfilePhoto != null)
            {
                user.ProfilePhotoURL = await _fileManagementService.AddImagesAsync(model.ProfilePhoto, user.FullName);
            }

            if (model.WallpaperPhoto != null)
            {
                user.WallpaperPhotoURL = await _fileManagementService.AddImagesAsync(model.WallpaperPhoto, user.FullName);
            }

            if (!string.IsNullOrEmpty(model.Email) && user.Email != model.Email)
            {
                var emailExists = await _userManager.FindByEmailAsync(model.Email);
                if (emailExists != null && emailExists.Id != UserId)
                {
                    return false;
                }

                user.Email = model.Email;
               
            }

            if (!string.IsNullOrEmpty(model.Phone) && user.PhoneNumber != model.Phone)
            {
                user.PhoneNumber = model.Phone;

            }

            if (!string.IsNullOrEmpty(model.Adress) && user.Address != model.Adress)
            {
                user.Address = model.Adress;

            }
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return false;
            }

            await _unitOfWork.SaveAsync();
            return true;
        }


        public async Task<string> RegisterUserAsync(UserRegisterDto model)
        {
            try
            {
                AppUser newUser = _mapper.Map<AppUser>(model);
                if (await _userManager.FindByNameAsync(model.FullName) != null)
                {
                    return "Error: The username already exists. Please choose a different username.";
                }

                if (await _userManager.FindByEmailAsync(model.Email) != null)
                {
                    return "Error: The email address is already associated with another account.";
                }

                //var existingUserWithPhone = _userManager.Users.FirstOrDefault(u => u.PhoneNumber == model.PhoneNumber);
                //if (!string.IsNullOrEmpty(model.PhoneNumber) && existingUserWithPhone != null)
                //{
                //    return "Error: The phone number is already associated with another account.";
                //}
                //if (model.ProfilePhoto != null)
                //{
                //    newUser.ProfilePhotoURL = await _fileManagementService.AddImagesAsync(model.ProfilePhoto, newUser.FullName);
                //}
                newUser.UserName = model.FullName.Replace(" ", "");
                var result = await _userManager.CreateAsync(newUser, model.Password);

                if (!result.Succeeded)
                    return string.Join(", ", result.Errors.Select(e => e.Description));

                

                await _unitOfWork.SaveAsync();  

                await _userManager.AddToRoleAsync(newUser, "User");

                await SendConfirmationEmailAsync(newUser);

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


        private async Task SendConfirmationEmailAsync(AppUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"{_configuration["Token:Issuer"]}/api/V1/Auth/ConfirmEmail?userId={user.Id}&token={Uri.EscapeDataString(token)}";

            var emailBody = $@"
                <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: auto; border: 1px solid #eee; border-radius: 8px; padding: 20px; background-color: #fff;'>
                    <div style='text-align: center; margin-bottom: 20px;'>
                        <h1 style='color: #e2851b; margin: 0;'>HopeGivers</h1>
                        <p style='color: #999; font-size: 14px;'>Empowering Hope, One Click at a Time</p>
                    </div>
                    <h2 style='color: #e2851b;'>Confirm Your Email Address</h2>
                    <p>Hello <strong>{user.FullName}</strong>,</p>
                    <p>Thank you for registering! Please confirm your email address to activate your account.</p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{confirmationLink}' style='display: inline-block; padding: 12px 25px; background-color: #e2851b; color: #fff; text-decoration: none; border-radius: 5px; font-weight: bold;'>
                            Confirm Email
                        </a>
                    </div>
                    <p>If you did not create an account, please ignore this email.</p>
                    <p>Thank you,<br/>HopeGivers Team</p>
                    <hr style='margin-top: 30px; border: none; border-top: 1px solid #ddd;'/>
                    <p style='font-size: 12px; color: #aaa;'>
                        This email was sent to {user.Email}. If you have any questions, contact us at 
                        <a href='mailto:support@hopegivers.org' style='color: #e2851b;'>support@hopegivers.org</a>.
                    </p>
                </div>";


            await _emailService.SendEmailAsync(
                user.Email,
                "Confirm Your Email",
                emailBody
            );
        }


        public async Task<AppUser> GetUserByUserNameAsync(string username)
        {
            return await _userManager.Users
                                     .Where(u => u.UserName == username)
                                     .FirstOrDefaultAsync();
        }

        public async Task<AppUserDto> GetCurrentUserAsync(ClaimsPrincipal user)
        {
            var UserName = _userManager.GetUserName(user);  
            var appUser = await _userManager.Users
                                            .FirstOrDefaultAsync(u => u.UserName == UserName);
            if (appUser == null)
                return null;

            var roles = await _userManager.GetRolesAsync(appUser);
            var token = await _tokenService.GetAndCreateToken(appUser);

            var result = _mapper.Map<AppUserDto>(appUser);
            result.Token = token;
            result.ExpireAt = DateTime.Now.AddHours(30);
            result.Roles = roles.ToList();

            return result;
        }

        public async Task<List<CharityDto>> GetAllCharitiesAsync()
        {
            var charityUsers = await _userManager.GetUsersInRoleAsync("Charity");

            return _mapper.Map<List<CharityDto>>(charityUsers);
        }

        public async Task<AppUser> GetCharityByUserName(string UserName)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == UserName);
            return user;
        }

        public async Task<CharityInfoDto> GetCharityByUserNameAsync(string UserName)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == UserName);

            if (user == null)
                return null;

            var charityDto = _mapper.Map<CharityInfoDto>(user);
            return charityDto;
        }


        #region Amin
        public async Task<IEnumerable<CharityReviewDto>> GetPendingCharitiesAsync()
        {
            var charities = await _userManager.GetUsersInRoleAsync("Charity");
            var pendingCharities = charities
                .Where(c => c.IsApproved==false)
                .ToList();

            return _mapper.Map<IEnumerable<CharityReviewDto>>(pendingCharities);
        }

        public async Task<IEnumerable<CharityReviewDto>> GetAllCharitiesAsyncForAdmin(string status = null)
        {
            var charities = await _userManager.GetUsersInRoleAsync("Charity");
            var filteredCharities = charities.AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                if (status == "Pending") filteredCharities = filteredCharities.Where(c => c.IsApproved == false);
                else if(status== "Approved ") filteredCharities = filteredCharities.Where(c => c.IsApproved == true);
                
            }

            var result = filteredCharities.ToList();
            return _mapper.Map<IEnumerable<CharityReviewDto>>(result);
        }

        public async Task<bool> AcceptCharityAsync( string id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null || !await _userManager.IsInRoleAsync(user, "Charity"))
                return false;

            user.IsApproved = true;
            await _userManager.UpdateAsync(user);
            await _unitOfWork.SaveAsync();
            return true;
        }


        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userManager.GetUsersInRoleAsync("User");
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null )
                return false;

            await _userManager.DeleteAsync(user);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> UpdateUserRoleAsync(UpdateUserRoleDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
                return false;

            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!removeResult.Succeeded)
                return false;

            if (!await _roleManager.RoleExistsAsync(dto.NewRole))
            {
                   return false;
            }

            var addResult = await _userManager.AddToRoleAsync(user, dto.NewRole);
            return addResult.Succeeded;
        }





        #endregion
    }
}
