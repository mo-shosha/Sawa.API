using AutoMapper;
using SAWA.core.DTO;
using SAWA.core.Models;

namespace SAWA.API.Mapping
{
    public class BranchesMapping:Profile
    {
        public BranchesMapping()
        {
            CreateMap<BranchCreateDto, Branch>()
                .ForMember(dest => dest.Photos, opt => opt.Ignore())     
                .ForMember(dest => dest.CharityId, opt => opt.Ignore())   
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());  

            CreateMap<Branch, BranchDto>()
                .ForMember(dest => dest.photos, opt => opt.MapFrom(src => src.Photos));

            CreateMap<BranchUpdateDto, Branch>()
                 .ForMember(dest => dest.Photos, opt => opt.Ignore())  
                 .ForMember(dest => dest.CharityId, opt => opt.Ignore())  
                 .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());  


        }
    }
}
