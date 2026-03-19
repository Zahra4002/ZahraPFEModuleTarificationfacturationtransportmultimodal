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
        bool? IsActive
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

                await _supplierRepository.UpdateAsync(supplier, cancellationToken);
                await _supplierRepository.SaveChange(cancellationToken);

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