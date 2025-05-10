using AutoMapper;
using SAWA.core.DTO;
using SAWA.core.Models;

namespace SAWA.API.Mapping
{
    public class DonationMapper :Profile
    {
        public DonationMapper()
        {
            CreateMap<Donation, DonationDto>();

            CreateMap<MonetaryDonationRequestDto, Donation>()
                    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => DonationType.Monetary))
                    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => DonationStatus.Pending))
                    .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                    .ForMember(dest => dest.StripeSessionId, opt => opt.Ignore())
                    .ForMember(dest => dest.Photos, opt => opt.Ignore())
                    .ForMember(dest => dest.UserId, opt => opt.Ignore())
                    .ForMember(dest => dest.CharityId, opt => opt.Ignore());

            CreateMap<ItemDonationRequestDto, Donation>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => DonationType.Product))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => DonationStatus.Pending))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Photos, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CharityId, opt => opt.Ignore())
                .ForMember(dest => dest.Amount, opt => opt.Ignore());
        }
    }
}

