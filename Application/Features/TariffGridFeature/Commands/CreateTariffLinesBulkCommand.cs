// Application/Features/TariffGridFeature/Commands/CreateTariffLinesBulkCommand.cs
using Application.Features.TariffGridFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.TariffGridFeature.Commands
{
    public record CreateTariffLinesBulkCommand(Guid GridId, CreateTariffLineBulkDTO BulkDto) : IRequest<ResponseHttp>;

    public class CreateTariffLinesBulkCommandHandler : IRequestHandler<CreateTariffLinesBulkCommand, ResponseHttp>
    {
        private readonly ITariffGridRepository _tariffGridRepository;
        private readonly IMapper _mapper;

        public CreateTariffLinesBulkCommandHandler(ITariffGridRepository tariffGridRepository, IMapper mapper)
        {
            _tariffGridRepository = tariffGridRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(CreateTariffLinesBulkCommand request, CancellationToken cancellationToken)
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

                if (request.BulkDto.Lines == null || !request.BulkDto.Lines.Any())
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "No lines provided",
                        Status = StatusCodes.Status400BadRequest
                    };
                }

                var lines = new List<TariffLine>();
                var duplicateErrors = new List<string>();

                foreach (var lineDto in request.BulkDto.Lines)
                {
                    // Check for duplicate in existing lines
                    var exists = await _tariffGridRepository.LineExistsInGridAsync(
                        request.GridId,
                        lineDto.ZoneFromId,
                        lineDto.ZoneToId);

                    if (exists)
                    {
                        duplicateErrors.Add($"Line with ZoneFrom '{lineDto.ZoneFromId}' and ZoneTo '{lineDto.ZoneToId}' already exists");
                        continue;
                    }

                    // Check for duplicate within the batch
                    if (lines.Any(l => l.ZoneFromId == lineDto.ZoneFromId && l.ZoneToId == lineDto.ZoneToId))
                    {
                        duplicateErrors.Add($"Duplicate line with ZoneFrom '{lineDto.ZoneFromId}' and ZoneTo '{lineDto.ZoneToId}' in batch");
                        continue;
                    }

                    var line = new TariffLine
                    {
                        Id = Guid.NewGuid(),
                        TariffGridId = request.GridId,
                        ZoneFromId = lineDto.ZoneFromId,
                        ZoneToId = lineDto.ZoneToId,

                        // Weight based pricing
                        PricePerKg = lineDto.PricePerKg,
                        MinWeight = lineDto.MinWeight,
                        MaxWeight = lineDto.MaxWeight,

                        // Volume based pricing
                        PricePerM3 = lineDto.PricePerM3,
                        MinVolume = lineDto.MinVolume,
                        MaxVolume = lineDto.MaxVolume,

                        // Container based pricing (only those that exist in your entity)
                        PricePerContainer20ft = lineDto.PricePerContainer20ft,
                        PricePerContainer40ft = lineDto.PricePerContainer40ft,

                        // Note: PricePerContainer40ftHC is not in your entity - removed
                        // PricePerContainer40ftHC = lineDto.PricePerContainer40ftHC,

                        // Linear meter pricing - not in your entity - removed
                        // PricePerLinearMeter = lineDto.PricePerLinearMeter,

                        // General
                        BasePrice = lineDto.BasePrice,
                        TransitDays = lineDto.TransitDays,

                        // Height based pricing - not in your entity - removed
                        // MinHeight = lineDto.MinHeight,
                        // MaxHeight = lineDto.MaxHeight,

                        CreatedDate = DateTime.UtcNow,
                        IsDeleted = false
                    };

                    lines.Add(line);
                }

                if (lines.Any())
                {
                    await _tariffGridRepository.AddLinesBulkAsync(lines);
                    await _tariffGridRepository.SaveChange(cancellationToken);
                }

                var result = new
                {
                    Created = lines.Count,
                    Failed = duplicateErrors,
                    Lines = _mapper.Map<IEnumerable<TariffLineDTO>>(lines)
                };

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