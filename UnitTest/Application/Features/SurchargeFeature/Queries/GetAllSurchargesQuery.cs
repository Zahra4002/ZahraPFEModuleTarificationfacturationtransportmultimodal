// Application/Features/SurchargeFeature/Queries/GetAllSurchargesQuery.cs
using Application.Features.SurchargeFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.Common;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.SurchargeFeature.Queries
{
    public record GetAllSurchargesQuery(
        int? PageNumber,
        int? PageSize,
        string? SortBy,
        bool? SortDescending,
        string? SearchTerm,
        string? Type,
        bool? IsActive) : IRequest<ResponseHttp>;

    public class GetAllSurchargesQueryHandler : IRequestHandler<GetAllSurchargesQuery, ResponseHttp>
    {
        private readonly ISurchargeRepository _surchargeRepository;
        private readonly IMapper _mapper;

        public GetAllSurchargesQueryHandler(ISurchargeRepository surchargeRepository, IMapper mapper)
        {
            _surchargeRepository = surchargeRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(GetAllSurchargesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                SurchargeType? type = null;
                if (!string.IsNullOrWhiteSpace(request.Type))
                {
                    if (!Enum.TryParse<SurchargeType>(request.Type, true, out var parsedType))
                    {
                        return new ResponseHttp
                        {
                            Fail_Messages = "Invalid surcharge type",
                            Status = StatusCodes.Status400BadRequest
                        };
                    }
                    type = parsedType;
                }

                var surcharges = await _surchargeRepository.GetAllWithFiltersAsync(
                    request.PageNumber,
                    request.PageSize,
                    request.SortBy,
                    request.SortDescending ?? false,
                    request.SearchTerm,
                    type,
                    request.IsActive,
                    cancellationToken);

                if (surcharges == null || !surcharges.Items.Any())
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "No surcharges found",
                        Status = StatusCodes.Status404NotFound
                    };
                }

                var surchargeDtos = _mapper.Map<IEnumerable<SurchargeDTO>>(surcharges.Items);

                // Load rule counts
                foreach (var dto in surchargeDtos)
                {
                    var rules = await _surchargeRepository.GetRulesBySurchargeIdAsync(dto.Id, cancellationToken);
                    dto.RulesCount = rules.Count();
                }

                var surchargesToReturn = new PagedList<SurchargeDTO>(
                    surchargeDtos,
                    surcharges.TotalCount,
                    surcharges.CurrentPage,
                    surcharges.PageSize
                );

                var totalPages = (int)Math.Ceiling(surcharges.TotalCount / (double)surcharges.PageSize);
                var hasPreviousPage = surcharges.CurrentPage > 1;
                var hasNextPage = surcharges.CurrentPage < totalPages;

                var response = new
                {
                    items = surchargesToReturn.Items,
                    totalCount = surchargesToReturn.TotalCount,
                    pageNumber = surchargesToReturn.CurrentPage,
                    pageSize = surchargesToReturn.PageSize,
                    totalPages = totalPages,
                    hasPreviousPage = hasPreviousPage,
                    hasNextPage = hasNextPage
                };

                return new ResponseHttp
                {
                    Status = StatusCodes.Status200OK,
                    Resultat = response
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