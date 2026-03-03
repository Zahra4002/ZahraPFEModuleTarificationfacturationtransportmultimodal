// Application/Features/TariffGridFeature/Queries/GetTariffGridByIdQuery.cs
using Application.Features.TariffGridFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.TariffGridFeature.Queries
{
    public record GetTariffGridByIdQuery(Guid Id) : IRequest<ResponseHttp>;

    public class GetTariffGridByIdQueryHandler : IRequestHandler<GetTariffGridByIdQuery, ResponseHttp>
    {
        private readonly ITariffGridRepository _tariffGridRepository;
        private readonly IMapper _mapper;

        public GetTariffGridByIdQueryHandler(ITariffGridRepository tariffGridRepository, IMapper mapper)
        {
            _tariffGridRepository = tariffGridRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(GetTariffGridByIdQuery request, CancellationToken cancellationToken)
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

                var lineCount = await _tariffGridRepository.GetLinesByGridIdAsync(request.Id, cancellationToken);

                var result = _mapper.Map<TariffGridDTO>(tariffGrid);
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