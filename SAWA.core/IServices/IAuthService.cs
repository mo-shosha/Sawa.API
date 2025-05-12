using Microsoft.AspNetCore.Identity;
using SAWA.core.DTO;
using SAWA.core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SAWA.core.IServices
{
    public interface IAuthService
    {
        Task<string> RegisterUserAsync(UserRegisterDto model);
        Task<string> RegisterCharityAsync(CharityRegisterDto model);
        Task<AppUserDto> LoginAsync(UserLoginDto model);
        Task<AppUserDto> LoginWithGoogleAsync(string email);
        Task LogoutAsync();
        Task<bool> UpdateProfileAsync(UserUpdateDto model, string UserId);
        Task<AppUser> GetUserByEmailAsync(string email);
        Task<AppUser> GetUserByIdAsync(string userId);
        Task<IdentityResult> ConfirmEmailAsync(AppUser user, string token);
        Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordDto dto);

        Task<AppUser> GetUserByUserNameAsync(string username);

        Task<AppUserDto> GetCurrentUserAsync(ClaimsPrincipal user);
        Task<List<CharityDto>> GetAllCharitiesAsync();
        Task<AppUser> GetCharityByUserName(string UserName);
        Task<CharityInfoDto> GetCharityByUserNameAsync(string UserName);

        #region Amin
        Task<IEnumerable<CharityReviewDto>> GetPendingCharitiesAsync();
        Task<IEnumerable<CharityReviewDto>> GetAllCharitiesAsyncForAdmin(string status = null);
        Task<bool> AcceptCharityAsync(string id);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<bool> DeleteUserAsync(string id);
        Task<bool> UpdateUserRoleAsync(UpdateUserRoleDto dto);

        #endregion
    }
}
