using Application.Interfaces;
using Application.Setting;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ShipmentFeature.Commands
{
        public record DeleteSegmentOfShipementCommand(
            Guid ShipmentId,
            Guid SegmentId
        ) : IRequest<ResponseHttp>
        {
            public class DeleteSegmentOfShipementCommandHandler : IRequestHandler<DeleteSegmentOfShipementCommand, ResponseHttp>
            {
                private readonly ITransportSegmentRepository _transportSegmentRepository;

                public DeleteSegmentOfShipementCommandHandler(ITransportSegmentRepository transportSegmentRepository)
                {
                    _transportSegmentRepository = transportSegmentRepository;
                }

                public async Task<ResponseHttp> Handle(DeleteSegmentOfShipementCommand request, CancellationToken cancellationToken)
                {
                    try
                    {
                        var validationResult = await _transportSegmentRepository.IsExitAsync(request.ShipmentId, request.SegmentId);
                        if (validationResult.ContainsKey(false))
                        {
                            return new ResponseHttp
                            {
                                Resultat = null,
                                Status = 404,
                                Fail_Messages = validationResult[false]
                            };
                        }

                        await _transportSegmentRepository.Delete(request.SegmentId);
                        await _transportSegmentRepository.SaveChange(cancellationToken);

                        return new ResponseHttp
                        {
                            Resultat = null,
                            Status = 204,
                            Fail_Messages = null
                        };
                    }
                    catch (Exception ex)
                    {
                        return new ResponseHttp
                        {
                            Resultat = null,
                            Status = 500,
                            Fail_Messages = $"An error occurred while updating the segment: {ex.Message}"
                        };
                    }
                }
            }
        }
    }
