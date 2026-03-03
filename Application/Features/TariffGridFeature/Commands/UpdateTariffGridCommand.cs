// Application/Features/TariffGridFeature/Commands/UpdateTariffGridCommand.cs
using Application.Features.TariffGridFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.TariffGridFeature.Commands
{
    public record UpdateTariffGridCommand(Guid Id, UpdateTariffGridDTO TariffGridDto) : IRequest<ResponseHttp>;

    public class UpdateTariffGridCommandHandler : IRequestHandler<UpdateTariffGridCommand, ResponseHttp>
    {
        private readonly ITariffGridRepository _tariffGridRepository;
        private readonly IMapper _mapper;

        public UpdateTariffGridCommandHandler(ITariffGridRepository tariffGridRepository, IMapper mapper)
        {
            _tariffGridRepository = tariffGridRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(UpdateTariffGridCommand request, CancellationToken cancellationToken)
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

                // Check if grid is editable (not published/archived in a real scenario)
                // For now, we'll allow updates

                tariffGrid.Name = request.TariffGridDto.Name;
                tariffGrid.Description = request.TariffGridDto.Description;
                tariffGrid.ValidFrom = request.TariffGridDto.ValidFrom;
                tariffGrid.ValidTo = request.TariffGridDto.ValidTo;
                tariffGrid.IsActive = request.TariffGridDto.IsActive;
                tariffGrid.CurrencyCode = request.TariffGridDto.CurrencyCode;
                tariffGrid.ModifiedDate = DateTime.UtcNow;

                await _tariffGridRepository.Update(tariffGrid);
                await _tariffGridRepository.SaveChange(cancellationToken);

                // Get updated grid with line count
                var updatedGrid = await _tariffGridRepository.GetByIdAsync(request.Id, cancellationToken);
                var lineCount = await _tariffGridRepository.GetLinesByGridIdAsync(request.Id, cancellationToken);

                var result = _mapper.Map<TariffGridDTO>(updatedGrid);
                result.TariffLinesCount = lineCount.Count();

                return new ResponseHttp
                {
                    Resultat = result,
                    Status = StatusCodes.Status200OK
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