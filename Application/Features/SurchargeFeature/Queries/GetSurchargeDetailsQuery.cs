using Application.Features.SurchargeFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.SurchargeFeature.Queries
{
    public record GetSurchargeDetailsQuery(Guid Id) : IRequest<ResponseHttp>;

    public class GetSurchargeDetailsQueryHandler : IRequestHandler<GetSurchargeDetailsQuery, ResponseHttp>
    {
        private readonly ISurchargeRepository _surchargeRepository;
        private readonly IMapper _mapper;

        public GetSurchargeDetailsQueryHandler(ISurchargeRepository surchargeRepository, IMapper mapper)
        {
            _surchargeRepository = surchargeRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(GetSurchargeDetailsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var surcharge = await _surchargeRepository.GetByIdWithRulesAsync(request.Id, cancellationToken);
                if (surcharge == null)
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "Surcharge not found",
                        Status = StatusCodes.Status404NotFound
                    };
                }

                var result = _mapper.Map<SurchargeDetailsDTO>(surcharge);

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