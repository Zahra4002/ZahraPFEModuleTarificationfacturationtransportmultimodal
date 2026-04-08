using Application.Features.MerchandiseTypeFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.MerchandiseTypeFeature.Queries
{
    public record GetMerchandiseTypeByCodeQuery(string Code) : IRequest<ResponseHttp>;

    public class GetMerchandiseTypeByCodeQueryHandler : IRequestHandler<GetMerchandiseTypeByCodeQuery, ResponseHttp>
    {
        private readonly IMerchandiseTypeRepository _repository;
        private readonly IMapper _mapper;

        public GetMerchandiseTypeByCodeQueryHandler(IMerchandiseTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(GetMerchandiseTypeByCodeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _repository.GetByCodeAsync(request.Code, cancellationToken);
                if (entity == null)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status404NotFound,
                        Fail_Messages = $"Aucun type de marchandise trouvé avec le code '{request.Code}'."
                    };
                }

                var dto = _mapper.Map<MerchandiseTypeDto>(entity);

                return new ResponseHttp
                {
                    Status = StatusCodes.Status200OK,
                    Resultat = dto
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