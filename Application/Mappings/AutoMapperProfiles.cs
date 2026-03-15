using Application.Features.ClientFeature.Commands;
using Application.Features.ContractFeature.Commands;
using Application.Features.ContractFeature.Dtos;
using Application.Features.CurrencyFeature.Commands;
using Application.Features.CurrencyFeature.Dtos;
using Application.Features.InvoiceFeature.Commands;
using Application.Features.InvoiceFeature.Dtos;
using Application.Features.QuoteFeature.Commands;
using Application.Features.QuoteFeature.Dtos;
using Application.Features.ShipmentFeature.Commands;
using Application.Features.ShipmentFeature.Dtos;
using Application.Features.SupplierFeature.Commands;
using Application.Features.SupplierFeature.Dtos;
using Application.Features.SurchargeFeature.Dtos;
using Application.Features.TariffGridFeature.Dtos;
using Application.Features.TestFeature.Commands;
using Application.Features.TestFeature.Dtos;
using Application.Features.UserFeature.Dtos;
using Application.Features.ZoneFeature.Commands;
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
            // ZONE MAPPINGS
            // ============================
            CreateMap<AddZoneCommand, Zone>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedById, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedDate, opt => opt.Ignore())
                .ForMember(dest => dest.TariffLinesFrom, opt => opt.Ignore())
                .ForMember(dest => dest.TariffLinesTo, opt => opt.Ignore());
            // Ajouter ce mapping pour UpdateZoneCommand
            CreateMap<UpdateZoneCommand, Zone>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedById, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedDate, opt => opt.Ignore())
                .ForMember(dest => dest.TariffLinesFrom, opt => opt.Ignore())
                .ForMember(dest => dest.TariffLinesTo, opt => opt.Ignore());

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
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                 .ForMember(dest => dest.TransportSegmentId, opt => opt.MapFrom(src => src.TransportSegmentId))
                 .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                 .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                 .ForMember(dest => dest.UnitPriceHT, opt => opt.MapFrom(src => src.UnitPriceHT))
                 .ForMember(dest => dest.VATRate, opt => opt.MapFrom(src => src.VATRate))
                   .ForMember(dest => dest.DiscountPercent, opt => opt.MapFrom(src => src.DiscountPercent))
                   .ForMember(dest => dest.TotalHT, opt => opt.MapFrom(src => src.TotalHT))
             .ForMember(dest => dest.TotalVAT, opt => opt.MapFrom(src => src.TotalVAT))
             .ForMember(dest => dest.TotalTTC, opt => opt.MapFrom(src => src.TotalTTC));



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
              .ForMember(dest => dest.ClientName,
                  opt => opt.MapFrom(src => src.Client.Name))

              .ForMember(dest => dest.SupplierName,
                opt => opt.MapFrom(src => src.Supplier.Name))
     
             .ForMember(dest => dest.ShipmentNumber,
                opt => opt.MapFrom(src => src.Shipment.ShipmentNumber))

            .ForMember(dest => dest.CurrencyCode,
                opt => opt.MapFrom(src => src.Currency.Code))

           .ForMember(dest => dest.Lines,
                 opt => opt.MapFrom(src => src.Lines));

            


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
            // TRANSPORT SEGMENT DTO
            // ============================
            CreateMap<TransportSegment, Application.Features.SupplierFeature.Dtos.TransportSegmentDto>()
                
                .ForMember(dest => dest.TransportModeName,
                    opt => opt.MapFrom(src => src.TransportMode.ToString()));
           

            // ============================
            // SUPPLIER MAPPINGS
            // ============================

            // Entity -> DTO
            CreateMap<Supplier, SupplierDto>()
                .ForMember(dest => dest.CreatedDate,
                    opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.Contracts,
                    opt => opt.MapFrom(src => src.Contracts.Where(c => !c.IsDeleted).ToList()))
                .ForMember(dest => dest.TransportSegments,
                    opt => opt.MapFrom(src => src.TransportSegments.Where(ts => !ts.IsDeleted).ToList()));

            // DTO -> Command
            CreateMap<CreateSupplierDto, CreateSupplierCommand>();

            // Command -> Entity (pour Create)
            CreateMap<CreateSupplierCommand, Supplier>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedDate, opt => opt.Ignore());
            CreateMap<Invoice, InvoiceSummaryDTO>();
            CreateMap<Contract, ContractSummaryDto>();
            CreateMap<Contract, ContractDto>();
            CreateMap<TransportSegment, Features.SupplierFeature.Dtos.TransportSegmentDto>()
                .ForMember(dest => dest.TransportModeName,
                    opt => opt.MapFrom(src => src.TransportMode.ToString()));
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
            // ============================
            // UPDATE SUPPLIER COMMAND
            // ============================
            CreateMap<UpdateSupplierCommand, Supplier>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())  // On garde l'ID existant
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => "System"))
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedById, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedDate, opt => opt.Ignore())
                .ForMember(dest => dest.Contracts, opt => opt.Ignore())
                .ForMember(dest => dest.TransportSegments, opt => opt.Ignore());
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

            // ============================
            // CLIENT MAPPINGS
            // ============================


            // Command -> Entity (for create)
            CreateMap<AddClientCommandNew,Client>()
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedById, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedDate, opt => opt.Ignore())           
                .ForMember(dest => dest.BullingAddress, opt => opt.MapFrom(src => src.bullingAddress))
                .ForMember(dest => dest.ShippingAddress, opt => opt.MapFrom(src => src.shippingAddress))
                .ForMember(dest => dest.DefaultCurrencyCode, opt => opt.MapFrom(src => src.defaultCurrencyCode))
                .ForMember(dest => dest.CreditLimit, opt => opt.MapFrom(src => src.creditLimit))
                .ForMember(dest => dest.PaymentTermDays, opt => opt.MapFrom(src => src.paymenttermDays))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.isActive))
                .ForMember(dest => dest.Contracts, opt => opt.Ignore())
                .ForMember(dest => dest.invoices, opt => opt.Ignore())
                .ForMember(dest => dest.shipments, opt => opt.Ignore());
                
            // Command -> Entity (for update)

            CreateMap<UpdateClientCommandNew, Client>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ClientId))
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedById, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedDate, opt => opt.Ignore())
                .ForMember(dest => dest.BullingAddress, opt => opt.MapFrom(src => src.bullingAddress))
                .ForMember(dest => dest.ShippingAddress, opt => opt.MapFrom(src => src.shippingAddress))
                .ForMember(dest => dest.DefaultCurrencyCode, opt => opt.MapFrom(src => src.defaultCurrencyCode))
                .ForMember(dest => dest.CreditLimit, opt => opt.MapFrom(src => src.creditLimit))
                .ForMember(dest => dest.PaymentTermDays, opt => opt.MapFrom(src => src.paymenttermDays))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.isActive))
                .ForMember(dest => dest.Contracts, opt => opt.Ignore())
                .ForMember(dest => dest.invoices, opt => opt.Ignore())
                .ForMember(dest => dest.shipments, opt => opt.Ignore());

            // Entity -> DTO (for responses)
            CreateMap<Client, ClientDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.TaxId, opt => opt.MapFrom(src => src.TaxId))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.BullingAddress, opt => opt.MapFrom(src => src.BullingAddress))
                .ForMember(dest => dest.ShippingAddress, opt => opt.MapFrom(src => src.ShippingAddress))
                .ForMember(dest => dest.DefaultCurrencyCode, opt => opt.MapFrom(src => src.DefaultCurrencyCode))
                .ForMember(dest => dest.CreditLimit, opt => opt.MapFrom(src => src.CreditLimit))
                .ForMember(dest => dest.PaymentTermDays, opt => opt.MapFrom(src => src.PaymentTermDays))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

            // Entity -> DTO
            CreateMap<Client, ClientDTO>();
                

            // PagedList support pour Client
            CreateMap<PagedList<Client>, PagedList<ClientDTO>>()
                .ConvertUsing((src, dest, context) =>
                {
                    var items = context.Mapper.Map<List<ClientDTO>>(src.Items);
                    return new PagedList<ClientDTO>(
                        items,
                        src.TotalCount,
                        src.CurrentPage,
                        src.PageSize);
                });


            // ============================
            // CONTRACT MAPPINGS
            // ============================

