using AutoMapper;
using SAWA.core.DTO;
using SAWA.core.Models;

namespace SAWA.API.Mapping
{
    public class PostMapping : Profile
    {
        public PostMapping()
        {
            CreateMap<Post, PostDto>()
                .ForMember(dest => dest.CharityName, opt => opt.MapFrom(src => src.Charity.UserName))
                .ForMember(dest => dest.Photos, opt => opt.MapFrom(src => src.Photos))
                .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments));

            CreateMap<PostCreateDto, Post>()
                .ForMember(dest => dest.Photos, opt => opt.Ignore())  
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<Photo, PhotoDto>()
                .ForMember(dest => dest.ImgName, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.PostId, opt => opt.MapFrom(src => src.PostId ?? 0));

            CreateMap<PhotoDto, Photo>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImgName))
                .ForMember(dest => dest.PostId, opt => opt.MapFrom(src => src.PostId));

            CreateMap<Comment, CommentDto>()
                .ForMember(dest => dest.PostId, opt => opt.MapFrom(src => src.PostId));

            CreateMap<CommentDto, Comment>();
        }
    }
}
