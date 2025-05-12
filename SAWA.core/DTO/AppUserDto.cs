using Microsoft.AspNetCore.Http;
using SAWA.core.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAWA.core.DTO
{
    public record AppUserDto // Data returned when user logs in
    {
        public string UserName { get; init; }
        public string Email { get; init; }
        public string PhoneNumber { get; init; }
        public string FullName { get; init; }
        public string Address { get; init; }
        public string ProfilePhotoUrl { get; init; }

        public string WallpaperPhotoUrl { get; init; }
        public DateTime CreatedAt { get; init; }
        public bool IsEmailConfirmed { get; init; }
        public IList<string> Roles { get; set; }
        public string Token { get; set; }
        public DateTime ExpireAt { get; set; }
    }
    public record BaseRegisterDto
    {

        [Required, EmailAddress]
        public string Email { get; init; }

        [Required, Phone]
        public string PhoneNumber { get; init; }

        [Required, DataType(DataType.Password)]
        public string Password { get; init; }

        [Required, DataType(DataType.Password), Compare("Password")]
        public string ConfirmPassword { get; init; }

        //public IFormFile? ProfilePhoto { get; init; }

        [Required]
        public string Address { get; init; }
    }
    public record UserRegisterDto:BaseRegisterDto // Data received from user registration
    {
        [Required]
        public string FullName { get; init; }

    }

    public record CharityRegisterDto : BaseRegisterDto // Data received from charity registration
    {
        [Required]
        public string CharityName { get; init; }

        //[Required]
        //public IFormFile? WallpaperPhoto { get; init; }

        //[Required]
        //public string CharityType { get; init; }

        [Required]
        public string Description { get; init; }

        [Required]
        [PdfOnly]
        public IFormFile Document { get; init; }

        //[Required]
        //public string RegistrationNumber { get; init; }

        [Required]
        public string Country { get; init; }


    }

    public record UserLoginDto // Data received from user login
    {
        [Required, EmailAddress]
        public string Email { get; init; }

        [Required, DataType(DataType.Password)]
        public string Password { get; init; }

    }


    public record RestPasswordDTO : UserLoginDto
    {
        public string Token { get; set; }
    }

    public record ForgotPasswordDto
    {
        [Required, EmailAddress]
        public string Email { get; init; }
    }


    public record ActiveAccountDTO
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }


    public record ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; init; }

        [Required, DataType(DataType.Password)]
        public string NewPassword { get; init; }

        [Required, DataType(DataType.Password), Compare("NewPassword")]
        public string ConfirmNewPassword { get; init; }
    }


    public record GoogleLoginDto
    {
        [Required]
        public string AccessToken { get; set; }
    }



    public record CharityDto
    {
        public string UserName { get; set; }
        public string CharityName { get; set; }
        public string PhotoUrl { get; set; }
        public string Address { get; set; }

    }

    public record UpdateUserRoleDto
    {
        public string UserId { get; set; }
        public string NewRole { get; set; }
    }

    public record CharityReviewDto
    {
        public string Id { get; set; }
        public string CharityName { get; set; }
        public string Description { get; set; }

        public string DocumentURL { get; set; }

        public string Status { get; set; }
        public DateTime CreateAt { get; set; }
        public string Email { get; set; }
    }

    public record UserDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public DateTime CreateAt { get; set; }
        public string Email { get; set; }
    }

    public record CharityInfoDto
    {
        public string CharityName { get; set; }
        public string Address { get; set; }
        public string ProfilePhotoURL { get; set; }
        public string WallpaperPhotoUrl { get; init; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }
    }

    public record UserUpdateDto
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Adress { get; set; }
        public IFormFile? WallpaperPhoto { get; set; }
        public IFormFile? ProfilePhoto { get; set; }

    }
}
