using Application.Features.SupplierFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.SupplierFeature.Commands
{
    public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, ResponseHttp>
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IZoneRepository _zoneRepository;
        private readonly IMapper _mapper;

        public CreateSupplierCommandHandler(
            ISupplierRepository supplierRepository,
            IZoneRepository zoneRepository,
            IMapper mapper)
        {
            _supplierRepository = supplierRepository;
            _zoneRepository = zoneRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Vérifier si le code existe déjà
                var existingSupplier = await _supplierRepository.GetByCodeAsync(request.Code);
                if (existingSupplier != null)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Fail_Messages = $"Un fournisseur avec le code {request.Code} existe déjà"
                    };
                }

                var supplier = new Supplier
                {
                    Id = Guid.NewGuid(),
                    Code = request.Code,
                    Name = request.Name,
                    TaxId = request.TaxId,
                    Email = request.Email,
                    Phone = request.Phone,
                    Address = request.Address,
                    DefaultCurrencyCode = request.DefaultCurrencyCode,
                    IsActive = request.IsActive,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "System",
                    Contracts = new List<Contract>(),
                    TransportSegments = new List<TransportSegment>()
                };

                // ✅ Créer les contrats associés
                if (request.Contracts != null)
                {
                    foreach (var contractDto in request.Contracts)
                    {
                        var contract = new Contract
                        {
                            Id = Guid.NewGuid(),
                            ContractNumber = contractDto.ContractNumber,
                            Name = contractDto.Name,
                            Type = (ContractType)contractDto.Type,
                            ValidFrom = contractDto.ValidFrom,
                            ValidTo = contractDto.ValidTo,
                            Terms = contractDto.Terms,
                            GlobalDiscountPercent = contractDto.GlobalDiscountPercent,
                            IsActive = contractDto.IsActive,
                            SupplierId = supplier.Id,
                            CreatedDate = DateTime.UtcNow,
                            CreatedBy = "System"
                        };
                        supplier.Contracts.Add(contract);
                    }
                }

                // ✅ Créer les segments associés
                if (request.TransportSegments != null)
                {
                    foreach (var segmentDto in request.TransportSegments)
                    {
                        // Vérifier que les zones existent si fournies
                        if (segmentDto.ZoneFromId.HasValue)
                        {
                            var zoneFrom = await _zoneRepository.GetByIdAsync(segmentDto.ZoneFromId.Value);
                            if (zoneFrom == null)
                            {
                                return new ResponseHttp
                                {
                                    Status = StatusCodes.Status400BadRequest,
                                    Fail_Messages = $"La zone d'origine avec ID {segmentDto.ZoneFromId} n'existe pas"
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
                                    Fail_Messages = $"La zone de destination avec ID {segmentDto.ZoneToId} n'existe pas"
                                };
                            }
                        }

                        var segment = new TransportSegment
                        {
                            Id = Guid.NewGuid(),
                            Sequence = segmentDto.Sequence,
                            TransportMode = (TransportMode)segmentDto.TransportMode,
                            ZoneFromId = segmentDto.ZoneFromId,
                            ZoneToId = segmentDto.ZoneToId,
                            DistanceKm = segmentDto.DistanceKm,
                            BaseCost = segmentDto.BaseCost,
                            CurrencyCode = segmentDto.CurrencyCode,
                            SurchargesTotal = 0,
                            TotalCost = segmentDto.BaseCost,
                            SupplierId = supplier.Id,
                            CreatedDate = DateTime.UtcNow,
                            CreatedBy = "System"
                        };
                        supplier.TransportSegments.Add(segment);
                    }
                }

                await _supplierRepository.AddAsync(supplier, cancellationToken);
                await _supplierRepository.SaveChangesAsync(cancellationToken);

                var supplierDto = _mapper.Map<SupplierDto>(supplier);

                return new ResponseHttp
                {
                    Resultat = supplierDto,
                    Status = StatusCodes.Status201Created
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