// Command → Entity (pour la création)
CreateMap<AddContractCommandNew, Contract>()
    .ForMember(dest => dest.Id, opt => opt.Ignore())
    .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
    .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
    .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
    .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
    .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
    .ForMember(dest => dest.ModifiedById, opt => opt.Ignore())
    .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
    .ForMember(dest => dest.DeletedDate, opt => opt.Ignore())
    .ForMember(dest => dest.GlobalDiscountPercent, opt => opt.Ignore())
    .ForMember(dest => dest.Client, opt => opt.Ignore())
    .ForMember(dest => dest.Supplier, opt => opt.Ignore())
    .ForMember(dest => dest.ContractPricings, opt => opt.Ignore())
    // Mapping spécifique pour les noms différents
    .ForMember(dest => dest.ContractNumber, opt => opt.MapFrom(src => src.contractNumber))
    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.name))
    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
    .ForMember(dest => dest.ValidFrom, opt => opt.MapFrom(src => src.validForm))
    .ForMember(dest => dest.ValidTo, opt => opt.MapFrom(src => src.validTo))
    .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.ClientId))
    .ForMember(dest => dest.SupplierId, opt => opt.MapFrom(src => src.SupplierId))
    .ForMember(dest => dest.Terms, opt => opt.MapFrom(src => src.terms))
    .ForMember(dest => dest.TermsAccepted, opt => opt.MapFrom(src => src.termsAccepted))
    .ForMember(dest => dest.TermsAcceptedAt, opt => opt.MapFrom(src => src.termsAccptedAt))
    .ForMember(dest => dest.MinimumVolume, opt => opt.MapFrom(src => src.minimumVolume))
    .ForMember(dest => dest.MinimumVolumeUnit, opt => opt.MapFrom(src => src.minimumVolumeUnit))
    .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
    .ForMember(dest => dest.AutoRenew, opt => opt.MapFrom(src => src.AutoRenew))
    .ForMember(dest => dest.RenewalNoticeDays, opt => opt.MapFrom(src => src.RenewalNoticeDays));

