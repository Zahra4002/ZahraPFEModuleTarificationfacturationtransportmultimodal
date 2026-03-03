using Application.Features.TariffGridFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.TariffGridFeature.Commands
{
    public record CreateTariffLineCommand(Guid GridId, CreateTariffLineDTO LineDto) : IRequest<ResponseHttp>;

    public class CreateTariffLineCommandHandler : IRequestHandler<CreateTariffLineCommand, ResponseHttp>
    {
        private readonly ITariffGridRepository _tariffGridRepository;
        private readonly IMapper _mapper;

        public CreateTariffLineCommandHandler(ITariffGridRepository tariffGridRepository, IMapper mapper)
        {
            _tariffGridRepository = tariffGridRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(CreateTariffLineCommand request, CancellationToken cancellationToken)
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

                // Check for duplicate zone combination
                var exists = await _tariffGridRepository.LineExistsInGridAsync(
                    request.GridId,
                    request.LineDto.ZoneFromId,
                    request.LineDto.ZoneToId);

                if (exists)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "A line with this zone combination already exists in this grid",
                        Status = StatusCodes.Status400BadRequest
                    };
                }

                var line = new TariffLine
                {
                    Id = Guid.NewGuid(),
                    TariffGridId = request.GridId,
                    ZoneFromId = request.LineDto.ZoneFromId,
                    ZoneToId = request.LineDto.ZoneToId,

                    // Weight based
                    PricePerKg = request.LineDto.PricePerKg,
                    MinWeight = request.LineDto.MinWeight,
                    MaxWeight = request.LineDto.MaxWeight,

                    // Volume based
                    PricePerM3 = request.LineDto.PricePerM3,
                    MinVolume = request.LineDto.MinVolume,
                    MaxVolume = request.LineDto.MaxVolume,

                    // Container based - only those that exist in your entity
                    PricePerContainer20ft = request.LineDto.PricePerContainer20ft,
                    PricePerContainer40ft = request.LineDto.PricePerContainer40ft,

                    // Note: PricePerContainer40ftHC is not in your entity - we'll skip it
                    // PricePerContainer40ftHC = request.LineDto.PricePerContainer40ftHC,

                    // Linear meter - not in your entity
                    // PricePerLinearMeter = request.LineDto.PricePerLinearMeter,

                    // General
                    BasePrice = request.LineDto.BasePrice,
                    TransitDays = request.LineDto.TransitDays,

                    // Height - not in your entity
                    // MinHeight = request.LineDto.MinHeight,
                    // MaxHeight = request.LineDto.MaxHeight,

                    CreatedDate = DateTime.UtcNow,
                    IsDeleted = false
                };

                await _tariffGridRepository.AddLineAsync(line);
                await _tariffGridRepository.SaveChange(cancellationToken);

                // Load zone names for response
                var createdLine = await _tariffGridRepository.GetLineByIdAsync(line.Id, cancellationToken);
                var result = _mapper.Map<TariffLineDTO>(createdLine);

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