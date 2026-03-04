using Application.Features.SurchargeFeature.Dtos;
using Application.Features.TariffGridFeature.Dtos;
using Application.Features.TestFeature.Commands;
using Application.Features.TestFeature.Dtos;
using Application.Features.UserFeature.Dtos;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using System.Text.Json;

namespace Application.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // Test mappings
            CreateMap<AddTestCommandNew, Test>();
            CreateMap<Test, TestDTO>().ReverseMap();

            // Pour PagedList<Test> -> PagedList<TestDTO>
            CreateMap<PagedList<Test>, PagedList<TestDTO>>()
                .ConvertUsing((src, dest, context) =>
                {
                    var items = context.Mapper.Map<IEnumerable<TestDTO>>(src.Items);
                    return new PagedList<TestDTO>(items, src.TotalCount, src.CurrentPage, src.PageSize);
                });

            // User mappings
            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

            // Pour PagedList<User> -> PagedList<UserDTO>
            CreateMap<PagedList<User>, PagedList<UserDTO>>()
                .ConvertUsing((src, dest, context) =>
                {
                    var items = context.Mapper.Map<IEnumerable<UserDTO>>(src.Items);
                    return new PagedList<UserDTO>(items, src.TotalCount, src.CurrentPage, src.PageSize);
                });

            // Tariff Grid mappings
            CreateMap<TariffGrid, TariffGridDTO>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.TariffLinesCount, opt => opt.Ignore());

            CreateMap<TariffGrid, TariffGridDetailsDTO>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.TariffLines, opt => opt.MapFrom(src => src.TariffLines.Where(l => !l.IsDeleted)));

            CreateMap<TariffGrid, TariffGridHistoryDTO>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.TariffLinesCount, opt => opt.Ignore());

            // Tariff Line mappings
            CreateMap<TariffLine, TariffLineDTO>()
                .ForMember(dest => dest.ZoneFromName, opt => opt.MapFrom(src => src.ZoneFrom != null ? src.ZoneFrom.Name : null))
                .ForMember(dest => dest.ZoneToName, opt => opt.MapFrom(src => src.ZoneTo != null ? src.ZoneTo.Name : null))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => !src.IsDeleted));

            CreateMap<PagedList<TariffGrid>, PagedList<TariffGridDTO>>()
                .ConvertUsing((src, dest, context) =>
                {
                    var items = context.Mapper.Map<IEnumerable<TariffGridDTO>>(src.Items);
                    return new PagedList<TariffGridDTO>(items, src.TotalCount, src.CurrentPage, src.PageSize);
                });

            // Surcharge mappings
            CreateMap<Surcharge, SurchargeDTO>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.CalculationType, opt => opt.MapFrom(src => src.CalculationType.ToString()))
                .ForMember(dest => dest.RulesCount, opt => opt.Ignore());

            CreateMap<Surcharge, SurchargeDetailsDTO>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.CalculationType, opt => opt.MapFrom(src => src.CalculationType.ToString()))
                .ForMember(dest => dest.Rules, opt => opt.MapFrom(src => src.Rules.Where(r => !r.IsDeleted)));

            // SurchargeRule mapping avec une méthode séparée pour éviter l'erreur CS0854
            CreateMap<SurchargeRule, SurchargeRuleDTO>()
                .ForMember(dest => dest.SurchargeId, opt => opt.MapFrom(src => src.SurchargeId))
                .ForMember(dest => dest.ZoneFromName, opt => opt.MapFrom(src => src.ZoneFrom != null ? src.ZoneFrom.Name : null))
                .ForMember(dest => dest.ZoneToName, opt => opt.MapFrom(src => src.ZoneTo != null ? src.ZoneTo.Name : null))
                .ForMember(dest => dest.ApplicableTransportModes, opt => opt.MapFrom(src => ConvertTransportModes(src.ApplicableTransportModes)));
        }

        // Méthode helper pour convertir le JSON en liste
        private static List<string>? ConvertTransportModes(string? applicableTransportModes)
        {
            if (string.IsNullOrEmpty(applicableTransportModes))
                return null;

            try
            {
                return JsonSerializer.Deserialize<List<string>>(applicableTransportModes);
            }
            catch
            {
                return null;
            }
        }
    }
}