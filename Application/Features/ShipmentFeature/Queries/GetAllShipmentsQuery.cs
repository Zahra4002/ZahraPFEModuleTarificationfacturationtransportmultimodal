using Application.Features.ShipmentFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ShipmentFeature.Queries
{
    public record GetAllShipmentsQuery(
    int? PageNumber,
    int? PageSize,
    string? SortBy,
    bool SortDescending = false,
    string? SearchTerm=null
     
) : IRequest<ResponseHttp>
    {
        public class GetAllShipmentsQueryHandler : IRequestHandler<GetAllShipmentsQuery, ResponseHttp>
        {
            private readonly IShipmentRepository _repository;

            public GetAllShipmentsQueryHandler(IShipmentRepository repository)
            {
                _repository = repository;
            }

            public async Task<ResponseHttp> Handle(GetAllShipmentsQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var shipments = await _repository.GetAllWithTypeAsync(
                        request.PageNumber,
                        request.PageSize,
                        request.SortBy,
                        request.SortDescending,
                        request.SearchTerm,
                        cancellationToken);

                    if (shipments == null || shipments.Count == 0)
                    {
                        return new ResponseHttp
                        {
                            Resultat = null,
                            Status = 404,
                            Fail_Messages = "No Shipments found."
                        };
                    }

                    return new ResponseHttp
                    {
                        Resultat = shipments.Select(s => new ShipmentDto(s)).ToList(),
                        Status = 200,
                        Fail_Messages = null
                    };
                }
                catch (Exception ex)
                {
                    return new ResponseHttp
                    {
                        Resultat = null,
                        Status = 500,
                        Fail_Messages = $"An error occurred while retrieving shipments: {ex.Message}"
                    };
                }
            }
        }
    }
}
