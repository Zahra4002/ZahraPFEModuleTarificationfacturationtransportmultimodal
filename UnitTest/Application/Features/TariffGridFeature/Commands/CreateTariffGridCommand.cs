// Application/Features/TariffGridFeature/Commands/CreateTariffGridCommand.cs
using Application.Features.TariffGridFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.TariffGridFeature.Commands
{
    public record CreateTariffGridCommand(CreateTariffGridDTO TariffGridDto) : IRequest<ResponseHttp>;

    public class CreateTariffGridCommandHandler : IRequestHandler<CreateTariffGridCommand, ResponseHttp>
    {
        private readonly ITariffGridRepository _tariffGridRepository;
        private readonly IMapper _mapper;

        public CreateTariffGridCommandHandler(ITariffGridRepository tariffGridRepository, IMapper mapper)
        {
            _tariffGridRepository = tariffGridRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(CreateTariffGridCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if grid with same code already exists
                var existingGrid = await _tariffGridRepository.GetByCodeAsync(request.TariffGridDto.Code);
                if (existingGrid != null)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = $"Tariff grid with code '{request.TariffGridDto.Code}' already exists",
                        Status = StatusCodes.Status400BadRequest
                    };
                }

                // Parse transport mode
                if (!Enum.TryParse<TransportMode>(request.TariffGridDto.TransportMode, true, out var transportMode))
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = $"Invalid transport mode. Must be: Maritime, Aerien, Routier, Ferroviaire, Fluvial",
                        Status = StatusCodes.Status400BadRequest
                    };
                }

                var tariffGrid = new TariffGrid
                {
                    Code = request.TariffGridDto.Code,
                    Name = request.TariffGridDto.Name,
                    Description = request.TariffGridDto.Description,
                    TransportMode = transportMode,
                    Version = 1,
                    ValidFrom = request.TariffGridDto.ValidFrom,
                    ValidTo = request.TariffGridDto.ValidTo,
                    IsActive = true,
                    CurrencyCode = request.TariffGridDto.CurrencyCode,
                    CreatedDate = DateTime.UtcNow
                };

                await _tariffGridRepository.Post(tariffGrid);
                await _tariffGridRepository.SaveChange(cancellationToken);

                var result = _mapper.Map<TariffGridDTO>(tariffGrid);
                result.TariffLinesCount = 0;

                return new ResponseHttp
                {
                    Resultat = result,
                    Status = StatusCodes.Status201Created
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