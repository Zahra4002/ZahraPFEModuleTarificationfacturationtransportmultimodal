// Application/Features/TariffGridFeature/Queries/GetTariffGridDetailsQuery.cs
using Application.Features.TariffGridFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.TariffGridFeature.Queries
{
    public record GetTariffGridDetailsQuery(Guid Id) : IRequest<ResponseHttp>;

    public class GetTariffGridDetailsQueryHandler : IRequestHandler<GetTariffGridDetailsQuery, ResponseHttp>
    {
        private readonly ITariffGridRepository _tariffGridRepository;
        private readonly IMapper _mapper;

        public GetTariffGridDetailsQueryHandler(ITariffGridRepository tariffGridRepository, IMapper mapper)
        {
            _tariffGridRepository = tariffGridRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(GetTariffGridDetailsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var tariffGrid = await _tariffGridRepository.GetByIdWithLinesAsync(request.Id, cancellationToken);
                if (tariffGrid == null)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "Tariff grid not found",
                        Status = StatusCodes.Status404NotFound
                    };
                }

                var result = _mapper.Map<TariffGridDetailsDTO>(tariffGrid);

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