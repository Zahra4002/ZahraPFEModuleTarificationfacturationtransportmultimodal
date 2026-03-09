using Application.Interfaces;
using Application.Setting;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ZoneFeature.Commands
{
    public record class DeleteZoneCommand(
    Guid ZoneId
) : IRequest<ResponseHttp>
    {
        public class DeleteZoneCommandHandler : IRequestHandler<DeleteZoneCommand, ResponseHttp>
        {
            private readonly IZoneRepository _zoneRepository;

            public DeleteZoneCommandHandler(IZoneRepository zoneRepository)
            {
                _zoneRepository = zoneRepository;
            }

            public async Task<ResponseHttp> Handle(DeleteZoneCommand request, CancellationToken cancellationToken)
            {
                var zone = await _zoneRepository.GetByIdAsync(request.ZoneId, cancellationToken);
                if (zone == null)
                {
                    return new ResponseHttp()
                    {
                        Fail_Messages = $"Zone with ID {request.ZoneId} not found.",
                        Status = StatusCodes.Status404NotFound
                    };
                }

                await _zoneRepository.Delete(request.ZoneId);
                await _zoneRepository.SaveChange(cancellationToken);
                return new ResponseHttp()
                {
                    Status = StatusCodes.Status200OK,
                    Fail_Messages = null,
                    Resultat = $"Zone with ID {request.ZoneId} deleted successfully."
                };
            }
        }
    }

}
    