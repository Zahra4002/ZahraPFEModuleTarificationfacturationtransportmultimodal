using Application.Features.SupplierFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.SupplierFeature.Queries
{
    public class GetSupplierByIdQueryHandler : IRequestHandler<GetSupplierByIdQuery, ResponseHttp>
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IMapper _mapper;

        public GetSupplierByIdQueryHandler(ISupplierRepository supplierRepository, IMapper mapper)
        {
            _supplierRepository = supplierRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var supplier = await _supplierRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken);

                if (supplier == null)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status404NotFound,
                        Fail_Messages = $"Fournisseur avec ID {request.Id} non trouvé"
                    };
                }

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