using Application.Interfaces;
using Application.Setting;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ShipmentFeature.Queries
{
    public record GetShipmntById(Guid ClientId) : IRequest<ResponseHttp>
    {
        public class GetShipmentByIdHandler : IRequestHandler<GetShipmntById, ResponseHttp>
        {
            private readonly IShipmentRepository _shipementRepository;
            public GetShipmentByIdHandler(IShipmentRepository shipementRepository)
            {
                _shipementRepository = shipementRepository;
            }

            public async Task<ResponseHttp> Handle(GetShipmntById request, CancellationToken cancellationToken)
            {
                var exist = await _shipementRepository.IsExitAsync(request.ClientId);
                var existsKey = exist.Keys.First();
                var message = exist.Values.First();

                if (!existsKey)
                {
                    return new ResponseHttp
                    {
                        Status = 404,
                        Fail_Messages = message,
                        Resultat = null
                    };
                }

                var shipments = await _shipementRepository.SelectManyAsync(s => s.ClientId == request.ClientId, cancellationToken);

                if (shipments == null || shipments.Count == 0)
                {
                    return new ResponseHttp
                    {
                        Status = 404,
                        Fail_Messages = "No shipments found for the specified client.",
                        Resultat = null
                    };
                }

                // Map to DTOs if needed, here assumed as direct return
                return new ResponseHttp
                {
                    Status = 200,
                    Fail_Messages = "Shipments retrieved successfully.",
                    Resultat = shipments
                };  
            }
        }


    }
}
