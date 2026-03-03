using Application.Interfaces;
using Application.Setting;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.TariffGridFeature.Commands
{
    public record DeleteTariffLineCommand(Guid GridId, Guid LineId) : IRequest<ResponseHttp>;

    public class DeleteTariffLineCommandHandler : IRequestHandler<DeleteTariffLineCommand, ResponseHttp>
    {
        private readonly ITariffGridRepository _tariffGridRepository;

        public DeleteTariffLineCommandHandler(ITariffGridRepository tariffGridRepository)
        {
            _tariffGridRepository = tariffGridRepository;
        }

        public async Task<ResponseHttp> Handle(DeleteTariffLineCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if grid exists
                var grid = await _tariffGridRepository.GetByIdAsync(request.GridId, cancellationToken);
                if (grid == null)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "Tariff grid not found",
                        Status = StatusCodes.Status404NotFound
                    };
                }

                // Get the line to verify it belongs to this grid
                var line = await _tariffGridRepository.GetLineByIdAsync(request.LineId, cancellationToken);
                if (line == null || line.TariffGridId != request.GridId)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "Tariff line not found in this grid",
                        Status = StatusCodes.Status404NotFound
                    };
                }

                var result = await _tariffGridRepository.DeleteLineAsync(request.LineId);
                if (!result)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "Failed to delete tariff line",
                        Status = StatusCodes.Status400BadRequest
                    };
                }

                await _tariffGridRepository.SaveChange(cancellationToken);

                return new ResponseHttp
                {
                    Status = StatusCodes.Status204NoContent
                };
            }
            catch (Exception ex)
            {
                return new ResponseHttp
                {
                    Fail_Messages = ex.Message,
                    Status = StatusCodes.Status400BadRequest
                };
            }
        }
    }
}