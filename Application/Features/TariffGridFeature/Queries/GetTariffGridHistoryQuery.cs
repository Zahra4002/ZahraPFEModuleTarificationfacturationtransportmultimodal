using Application.Features.TariffGridFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.TariffGridFeature.Queries
{
    public record GetTariffGridHistoryQuery(string Code) : IRequest<ResponseHttp>;

    public class GetTariffGridHistoryQueryHandler : IRequestHandler<GetTariffGridHistoryQuery, ResponseHttp>
    {
        private readonly ITariffGridRepository _tariffGridRepository;
        private readonly IMapper _mapper;

        public GetTariffGridHistoryQueryHandler(ITariffGridRepository tariffGridRepository, IMapper mapper)
        {
            _tariffGridRepository = tariffGridRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(GetTariffGridHistoryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var history = await _tariffGridRepository.GetHistoryByCodeAsync(request.Code, cancellationToken);

                if (!history.Any())
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = $"No history found for tariff grid code: {request.Code}",
                        Status = StatusCodes.Status404NotFound
                    };
                }

                var result = _mapper.Map<IEnumerable<TariffGridHistoryDTO>>(history);

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