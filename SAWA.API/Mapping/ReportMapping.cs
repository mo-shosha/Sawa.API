using AutoMapper;
using SAWA.core.DTO;
using SAWA.core.Models;

namespace SAWA.API.Mapping
{
    public class ReportMapping : Profile
    {
        public ReportMapping()
        {
            CreateMap<ReportCreateDto, Report>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => Enum.Parse<ReportType>(src.Type)))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<Report, ReportDto>()
                .ForMember(dest => dest.ReporterName, opt => opt.MapFrom(src => src.Reporter.FullName))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));
        }

    }
}
