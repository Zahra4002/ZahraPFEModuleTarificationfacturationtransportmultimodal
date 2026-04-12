using Application.Features.ShipmentFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.ShipmentFeature.Queries
{
    public record GetSegmentsByShipmentIdQuery(Guid ShipmentId) : IRequest<ResponseHttp>;

    public class GetSegmentsByShipmentIdQueryHandler : IRequestHandler<GetSegmentsByShipmentIdQuery, ResponseHttp>
    {
        private readonly ITransportSegmentRepository _repository;
        private readonly IMapper _mapper;

        public GetSegmentsByShipmentIdQueryHandler(ITransportSegmentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(GetSegmentsByShipmentIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var segments = await _repository.GetByShipmentIdAsync(request.ShipmentId, cancellationToken);
                var dtos = segments.Select(s => new TransportSegmentDto(s)).ToList();

                return new ResponseHttp
                {
                    Status = StatusCodes.Status200OK,
                    Resultat = dtos
                };
            }
            catch (Exception ex)
            {
                return new ResponseHttp
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Fail_Messages = ex.Message
                };
            }
        }
    }
}