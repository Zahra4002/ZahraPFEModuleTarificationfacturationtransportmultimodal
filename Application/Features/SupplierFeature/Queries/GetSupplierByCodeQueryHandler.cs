using Application.Features.SupplierFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.SupplierFeature.Queries
{
    public record GetSupplierByCodeQuery(string Code) : IRequest<ResponseHttp>;
    public class GetSupplierByCodeQueryHandler : IRequestHandler<GetSupplierByCodeQuery, ResponseHttp>
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IMapper _mapper;

        public GetSupplierByCodeQueryHandler(ISupplierRepository supplierRepository, IMapper mapper)
        {
            _supplierRepository = supplierRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(GetSupplierByCodeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Récupérer le fournisseur par son code
                var supplier = await _supplierRepository.GetByCodeWithDetailsAsync(request.Code, cancellationToken);

                if (supplier == null)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status404NotFound,
                        Fail_Messages = $"Aucun fournisseur trouvé avec le code '{request.Code}'"
                    };
                }

                // 2️⃣ Mapper vers DTO
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