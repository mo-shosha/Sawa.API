using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SAWA.core.DTO
{
    public record AppUserDto // Data returned when user logs in
    {
        public string Id { get; init; }
        public string UserName { get; init; }
        public string Email { get; init; }
        public string PhoneNumber { get; init; }
        public string FullName { get; init; }
        public string? ProfilePhotoUrl { get; init; }

        public string? WallpaperPhotoUrl { get; init; }
        public DateTime CreatedAt { get; init; }
        public bool IsEmailConfirmed { get; init; }
        public IList<string> Roles { get; set; }
        public string Token { get; set; }
        public DateTime ExpireAt { get; set; }
    }
    public record BaseRegisterDto
    {
        [Required]
        public string UserName { get; init; }

        [Required, EmailAddress]
        public string Email { get; init; }

        [Required, Phone]
        public string PhoneNumber { get; init; }

        [Required, DataType(DataType.Password)]
        public string Password { get; init; }

        [Required, DataType(DataType.Password), Compare("Password")]
        public string ConfirmPassword { get; init; }

        public IFormFile? ProfilePhoto { get; init; }

        [Required]
        public string Address { get; init; }
    }
    public record UserRegisterDto:BaseRegisterDto // Data received from user registration
    {
        [Required]
        public string FirstName { get; init; }

        [Required]
        public string LastName { get; init; }

       
    }

    public record CharityRegisterDto : BaseRegisterDto // Data received from charity registration
    {
        [Required]
        public string CharityName { get; init; }

        [Required]
        public IFormFile? WallpaperPhoto { get; init; }

        [Required]
        public string CharityType { get; init; }

        [Required]
        public string Description { get; init; }

        [Required]
        public IFormFile WebsiteDocument { get; init; }

        [Required]
        public string RegistrationNumber { get; init; }

        [Required]
        public string Country { get; init; }


    }

    public record UserLoginDto // Data received from user login
    {
        [Required]
        public string Email { get; init; }

        [Required]
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

    

}
