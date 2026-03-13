using Application.Features.ShipmentFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.ShipmentFeature.Queries
{
    public record GetShipmntById(Guid ClientId) : IRequest<ResponseHttp>
    {
        public class GetShipmentByIdHandler : IRequestHandler<GetShipmntById, ResponseHttp>
        {
        private readonly IShipmentRepository _shipementRepository;
        private readonly IMapper _mapper;
            private readonly ILogger<GetShipmentByIdHandler> _logger;
            public GetShipmentByIdHandler(IShipmentRepository shipementRepository, IMapper mapper)
        {
            _shipementRepository = shipementRepository; 
            _mapper = mapper;
        }

            public async Task<ResponseHttp> Handle(GetShipmntById request, CancellationToken cancellationToken)
            {
                
                try
                {
                    _logger.LogInformation("Handling GetShipmntById for ClientId: {ClientId}", request.ClientId);

                    var exist = await _shipementRepository.IsExitAsync(request.ClientId);
                    if (exist == null || !exist.Any())
                    {
                        return new ResponseHttp
                        {
                            Status = 404,
                            Fail_Messages = "Client not found.",
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
                        Resultat = _mapper != null ? _mapper.Map<List<ShipmentDto>>(shipments) : shipments
                    };



                }
                catch (Exception ex)
                {
                    return new ResponseHttp
                    {
                        Status = 500,
                        Fail_Messages = $"Error: {ex.Message} | {ex.StackTrace}",
                        Resultat = null
                    };
                }
            }
        }


    }
}
