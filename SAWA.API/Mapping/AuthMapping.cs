using AutoMapper;
using SAWA.core.DTO;
using SAWA.core.Models;

namespace SAWA.API.Mapping
{
    public class AuthMapping:Profile
    {
        public AuthMapping()
        {
            CreateMap<AppUser, AppUserDto>()
               .ForMember(dest => dest.ProfilePhotoUrl, opt => opt.MapFrom(src => src.ProfilePhotoURL))
               .ForMember(dest => dest.WallpaperPhotoUrl, opt => opt.MapFrom(src=>src.WallpaperPhotoURL))
               .ForMember(dest => dest.Roles, opt => opt.Ignore()).ReverseMap();


            CreateMap<UserRegisterDto, AppUser>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.ProfilePhotoURL, opt => opt.Ignore())
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.WallpaperPhotoURL, opt => opt.Ignore()).ReverseMap();


            CreateMap<CharityRegisterDto, AppUser>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.CharityName))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.DocumentURL, opt => opt.Ignore())  
                .ForMember(dest => dest.IsApproved, opt => opt.MapFrom(_ => false)) 
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.ProfilePhotoURL, opt => opt.Ignore())
                .ForMember(dest => dest.WallpaperPhotoURL, opt => opt.Ignore()).ReverseMap();
        }
    }
}