// Entity → DTO (pour les réponses)
CreateMap<Contract, ContractDTO>()
    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
    .ForMember(dest => dest.ContractNumber, opt => opt.MapFrom(src => src.ContractNumber))
    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
    .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.ClientId))
    .ForMember(dest => dest.SupplierId, opt => opt.MapFrom(src => src.SupplierId))
    .ForMember(dest => dest.ValidFrom, opt => opt.MapFrom(src => src.ValidFrom))
    .ForMember(dest => dest.ValidTo, opt => opt.MapFrom(src => src.ValidTo))
    .ForMember(dest => dest.Terms, opt => opt.MapFrom(src => src.Terms))
    .ForMember(dest => dest.TermsAccepted, opt => opt.MapFrom(src => src.TermsAccepted))
    .ForMember(dest => dest.TermsAcceptedAt, opt => opt.MapFrom(src => src.TermsAcceptedAt))
    .ForMember(dest => dest.GlobalDiscountPercent, opt => opt.MapFrom(src => src.GlobalDiscountPercent))
    .ForMember(dest => dest.MinimumVolume, opt => opt.MapFrom(src => src.MinimumVolume))
    .ForMember(dest => dest.MinimumVolumeUnit, opt => opt.MapFrom(src => src.MinimumVolumeUnit))
    .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
    .ForMember(dest => dest.AutoRenew, opt => opt.MapFrom(src => src.AutoRenew))
    .ForMember(dest => dest.RenewalNoticeDays, opt => opt.MapFrom(src => src.RenewalNoticeDays));

