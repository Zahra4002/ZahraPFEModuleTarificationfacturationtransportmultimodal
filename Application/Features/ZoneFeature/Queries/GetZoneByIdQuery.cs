using Application.Features.ZoneFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ZoneFeature.Queries
{
    public record GetZoneByIdQuery(Guid id) : IRequest<ResponseHttp>
    {
        public class GetZoneByIdQueryHandler : IRequestHandler<GetZoneByIdQuery, ResponseHttp>
        {
            private readonly IZoneRepository _zoneRepository;
            public GetZoneByIdQueryHandler(IZoneRepository zoneRepository)
            {
                _zoneRepository = zoneRepository;
            }
            public async Task<ResponseHttp> Handle(GetZoneByIdQuery request, CancellationToken cancellationToken)
            {
                var zone = await _zoneRepository.GetById(request.id);

                if (zone == null)
                {
                    return new ResponseHttp
                    {
                        Resultat = null,
                        Status = StatusCodes.Status404NotFound,
                        Fail_Messages = $"Zone with ID {request.id} not found."
                    };
                }

                return new ResponseHttp
                {
                    Resultat = new ZoneDto(zone),
                    Status = StatusCodes.Status200OK,
                    Fail_Messages = null
                };
            }
        }
    }
}
