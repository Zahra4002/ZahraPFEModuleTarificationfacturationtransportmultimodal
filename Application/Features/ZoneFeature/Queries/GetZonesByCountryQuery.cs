using Application.Features.ZoneFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ZoneFeature.Queries
{
    public record GetZonesByCountryQuery(string country):IRequest<ResponseHttp>
    {
        public class GetZonesByCountryHandler : IRequestHandler<GetZonesByCountryQuery, ResponseHttp>
        {
            private readonly IZoneRepository _zoneRepository;
            public GetZonesByCountryHandler(IZoneRepository zoneRepository)
            {
                _zoneRepository = zoneRepository;
            }
            public async Task<ResponseHttp> Handle(GetZonesByCountryQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var zones = await _zoneRepository.SelectManyAsync(z => z.Country == request.country);
                    
                    if (zones == null || !zones.Any())
                    {
                        return new ResponseHttp
                        {
                            Resultat = null,
                            Status = 404,
                            Fail_Messages = "No zones found for the specified country"
                        };
                    }
                    else
                    {
                        var zoneList = zones.Select(z => new ZoneDto(z));
                        return new ResponseHttp
                        {
                            Resultat = zoneList,
                            Status = 200,
                            Fail_Messages = null
                        };
                    }
                }
                catch (Exception ex)
                {
                    return new ResponseHttp
                    {
                        Resultat = null,
                        Status = 500,
                        Fail_Messages = ex.Message
                    };
                }
            }
        }
    }
}
