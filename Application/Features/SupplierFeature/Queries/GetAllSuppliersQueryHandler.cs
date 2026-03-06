using Application.Features.SupplierFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.SupplierFeature.Queries
{
    public class GetAllSuppliersQueryHandler : IRequestHandler<GetAllSuppliersQuery, ResponseHttp>
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IMapper _mapper;

        public GetAllSuppliersQueryHandler(ISupplierRepository supplierRepository, IMapper mapper)
        {
            _supplierRepository = supplierRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(GetAllSuppliersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var pagedSuppliers = await _supplierRepository.GetAllWithDetailsAsync(
                    request.PageNumber,
                    request.PageSize,
                    cancellationToken);

                if (pagedSuppliers == null || !pagedSuppliers.Items.Any())
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "Aucun fournisseur trouvé",
                        Status = StatusCodes.Status404NotFound
                    };
                }

                var suppliersToReturn = _mapper.Map<PagedList<SupplierDto>>(pagedSuppliers);

                return new ResponseHttp
                {
                    Resultat = suppliersToReturn,
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