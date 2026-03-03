// Application/Features/TariffGridFeature/Queries/GetAllTariffGridsQuery.cs
using Application.Features.TariffGridFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.TariffGridFeature.Queries
{
    public record GetAllTariffGridsQuery(
        int? PageNumber,
        int? PageSize,
        string? SortBy,
        bool? SortDescending,
        string? SearchTerm) : IRequest<ResponseHttp>;

    public class GetAllTariffGridsQueryHandler : IRequestHandler<GetAllTariffGridsQuery, ResponseHttp>
    {
        private readonly ITariffGridRepository _tariffGridRepository;
        private readonly IMapper _mapper;

        public GetAllTariffGridsQueryHandler(ITariffGridRepository tariffGridRepository, IMapper mapper)
        {
            _tariffGridRepository = tariffGridRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(GetAllTariffGridsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var tariffGrids = await _tariffGridRepository.GetAllWithFiltersAsync(
                    request.PageNumber,
                    request.PageSize,
                    request.SortBy,
                    request.SortDescending ?? false,
                    request.SearchTerm,
                    cancellationToken);

                if (tariffGrids == null || !tariffGrids.Items.Any())
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "No tariff grids found",
                        Status = StatusCodes.Status404NotFound
                    };
                }

                var gridDtos = _mapper.Map<IEnumerable<TariffGridDTO>>(tariffGrids.Items);

                var gridsToReturn = new PagedList<TariffGridDTO>(
                    gridDtos,
                    tariffGrids.TotalCount,
                    tariffGrids.CurrentPage,
                    tariffGrids.PageSize
                );

                // Calculate these properties manually instead of using PagedList methods
                var totalPages = (int)Math.Ceiling(tariffGrids.TotalCount / (double)tariffGrids.PageSize);
                var hasPreviousPage = tariffGrids.CurrentPage > 1;
                var hasNextPage = tariffGrids.CurrentPage < totalPages;

                var response = new
                {
                    items = gridsToReturn.Items,
                    totalCount = gridsToReturn.TotalCount,
                    pageNumber = gridsToReturn.CurrentPage,
                    pageSize = gridsToReturn.PageSize,
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