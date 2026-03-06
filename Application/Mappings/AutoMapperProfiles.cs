using Application.Features.InvoiceFeature.Commands;
using Application.Features.InvoiceFeature.Dtos;
using Application.Features.QuoteFeature.Commands;
using Application.Features.QuoteFeature.Dtos;
using Application.Features.SupplierFeature.Commands;
using Application.Features.SupplierFeature.Dtos;
using Application.Features.SurchargeFeature.Dtos;
using Application.Features.TariffGridFeature.Dtos;
using Application.Features.TestFeature.Commands;
using Application.Features.TestFeature.Dtos;
using Application.Features.UserFeature.Dtos;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using System.Text.Json;

namespace Application.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // ============================
            // INVOICE LINE
            // ============================

            // DTO -> Entity (pour la création)
            CreateMap<InvoiceLineDto, InvoiceLine>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.InvoiceId, opt => opt.Ignore())
                .ForMember(dest => dest.Invoice, opt => opt.Ignore())
                .ForMember(dest => dest.TotalHT, opt => opt.Ignore())   // Propriété calculée
                .ForMember(dest => dest.TotalVAT, opt => opt.Ignore())  // Propriété calculée
                .ForMember(dest => dest.TotalTTC, opt => opt.Ignore())  // Propriété calculée
                .ForMember(dest => dest.DiscountPercent,               
                    opt => opt.MapFrom(src => src.DiscountPercent));

            // Entity -> DTO (pour les réponses)
            CreateMap<InvoiceLine, InvoiceLineDto>()
              .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));


            // ============================
            // ADD INVOICE COMMAND
            // ============================

            CreateMap<AddInvoiceCommand, Invoice>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.InvoiceDate,
                    opt => opt.MapFrom(src => src.IssueDate))
                .ForMember(dest => dest.Lines, opt => opt.Ignore()); // On gère les lignes manuellement


            // ============================
            // UPDATE INVOICE COMMAND
            // ============================

            CreateMap<UpdateInvoiceCommand, Invoice>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())    // ✅ Ajouté !
                .ForMember(dest => dest.ModifiedDate,
                    opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Lines, opt => opt.Ignore());


            // ============================
            // INVOICE ENTITY -> INVOICE DTO
            // ============================

            CreateMap<Invoice, InvoiceDTO>()
                // Propriétés calculées
                .ForMember(dest => dest.Balance,
                    opt => opt.MapFrom(src => src.TotalTTC - src.AmountPaid))
                .ForMember(dest => dest.IsOverdue,
                    opt => opt.MapFrom(src =>
                        src.Status == InvoiceStatus.Envoyee &&
                        src.DueDate < DateTime.UtcNow))
                .ForMember(dest => dest.DaysOverdue,
                    opt => opt.MapFrom(src =>
                        src.Status == InvoiceStatus.Envoyee &&
                        src.DueDate < DateTime.UtcNow
                            ? (DateTime.UtcNow - src.DueDate).Days
                            : 0))
                .ForMember(dest => dest.Status,                        // ✅ Convertir enum en string
                    opt => opt.MapFrom(src => src.Status.ToString()))

                // Informations dénormalisées
                .ForMember(dest => dest.ClientName,
                    opt => opt.MapFrom(src =>
                        src.Client != null ? src.Client.Name : null))
                .ForMember(dest => dest.SupplierName,
                    opt => opt.MapFrom(src =>
                        src.Supplier != null ? src.Supplier.Name : null))
                .ForMember(dest => dest.ShipmentNumber,
                    opt => opt.MapFrom(src =>
                        src.Shipment != null ? src.Shipment.ShipmentNumber : null))
                .ForMember(dest => dest.CurrencyCode,
                    opt => opt.MapFrom(src =>
                        src.Currency != null ? src.Currency.Code : null))
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.MapFrom(src => src.CreatedDate))

                // ✅ LIGNES - Sans référence circulaire !
                .ForMember(dest => dest.Lines,
                    opt => opt.MapFrom(src => src.Lines.Select(l => new InvoiceLineDto
                    {
                        Id = l.Id,
                        Description = l.Description,
                        Quantity = l.Quantity,
                        UnitPriceHT = l.UnitPriceHT,
                        VATRate = l.VATRate,
                        TransportSegmentId = l.TransportSegmentId,
                        DiscountPercent = l.DiscountPercent      
                    })));


            // ============================
            // PAGED LIST SUPPORT
            // ============================

            CreateMap<PagedList<Invoice>, PagedList<InvoiceDTO>>()
                .ConvertUsing((src, dest, context) =>
                {
                    var items = context.Mapper.Map<List<InvoiceDTO>>(src.Items);
                    return new PagedList<InvoiceDTO>(
                        items,
                        src.TotalCount,
                        src.CurrentPage,
                        src.PageSize);
                });


            // ============================
            // CREATE FROM SHIPMENT DTO -> COMMAND
            // ============================

            CreateMap<CreateInvoiceFromShipmentDto, CreateInvoiceFromShipmentCommand>();


            // ============================
            // OVERDUE INVOICE DTO
            // ============================

            CreateMap<Invoice, OverdueInvoiceDto>()
                .ForMember(dest => dest.Balance,
                    opt => opt.MapFrom(src => src.TotalTTC - src.AmountPaid))
                .ForMember(dest => dest.DaysOverdue,
                    opt => opt.MapFrom(src => (DateTime.UtcNow - src.DueDate).Days))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.ClientName,
                    opt => opt.MapFrom(src => src.Client != null ? src.Client.Name : null))
                .ForMember(dest => dest.SupplierName,
                    opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.Name : null))
                .ForMember(dest => dest.ShipmentNumber,
                    opt => opt.MapFrom(src => src.Shipment != null ? src.Shipment.ShipmentNumber : null))
                .ForMember(dest => dest.CurrencyCode,
                    opt => opt.MapFrom(src => src.Currency != null ? src.Currency.Code : null));

            // ============================
            // UPDATE STATUS DTO -> COMMAND
            // ============================

            CreateMap<UpdateInvoiceStatusDto, UpdateInvoiceStatusCommand>()
                    .ForMember(dest => dest.Status,


                       opt => opt.MapFrom(src => src.Status.ToString())); // ✅ Garder l'enum, pas .ToString() !
                                                                          // ============================
                                                                          // ADD INVOICE LINE
                                                                          // ============================
            CreateMap<AddInvoiceLineDto, AddInvoiceLineCommand>();
            CreateMap<AddInvoiceLineCommand, InvoiceLine>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Invoice, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());
            // ============================
            // UPDATE INVOICE LINE
            // ============================
            CreateMap<UpdateInvoiceLineDto, UpdateInvoiceLineCommand>();

            // ============================
            // QUOTE
            // ============================

            // DTO -> Command
            CreateMap<CreateQuoteDto, CreateQuoteCommand>();

            // Entity -> DTO
            CreateMap<Quote, QuoteDto>()
                .ForMember(dest => dest.ClientName,
                    opt => opt.MapFrom(src => src.Client != null ? src.Client.Name : null))
                .ForMember(dest => dest.MerchandiseTypeName,
                    opt => opt.MapFrom(src => src.MerchandiseType != null ? src.MerchandiseType.Name : null))
                .ForMember(dest => dest.IsValid,
                    opt => opt.MapFrom(src => !src.IsAccepted && DateTime.UtcNow <= src.ValidUntil))
                .ForMember(dest => dest.OriginAddress,
                    opt => opt.MapFrom(src => src.OriginAddress))
               .ForMember(dest => dest.DestinationAddress,
                    opt => opt.MapFrom(src => src.DestinationAddress));

            // Command -> Entity (pour Create)
            CreateMap<CreateQuoteCommand, Quote>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsAccepted, opt => opt.Ignore())
                .ForMember(dest => dest.AcceptedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Client, opt => opt.Ignore())
                .ForMember(dest => dest.MerchandiseType, opt => opt.Ignore())
                .ForMember(dest => dest.Shipment, opt => opt.Ignore());

            // PagedList support
            CreateMap<PagedList<Quote>, PagedList<QuoteDto>>()
                .ConvertUsing((src, dest, context) =>
                {
                    var items = context.Mapper.Map<List<QuoteDto>>(src.Items);
                    return new PagedList<QuoteDto>(
                        items,
                        src.TotalCount,
                        src.CurrentPage,
                        src.PageSize);
                });
            // ============================
            // CONTRACT DTO
            // ============================
            CreateMap<Contract, ContractDto>()
                .ForMember(dest => dest.ClientName,
                    opt => opt.MapFrom(src => src.Client != null ? src.Client.Name : null))
                .ForMember(dest => dest.SupplierName,
                    opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.Name : null))
                .ForMember(dest => dest.IsValid,
                    opt => opt.MapFrom(src => src.IsValidAt(DateTime.UtcNow)))
                .ForMember(dest => dest.IsExpiringSoon,
                    opt => opt.MapFrom(src => src.IsExpiringSoon(30)));

            // ============================
            // TRANSPORT SEGMENT DTO
            // ============================
            CreateMap<TransportSegment, TransportSegmentDto>()
                .ForMember(dest => dest.ZoneFromName,
                    opt => opt.MapFrom(src => src.ZoneFromet != null ? src.ZoneFromet.Name : null))
                .ForMember(dest => dest.ZoneToName,
                    opt => opt.MapFrom(src => src.ZoneTo != null ? src.ZoneTo.Name : null))
                .ForMember(dest => dest.TransportModeName,
                    opt => opt.MapFrom(src => src.TransportMode.ToString()))
                .ForMember(dest => dest.TotalCost,
                    opt => opt.MapFrom(src => src.TotalCost))
                .ForMember(dest => dest.SurchargesTotal,
                    opt => opt.MapFrom(src => src.SurchargesTotal));

            // ============================
            // SUPPLIER (avec collections)
            // ============================
            CreateMap<Supplier, SupplierDto>()
                .ForMember(dest => dest.CreatedDate,
                    opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.Contracts,
                    opt => opt.MapFrom(src => src.Contracts.Where(c => !c.IsDeleted).ToList()))
                .ForMember(dest => dest.TransportSegments,
                    opt => opt.MapFrom(src => src.TransportSegments.Where(ts => !ts.IsDeleted).ToList()));

            // DTO -> Command
            CreateMap<CreateSupplierDto, CreateSupplierCommand>();
            CreateMap<UpdateSupplierDto, UpdateSupplierCommand>();
            CreateMap<CreateContractDto, CreateContractDto>();
            CreateMap<CreateTransportSegmentDto, CreateTransportSegmentDto>();

            // PagedList support
            CreateMap<PagedList<Supplier>, PagedList<SupplierDto>>()
                .ConvertUsing((src, dest, context) =>
                {
                    var items = context.Mapper.Map<List<SupplierDto>>(src.Items);
                    return new PagedList<SupplierDto>(
                        items,
                        src.TotalCount,
                        src.CurrentPage,
                        src.PageSize);
                });
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