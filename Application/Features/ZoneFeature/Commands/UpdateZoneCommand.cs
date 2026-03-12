using Application.Features.ZoneFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.ZoneFeature.Commands
{
    public record UpdateZoneCommand(
         Guid Id,
        string Code,
        string Name,
        string Country,
        string? Region,
        string? Description,
        bool IsActive
    ) : IRequest<ResponseHttp>
    {
        public class UpdateZoneCommandHandler : IRequestHandler<UpdateZoneCommand, ResponseHttp>
        {
            private readonly IZoneRepository _zoneRepository;
            private readonly IMapper _mapper;

            public UpdateZoneCommandHandler(IZoneRepository zoneRepository, IMapper mapper)
            {
                _zoneRepository = zoneRepository;
                _mapper = mapper;
            }

            public async Task<ResponseHttp> Handle(UpdateZoneCommand request, CancellationToken cancellationToken)
            {
                Zone? zone = await _zoneRepository.GetById(request.Id);
                if (zone == null)
                {
                    return new ResponseHttp
                    {
                        Resultat = null,
                        Fail_Messages = "Zone with this Id not found",
                        Status = StatusCodes.Status404NotFound
                    };
                }

                // Utiliser AutoMapper pour mapper les propriétés du command vers l'entité existante
                _mapper.Map(request, zone);

                await _zoneRepository.Update(zone);
                await _zoneRepository.SaveChange(cancellationToken);

                return new ResponseHttp
                {
                    Resultat = new ZoneDto(zone),
                    Fail_Messages = null,
                    Status = StatusCodes.Status200OK
                };
            }
        }
    }
}