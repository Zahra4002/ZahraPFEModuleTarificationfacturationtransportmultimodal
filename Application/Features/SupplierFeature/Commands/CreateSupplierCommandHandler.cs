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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.SupplierFeature.Commands
{
    public record CreateSupplierCommand(
        string Code,
        string Name,
        string? TaxId,
        string? Email,
        string? Phone,
        string Address,
        string DefaultCurrencyCode,
        bool IsActive

    ) : IRequest<ResponseHttp>;

    public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, ResponseHttp>
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IContractRepository _contractRepository;
        private readonly ITransportSegmentRepository _transportSegmentRepository;
        private readonly IMapper _mapper;

        public CreateSupplierCommandHandler(
            ISupplierRepository supplierRepository,
            IContractRepository contractRepository,
            ITransportSegmentRepository transportSegmentRepository,
            IMapper mapper)
        {
            _supplierRepository = supplierRepository;
            _contractRepository = contractRepository;
            _transportSegmentRepository = transportSegmentRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Vérifier si le code existe déjà
                var existingSupplier = await _supplierRepository.GetByCodeAsync(request.Code);
                if (existingSupplier != null)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Fail_Messages = $"Un fournisseur avec le code {request.Code} existe déjà"
                    };
                }

                // 2️⃣ Créer le fournisseur
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

                };

                // 3️⃣ Ajouter les contrats existants par leurs IDs


                // 4️⃣ Ajouter les segments existants par leurs IDs


                // 5️⃣ Sauvegarder le fournisseur (cela sauvegardera aussi les relations)
                await _supplierRepository.AddAsync(supplier, cancellationToken);
                await _supplierRepository.SaveChangesAsync(cancellationToken);

                // 6️⃣ Retourner le fournisseur avec ses contrats et segments
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