// Application/Features/TariffGridFeature/Commands/UpdateTariffLineCommand.cs
using Application.Features.TariffGridFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.TariffGridFeature.Commands
{
    public record UpdateTariffLineCommand(Guid GridId, Guid LineId, UpdateTariffLineDTO LineDto) : IRequest<ResponseHttp>;

    public class UpdateTariffLineCommandHandler : IRequestHandler<UpdateTariffLineCommand, ResponseHttp>
    {
        private readonly ITariffGridRepository _tariffGridRepository;
        private readonly IMapper _mapper;

        public UpdateTariffLineCommandHandler(ITariffGridRepository tariffGridRepository, IMapper mapper)
        {
            _tariffGridRepository = tariffGridRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(UpdateTariffLineCommand request, CancellationToken cancellationToken)
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

                // Get the line
                var line = await _tariffGridRepository.GetLineByIdAsync(request.LineId, cancellationToken);
                if (line == null || line.TariffGridId != request.GridId)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "Tariff line not found in this grid",
                        Status = StatusCodes.Status404NotFound
                    };
                }

                // Check for duplicate zone combination if zones are being changed
                if ((request.LineDto.ZoneFromId != line.ZoneFromId || request.LineDto.ZoneToId != line.ZoneToId) &&
                    request.LineDto.ZoneFromId.HasValue && request.LineDto.ZoneToId.HasValue)
                {
                    var exists = await _tariffGridRepository.LineExistsInGridAsync(
                        request.GridId,
                        request.LineDto.ZoneFromId,
                        request.LineDto.ZoneToId,
                        request.LineId);

                    if (exists)
                    {
                        return new ResponseHttp
                        {
                            Fail_Messages = "A line with this zone combination already exists in this grid",
                            Status = StatusCodes.Status400BadRequest
                        };
                    }
                }

                // Update fields - only those that exist in your entity
                if (request.LineDto.ZoneFromId.HasValue)
                    line.ZoneFromId = request.LineDto.ZoneFromId;

                if (request.LineDto.ZoneToId.HasValue)
                    line.ZoneToId = request.LineDto.ZoneToId;

                // Weight based
                if (request.LineDto.PricePerKg.HasValue)
                    line.PricePerKg = request.LineDto.PricePerKg;

                if (request.LineDto.MinWeight.HasValue)
                    line.MinWeight = request.LineDto.MinWeight;

                if (request.LineDto.MaxWeight.HasValue)
                    line.MaxWeight = request.LineDto.MaxWeight;

                // Volume based
                if (request.LineDto.PricePerM3.HasValue)
                    line.PricePerM3 = request.LineDto.PricePerM3;

                if (request.LineDto.MinVolume.HasValue)
                    line.MinVolume = request.LineDto.MinVolume;

                if (request.LineDto.MaxVolume.HasValue)
                    line.MaxVolume = request.LineDto.MaxVolume;

                // Container based - only those that exist in your entity
                if (request.LineDto.PricePerContainer20ft.HasValue)
                    line.PricePerContainer20ft = request.LineDto.PricePerContainer20ft;

                if (request.LineDto.PricePerContainer40ft.HasValue)
                    line.PricePerContainer40ft = request.LineDto.PricePerContainer40ft;

                // Note: PricePerContainer40ftHC is not in your entity - skip it

                // Linear meter - not in your entity - skip it

                // General
                if (request.LineDto.BasePrice.HasValue)
                    line.BasePrice = request.LineDto.BasePrice;

                if (request.LineDto.TransitDays.HasValue)
                    line.TransitDays = request.LineDto.TransitDays;

                line.ModifiedDate = DateTime.UtcNow;

                await _tariffGridRepository.UpdateLineAsync(line);
                await _tariffGridRepository.SaveChange(cancellationToken);

                // Get updated line with zone names
                var updatedLine = await _tariffGridRepository.GetLineByIdAsync(line.Id, cancellationToken);
                var result = _mapper.Map<TariffLineDTO>(updatedLine);

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