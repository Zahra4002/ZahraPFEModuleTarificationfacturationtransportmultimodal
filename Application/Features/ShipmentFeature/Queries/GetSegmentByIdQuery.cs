using Application.Features.ShipmentFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.ShipmentFeature.Queries
{
    public record GetSegmentByIdQuery(Guid ShipmentId, Guid SegmentId) : IRequest<ResponseHttp>;

    public class GetSegmentByIdQueryHandler : IRequestHandler<GetSegmentByIdQuery, ResponseHttp>
    {
        private readonly ITransportSegmentRepository _repository;
        private readonly IMapper _mapper;

        public GetSegmentByIdQueryHandler(ITransportSegmentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(GetSegmentByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var segment = await _repository.GetByIdAsync(request.SegmentId, cancellationToken);
                if (segment == null || segment.ShipmentId != request.ShipmentId)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status404NotFound,
                        Fail_Messages = "Segment not found"
                    };
                }

                var dto = new TransportSegmentDto(segment);
                return new ResponseHttp
                {
                    Status = StatusCodes.Status200OK,
                    Resultat = dto
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