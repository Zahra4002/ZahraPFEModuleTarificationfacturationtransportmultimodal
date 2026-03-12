using Application.Features.SupplierFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.SupplierFeature.Commands
{
    public record UpdateSupplierCommand(
        Guid Id,
        string? Code,
        string? Name,
        string? TaxId,
        string? Email,
        string? Phone,
        string? Address,
        string? DefaultCurrencyCode,
        bool? IsActive,
        List<UpdateContractDto>? Contracts,
        List<UpdateTransportSegmentDto>? TransportSegments
    ) : IRequest<ResponseHttp>;
    public class UpdateSupplierCommandHandler : IRequestHandler<UpdateSupplierCommand, ResponseHttp>
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IZoneRepository _zoneRepository;
        private readonly IMapper _mapper;

        public UpdateSupplierCommandHandler(
            ISupplierRepository supplierRepository,
            IZoneRepository zoneRepository,
            IMapper mapper)
        {
            _supplierRepository = supplierRepository;
            _zoneRepository = zoneRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var supplier = await _supplierRepository
                    .GetByIdWithDetailsAsync(request.Id, cancellationToken);

                if (supplier == null)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status404NotFound,
                        Fail_Messages = $"Fournisseur avec ID {request.Id} non trouvé"
                    };
                }

                // ✅ Vérifier unicité du code AVANT le mapping
                if (!string.IsNullOrEmpty(request.Code) && request.Code != supplier.Code)
                {
                    var existingSupplier = await _supplierRepository.GetByCodeAsync(request.Code);
                    if (existingSupplier != null)
                    {
                        return new ResponseHttp
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Fail_Messages = $"Un fournisseur avec le code {request.Code} existe déjà"
                        };
                    }
                }

                // ✅ Utiliser AutoMapper pour les propriétés simples
                _mapper.Map(request, supplier);

                // ✅ Mettre à jour les métadonnées manuellement
                supplier.ModifiedDate = DateTime.UtcNow;
                supplier.ModifiedBy = "System";

                // =====================================================
                // ✅ Gestion des CONTRATS (inchangée)
                // =====================================================
                if (request.Contracts != null)
                {
                    supplier.Contracts.Clear();

                    foreach (var contractDto in request.Contracts)
                    {
                        supplier.Contracts.Add(new Contract
                        {
                            Id = Guid.NewGuid(),
                            ContractNumber = contractDto.ContractNumber ?? $"CONT-{Guid.NewGuid():N}",
                            Name = contractDto.Name ?? "Nouveau contrat",
                            Type = contractDto.Type.HasValue
                                ? (ContractType)contractDto.Type.Value
                                : ContractType.Standard,
                            ValidFrom = contractDto.ValidFrom ?? DateTime.UtcNow,
                            ValidTo = contractDto.ValidTo ?? DateTime.UtcNow.AddYears(1),
                            Terms = contractDto.Terms,
                            GlobalDiscountPercent = contractDto.GlobalDiscountPercent ?? 0,
                            IsActive = contractDto.IsActive ?? true,
                            SupplierId = supplier.Id,
                            CreatedDate = DateTime.UtcNow,
                            CreatedBy = "System"
                        });
                    }
                }

                // =====================================================
                // ✅ Gestion des SEGMENTS (inchangée)
                // =====================================================
                if (request.TransportSegments != null)
                {
                    supplier.TransportSegments.Clear();

                    foreach (var segmentDto in request.TransportSegments)
                    {
                        // Vérifier existence zones (même code qu'avant)
                        if (segmentDto.ZoneFromId.HasValue)
                        {
                            var zoneFrom = await _zoneRepository.GetByIdAsync(segmentDto.ZoneFromId.Value);
                            if (zoneFrom == null)
                            {
                                return new ResponseHttp
                                {
                                    Status = StatusCodes.Status400BadRequest,
                                    Fail_Messages = $"Zone origine {segmentDto.ZoneFromId} inexistante"
                                };
                            }
                        }

                        if (segmentDto.ZoneToId.HasValue)
                        {
                            var zoneTo = await _zoneRepository.GetByIdAsync(segmentDto.ZoneToId.Value);
                            if (zoneTo == null)
                            {
                                return new ResponseHttp
                                {
                                    Status = StatusCodes.Status400BadRequest,
                                    Fail_Messages = $"Zone destination {segmentDto.ZoneToId} inexistante"
                                };
                            }
                        }

                        supplier.TransportSegments.Add(new TransportSegment
                        {
                            Id = Guid.NewGuid(),
                            Sequence = segmentDto.Sequence ?? 1,
                            TransportMode = segmentDto.TransportMode.HasValue
                                ? (TransportMode)segmentDto.TransportMode.Value
                                : TransportMode.Routier,
                            ZoneFromId = segmentDto.ZoneFromId,
                            ZoneToId = segmentDto.ZoneToId,
                            DistanceKm = segmentDto.DistanceKm,
                            BaseCost = segmentDto.BaseCost ?? 0,
                            CurrencyCode = segmentDto.CurrencyCode ?? "EUR",
                            SurchargesTotal = 0,
                            TotalCost = segmentDto.BaseCost ?? 0,
                            SupplierId = supplier.Id,
                            CreatedDate = DateTime.UtcNow,
                            CreatedBy = "System"
                        });
                    }
                }

                await _supplierRepository.SaveChangesAsync(cancellationToken);

                var supplierDto = _mapper.Map<SupplierDto>(supplier);

                return new ResponseHttp
                {
                    Resultat = supplierDto,
                    Status = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ResponseHttp
                {
                    Fail_Messages = ex.Message,
                    Status = StatusCodes.Status400BadRequest
                };
            }
        }
    }
}