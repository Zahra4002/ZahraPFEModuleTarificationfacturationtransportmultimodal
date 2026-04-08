using Application.Features.MerchandiseTypeFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.MerchandiseTypeFeature.Queries
{
    public record GetAllMerchandiseTypesQuery(
        int? PageNumber = 1,
        int? PageSize = 10,
        string? SortBy = null,
        bool SortDescending = false,
        string? SearchTerm = null,
        bool? IsActive = null
    ) : IRequest<ResponseHttp>;

    public class GetAllMerchandiseTypesQueryHandler : IRequestHandler<GetAllMerchandiseTypesQuery, ResponseHttp>
    {
        private readonly IMerchandiseTypeRepository _repository;
        private readonly IMapper _mapper;

        public GetAllMerchandiseTypesQueryHandler(IMerchandiseTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(GetAllMerchandiseTypesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _repository.GetPagedAsync(
                    request.PageNumber ?? 1,
                    request.PageSize ?? 10,
                    request.SortBy,
                    request.SortDescending,
                    request.SearchTerm,
                    request.IsActive,
                    cancellationToken
                );

                var dtos = _mapper.Map<List<MerchandiseTypeDto>>(result.Items);

                return new ResponseHttp
                {
                    Status = StatusCodes.Status200OK,
                    Resultat = new
                    {
                        Items = dtos,
                        TotalCount = result.TotalCount,
                        PageNumber = result.CurrentPage,
                        PageSize = result.PageSize,
                        TotalPages = result.TotalPages
                    }
                };
            }
            catch (Exception ex)
            {
                return new ResponseHttp
                {
                    Status = StatusCodes.Status400BadRequest,
                    Fail_Messages = ex.Message
                };
            }
        }
    }
}