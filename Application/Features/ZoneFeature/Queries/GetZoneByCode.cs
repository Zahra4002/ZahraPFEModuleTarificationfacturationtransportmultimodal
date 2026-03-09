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
    public record GetZoneByCode(string zoneCode)
    :IRequest<ResponseHttp>
    {
        public class GetZoneByCodeHandler : IRequestHandler<GetZoneByCode, ResponseHttp>
        {
            IZoneRepository _zoneRepository;

            public GetZoneByCodeHandler(IZoneRepository zoneRepository)
            {
                _zoneRepository = zoneRepository;
            }
            public async Task<ResponseHttp> Handle(GetZoneByCode request, CancellationToken cancellationToken)
            {
                try
                {
                    var zone = await _zoneRepository.SelectOneAsync(z => z.Code == request.zoneCode);
                    if (zone == null)
                    {
                        return new ResponseHttp                         {
                            Resultat = null,
                            Status = 404,
                            Fail_Messages = "Zone not found"
                        };
                    }
                    else
                    {
                        return new ResponseHttp
                        {
                            Resultat = zone,
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
