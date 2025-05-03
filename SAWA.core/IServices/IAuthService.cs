using Microsoft.AspNetCore.Identity;
using SAWA.core.DTO;
using SAWA.core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAWA.core.IServices
{
    public interface IAuthService
    {
        Task<string> RegisterUserAsync(UserRegisterDto model);
        Task<string> RegisterCharityAsync(CharityRegisterDto model);
        Task<AppUserDto> LoginAsync(UserLoginDto model);
        Task LogoutAsync();
        Task<AppUser> GetUserByEmailAsync(string email);
        Task<AppUser> GetUserByIdAsync(string userId);
        Task<IdentityResult> ConfirmEmailAsync(string email, string token);
    }
}
