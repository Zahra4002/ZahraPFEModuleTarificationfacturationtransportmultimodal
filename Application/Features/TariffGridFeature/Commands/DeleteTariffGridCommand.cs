// Application/Features/TariffGridFeature/Commands/DeleteTariffGridCommand.cs
using Application.Interfaces;
using Application.Setting;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.TariffGridFeature.Commands
{
    public record DeleteTariffGridCommand(Guid Id) : IRequest<ResponseHttp>;

    public class DeleteTariffGridCommandHandler : IRequestHandler<DeleteTariffGridCommand, ResponseHttp>
    {
        private readonly ITariffGridRepository _tariffGridRepository;

        public DeleteTariffGridCommandHandler(ITariffGridRepository tariffGridRepository)
        {
            _tariffGridRepository = tariffGridRepository;
        }

        public async Task<ResponseHttp> Handle(DeleteTariffGridCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var tariffGrid = await _tariffGridRepository.GetByIdAsync(request.Id, cancellationToken);
                if (tariffGrid == null)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "Tariff grid not found",
                        Status = StatusCodes.Status404NotFound
                    };
                }

                // Check if grid has lines - maybe prevent deletion or cascade?
                var lines = await _tariffGridRepository.GetLinesByGridIdAsync(request.Id, cancellationToken);
                if (lines.Any())
                {
                    // Option 1: Prevent deletion
                    // return new ResponseHttp
                    // {
                    //     Fail_Messages = "Cannot delete tariff grid with existing lines",
                    //     Status = StatusCodes.Status400BadRequest
                    // };

                    // Option 2: Soft delete all lines first
                    foreach (var line in lines)
                    {
                        await _tariffGridRepository.DeleteLineAsync(line.Id);
                    }
                }

                await _tariffGridRepository.SoftDelete(request.Id);
                await _tariffGridRepository.SaveChange(cancellationToken);

                return new ResponseHttp
                {
                    Status = StatusCodes.Status200OK,
                    Resultat = true
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