using AutoMapper;
using SAWA.core.DTO;
using SAWA.core.Models;

namespace SAWA.API.Mapping
{
    public class CommentMapping : Profile
    {
        public CommentMapping()
        {
            CreateMap<Comment, CommentDto>()
               .ForMember(dest => dest.PostId, opt => opt.MapFrom(src => src.PostId))
               .ForMember(dest => dest.User_PhotoUrl, opt => opt.MapFrom(src => src.User.ProfilePhotoURL))
               .ForMember(dest => dest.User_FullName, opt => opt.MapFrom(src => src.User.FullName));

            CreateMap<CommentDto, Comment>();
            CreateMap<CommentCreateDto, Comment>();
        }
    }
}