// PagedList support pour Contract
CreateMap<PagedList<Contract>, PagedList<ContractDTO>>()
    .ConvertUsing((src, dest, context) =>
    {
        var items = context.Mapper.Map<List<ContractDTO>>(src.Items);
        return new PagedList<ContractDTO>(
            items,
            src.TotalCount,
            src.CurrentPage,
            src.PageSize);
    });

            //Command -> Entity (pour mise à jour) 
            CreateMap<UpdateContractPricingCommand, ContractPricing>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PricingId))
                .ForMember(dest => dest.Contract, opt => opt.Ignore())
                .ForMember(dest => dest.ZoneFrom, opt => opt.Ignore())
                .ForMember(dest => dest.ZoneTo, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedById, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ContractId, opt => opt.MapFrom(src => src.ContractId))
                .ForMember(dest => dest.ZoneFromId, opt => opt.MapFrom(src => src.ZoneFromId))
                .ForMember(dest => dest.ZoneToId, opt => opt.MapFrom(src => src.ZoneToId))
                .ForMember(dest => dest.TransportMode, opt => opt.MapFrom(src => src.TransportMode))
                .ForMember(dest => dest.UseFixedPrice, opt => opt.MapFrom(src => src.UseFixedPrice))
                .ForMember(dest => dest.FixedPrice, opt => opt.MapFrom(src => src.FixedPrice))
                .ForMember(dest => dest.DiscountPercent, opt => opt.MapFrom(src => src.DiscountPercent))
                .ForMember(dest => dest.VolumeThreshold, opt => opt.MapFrom(src => src.VolumeThreshold))
                .ForMember(dest => dest.VolumeDiscountPercent, opt => opt.MapFrom(src => src.VolumeDiscountPercent))
                .ForMember(dest => dest.CurrencyCode, opt => opt.MapFrom(src => src.CurrencyCode))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => DateTime.UtcNow));






            // Command → Entity (pour la mise à jour)
            CreateMap<UpdateContractCommandNew, Contract>()
    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ContractId))
    .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
    .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
    .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
    .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
    .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
    .ForMember(dest => dest.ModifiedById, opt => opt.Ignore())
    .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
    .ForMember(dest => dest.DeletedDate, opt => opt.Ignore())
    .ForMember(dest => dest.GlobalDiscountPercent, opt => opt.Ignore())
    .ForMember(dest => dest.Client, opt => opt.Ignore())
    .ForMember(dest => dest.Supplier, opt => opt.Ignore())
    .ForMember(dest => dest.ContractPricings, opt => opt.Ignore())
    // Mapping spécifique pour les noms différents
    .ForMember(dest => dest.ContractNumber, opt => opt.MapFrom(src => src.contractNumber))
    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.name))
    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
    .ForMember(dest => dest.ValidFrom, opt => opt.MapFrom(src => src.validForm))
    .ForMember(dest => dest.ValidTo, opt => opt.MapFrom(src => src.validTo))
    .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.ClientId))
    .ForMember(dest => dest.SupplierId, opt => opt.MapFrom(src => src.SupplierId))
    .ForMember(dest => dest.Terms, opt => opt.MapFrom(src => src.terms))
    .ForMember(dest => dest.TermsAccepted, opt => opt.MapFrom(src => src.termsAccepted))
    .ForMember(dest => dest.TermsAcceptedAt, opt => opt.MapFrom(src => src.termsAccptedAt))
    .ForMember(dest => dest.MinimumVolume, opt => opt.MapFrom(src => src.minimumVolume))
    .ForMember(dest => dest.MinimumVolumeUnit, opt => opt.MapFrom(src => src.minimumVolumeUnit))
    .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
    .ForMember(dest => dest.AutoRenew, opt => opt.MapFrom(src => src.AutoRenew))
    .ForMember(dest => dest.RenewalNoticeDays, opt => opt.MapFrom(src => src.RenewalNoticeDays));

            CreateMap<ContractPricing, ContractPricingDto>()
    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
    .ForMember(dest => dest.ContractId, opt => opt.MapFrom(src => src.ContractId))
    .ForMember(dest => dest.ZoneFromId, opt => opt.MapFrom(src => src.ZoneFromId))
    .ForMember(dest => dest.ZoneToId, opt => opt.MapFrom(src => src.ZoneToId))
    .ForMember(dest => dest.TransportMode, opt => opt.MapFrom(src => src.TransportMode))
    .ForMember(dest => dest.UseFixedPrice, opt => opt.MapFrom(src => src.UseFixedPrice))
    .ForMember(dest => dest.FixedPrice, opt => opt.MapFrom(src => src.FixedPrice))
    .ForMember(dest => dest.DiscountPercent, opt => opt.MapFrom(src => src.DiscountPercent))
    .ForMember(dest => dest.VolumeThreshold, opt => opt.MapFrom(src => src.VolumeThreshold))
    .ForMember(dest => dest.VolumeDiscountPercent, opt => opt.MapFrom(src => src.VolumeDiscountPercent))
    .ForMember(dest => dest.CurrencyCode, opt => opt.MapFrom(src => src.CurrencyCode))
    .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
    .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
    .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate))
    .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
    .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.ModifiedBy))




    // Navigation properties avec null-safety
    .ForMember(dest => dest.ZoneFromName,
        opt => opt.MapFrom(src => src.ZoneFrom != null ? src.ZoneFrom.Name : null))
    .ForMember(dest => dest.ZoneToName,
        opt => opt.MapFrom(src => src.ZoneTo != null ? src.ZoneTo.Name : null))
    .ForMember(dest => dest.TransportModeDisplayName,
        opt => opt.MapFrom(src => src.TransportMode.HasValue ? src.TransportMode.ToString() : null));

            // Mapping inverse (DTO -> Entity) si nécessaire
            CreateMap<ContractPricingDto, ContractPricing>()
                .ForMember(dest => dest.Contract, opt => opt.Ignore())
                .ForMember(dest => dest.ZoneFrom, opt => opt.Ignore())
                .ForMember(dest => dest.ZoneTo, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedById, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedDate, opt => opt.Ignore());

            // ============================
            // CURRENCY MAPPINGS
            // ============================
            CreateMap<AddCurrencyCommand, Currency>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedById, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ExchangeRatesFrom, opt => opt.Ignore())
                .ForMember(dest => dest.ExchangeRatesTo, opt => opt.Ignore())
                .ForMember(dest => dest.Invoices, opt => opt.Ignore());

            CreateMap<UpdateCurrencyCommand, Currency>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedById, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ExchangeRatesFrom, opt => opt.Ignore())
                .ForMember(dest => dest.ExchangeRatesTo, opt => opt.Ignore())
                .ForMember(dest => dest.Invoices, opt => opt.Ignore());

            CreateMap<Currency, CurrencyDto>();

            // ============================
            // EXCHANGE RATE MAPPINGS
            // ============================
            CreateMap<AddExchangeRateCommand, ExchangeRate>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FromCurrencyId, opt => opt.MapFrom(src => src.fromCurrencyId))
                .ForMember(dest => dest.ToCurrencyId, opt => opt.MapFrom(src => src.toCurrencyId))
                .ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.rate))
                .ForMember(dest => dest.EffectiveDate, opt => opt.MapFrom(src => src.effectiveDate))
                .ForMember(dest => dest.ExpiryDate, opt => opt.MapFrom(src => src.expiryDate))
                .ForMember(dest => dest.Source, opt => opt.MapFrom(src => src.source))
                .ForMember(dest => dest.FromCurrency, opt => opt.Ignore())
                .ForMember(dest => dest.ToCurrency, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedById, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedDate, opt => opt.Ignore());

            CreateMap<ExchangeRate, RateDto>()
    .ConstructUsing(src => new RateDto()) // Force l'utilisation du constructeur sans paramètres
    .ForMember(dest => dest.FromCurrencyCode,
        opt => opt.MapFrom(src => src.FromCurrency != null ? src.FromCurrency.Code : string.Empty))
    .ForMember(dest => dest.ToCurrencyCode,
        opt => opt.MapFrom(src => src.ToCurrency != null ? src.ToCurrency.Code : string.Empty));

            // ============================
            // CONTRACT MAPPINGS
            // ============================

            // =============================
            // Shipment Mappings
            // =============================
            CreateMap<Shipment, ShipmentDto>();

            

            // Mapping pour AddressDto <-> Address
            CreateMap<AddressDto,Domain.ValueObjects. Address>();
            CreateMap<Domain.ValueObjects.Address, AddressDto>();

            // Command -> Entity mappings
            CreateMap<AddShipmentCommand, Shipment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedById, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedDate, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore()) // Status sera défini par défaut
                .ForMember(dest => dest.TrackingNumber, opt => opt.Ignore()) // Se génère plus tard
                .ForMember(dest => dest.Client, opt => opt.Ignore())
                .ForMember(dest => dest.Quote, opt => opt.Ignore())
                .ForMember(dest => dest.Segments, opt => opt.Ignore())
                .ForMember(dest => dest.Invoices, opt => opt.Ignore());

            CreateMap<UpdateShipmentCommand, Shipment>()
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedById, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedDate, opt => opt.Ignore())
                .ForMember(dest => dest.Client, opt => opt.Ignore())
                .ForMember(dest => dest.Quote, opt => opt.Ignore())
                .ForMember(dest => dest.Segments, opt => opt.Ignore())
                .ForMember(dest => dest.Invoices, opt => opt.Ignore());


            CreateMap<TransportSegment, Features.ShipmentFeature.Dtos.TransportSegmentDto>();
            CreateMap<Features.ShipmentFeature.Dtos.TransportSegmentDto, TransportSegment>();

            CreateMap<AddSegmentToShipementCommand, TransportSegment>()
    .ForMember(dest => dest.Id, opt => opt.Ignore())
    .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
    .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
    .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
    .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
    .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
    .ForMember(dest => dest.ModifiedById, opt => opt.Ignore())
    .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
    .ForMember(dest => dest.DeletedDate, opt => opt.Ignore())
    .ForMember(dest => dest.Sequence, opt => opt.Ignore())
    .ForMember(dest => dest.SurchargesTotal, opt => opt.Ignore())
    .ForMember(dest => dest.TotalCost, opt => opt.Ignore())
    .ForMember(dest => dest.Supplier, opt => opt.Ignore())
    .ForMember(dest => dest.ZoneFromId, opt => opt.MapFrom(src => src.ZoneFromId))
    .ForMember(dest => dest.ZoneToId, opt => opt.MapFrom(src => src.ZoneToId))
    .ForMember(dest => dest.ZoneFrom, opt => opt.Ignore())
    .ForMember(dest => dest.ZoneTo, opt => opt.Ignore());

            CreateMap<UpdateSegmentOfShipementCommand,TransportSegment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedById, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedDate, opt => opt.Ignore())
                .ForMember(dest => dest.Sequence, opt => opt.Ignore())
                .ForMember(dest => dest.SurchargesTotal, opt => opt.Ignore())
                .ForMember(dest => dest.TotalCost, opt => opt.Ignore())
                .ForMember(dest => dest.Supplier, opt => opt.Ignore())
                .ForMember(dest => dest.ZoneFrom, opt => opt.Ignore())
                .ForMember(dest => dest.ZoneTo, opt => opt.Ignore());




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