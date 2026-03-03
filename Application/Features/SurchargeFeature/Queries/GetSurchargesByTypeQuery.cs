// Application/Features/SurchargeFeature/Queries/GetSurchargesByTypeQuery.cs
using Application.Features.SurchargeFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.SurchargeFeature.Queries
{
    public record GetSurchargesByTypeQuery(string Type) : IRequest<ResponseHttp>;

    public class GetSurchargesByTypeQueryHandler : IRequestHandler<GetSurchargesByTypeQuery, ResponseHttp>
    {
        private readonly ISurchargeRepository _surchargeRepository;
        private readonly IMapper _mapper;

        public GetSurchargesByTypeQueryHandler(ISurchargeRepository surchargeRepository, IMapper mapper)
        {
            _surchargeRepository = surchargeRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(GetSurchargesByTypeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (!Enum.TryParse<SurchargeType>(request.Type, true, out var type))
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "Invalid surcharge type",
                        Status = StatusCodes.Status400BadRequest
                    };
                }

                var surcharges = await _surchargeRepository.GetByTypeAsync(type, cancellationToken);

                if (!surcharges.Any())
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = $"No surcharges found for type: {request.Type}",
                        Status = StatusCodes.Status404NotFound
                    };
                }

                var result = _mapper.Map<IEnumerable<SurchargeDTO>>(surcharges);

                // Load rule counts
                foreach (var dto in result)
                {
                    var rules = await _surchargeRepository.GetRulesBySurchargeIdAsync(dto.Id, cancellationToken);
                    dto.RulesCount = rules.Count();
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