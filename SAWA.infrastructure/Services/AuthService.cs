using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        public AuthService(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IGenerateTokenServices tokenService,
            RoleManager<IdentityRole> roleManager,
            IMapper mapper,
            IFileManagementService fileManagementService,
            IUnitOfWork unitOfWork
            )
            
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _roleManager = roleManager;
            _mapper = mapper;
            _fileManagementService = fileManagementService;
            _unitOfWork = unitOfWork;
        }
        public async  Task<IdentityResult> ConfirmEmailAsync(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            return await _userManager.ConfirmEmailAsync(user, token);
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
                return "Success";
            }
            catch (Exception ex)
            {
                return $"An error occurred while registering the user: {ex.Message}";
            }
        }

    }
}
