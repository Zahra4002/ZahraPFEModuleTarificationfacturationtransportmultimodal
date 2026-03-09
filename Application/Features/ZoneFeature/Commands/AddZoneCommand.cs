using Application.Features.ZoneFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ZoneFeature.Commands
{
    public record AddZoneCommand(
        string Code,
        string Name,
        string Country,
        string? Region,
        string? Description,
        bool IsActive
    ) : IRequest<ResponseHttp>
    {
        public class AddZoneCommandHandler : IRequestHandler<AddZoneCommand, ResponseHttp>
        {
            private readonly IZoneRepository _zoneRepository;
            private readonly IMapper _mapper;


            public AddZoneCommandHandler(IZoneRepository zoneRepository, IMapper mapper)
            {
                _zoneRepository = zoneRepository;
                _mapper = mapper;
            }

            public async Task<ResponseHttp> Handle(AddZoneCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var zone = new Zone
                    {
                        Code = request.Code,
                        Name = request.Name,
                        Country = request.Country,
                        Region = request.Region,
                        Description = request.Description,
                        IsActive = request.IsActive
                    };
                    
                    /*var zone = _mapper.Map<Zone>(request);*/
                    await _zoneRepository.Post(zone);
                    await _zoneRepository.SaveChange(cancellationToken);

                    var zoneDto = new ZoneDto(zone); 

                    return new ResponseHttp
                    {
                        Resultat = zoneDto,
                        Status = StatusCodes.Status200OK,
                    };
                }
                catch (Exception ex)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = ex.Message,
                        Status = StatusCodes.Status500InternalServerError
                    };
                }
            }
        }
    }
}