// Application/Features/TariffGridFeature/Queries/GetTariffGridsByModeQuery.cs
using Application.Features.TariffGridFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.TariffGridFeature.Queries
{
    public record GetTariffGridsByModeQuery(string Mode) : IRequest<ResponseHttp>;

    public class GetTariffGridsByModeQueryHandler : IRequestHandler<GetTariffGridsByModeQuery, ResponseHttp>
    {
        private readonly ITariffGridRepository _tariffGridRepository;
        private readonly IMapper _mapper;

        public GetTariffGridsByModeQueryHandler(ITariffGridRepository tariffGridRepository, IMapper mapper)
        {
            _tariffGridRepository = tariffGridRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(GetTariffGridsByModeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate mode
                var validModes = new[] { "Terrestre", "Maritime", "RoRo", "Conteneurise", "Routier", "Aerien", "Ferroviaire", "Fluvial" };
                if (!validModes.Contains(request.Mode, StringComparer.OrdinalIgnoreCase))
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = $"Invalid mode. Must be one of: {string.Join(", ", validModes)}",
                        Status = StatusCodes.Status400BadRequest
                    };
                }

                var tariffGrids = await _tariffGridRepository.GetByTransportModeAsync(request.Mode, cancellationToken);

                if (!tariffGrids.Any())
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = $"No tariff grids found for mode: {request.Mode}",
                        Status = StatusCodes.Status404NotFound
                    };
                }

                var result = _mapper.Map<IEnumerable<TariffGridDTO>>(tariffGrids);

                // Load line counts
                foreach (var grid in result)
                {
                    var lines = await _tariffGridRepository.GetLinesByGridIdAsync(grid.Id, cancellationToken);
                    grid.TariffLinesCount = lines.Count();
                }

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