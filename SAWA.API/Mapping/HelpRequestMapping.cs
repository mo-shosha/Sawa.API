using AutoMapper;
using SAWA.core.DTO;
using SAWA.core.Models;
using System.Linq;

namespace SAWA.API.Mapping
{
    public class HelpRequestMapping : Profile
    {
        public HelpRequestMapping()
        {
            CreateMap<HelpRequest, HelpRequestDto>()
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.CharityName, opt => opt.MapFrom(src => src.Charity.FullName))
                .ForMember(dest => dest.PhotoUrls, opt => opt.MapFrom(src => src.Photos.Select(p => p.ImageUrl).ToList()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<HelpRequestCreateDto, HelpRequest>()
                .ForMember(dest => dest.Photos, opt => opt.Ignore())
                .ForMember(dest => dest.Charity, opt => opt.Ignore())
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));

            CreateMap<HelpRequestUpdateStatusDto, HelpRequest>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.NewStatus));

            CreateMap<HelpRequest, HelpRequestResponse>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<HelpRequest, HelpRequestUserResponse>()
                .IncludeBase<HelpRequest, HelpRequestResponse>()
                 .ForMember(dest => dest.PhotoUrls, opt => opt.MapFrom(src => src.Photos.Select(p => p.ImageUrl).ToList()))
                .ForMember(dest => dest.CharityName, opt => opt.MapFrom(src => src.Charity.FullName));

            CreateMap<HelpRequest, HelpRequestCharityResponse>()
                .IncludeBase<HelpRequest, HelpRequestResponse>()
                .ForMember(dest => dest.PhotoUrls, opt => opt.MapFrom(src => src.Photos.Select(p => p.ImageUrl).ToList()))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName));
        }
    }
}
