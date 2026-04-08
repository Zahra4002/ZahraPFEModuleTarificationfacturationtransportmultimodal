using Application.Features.MerchandiseTypeFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.MerchandiseTypeFeature.Queries
{
    public record GetMerchandiseTypeByIdQuery(Guid Id) : IRequest<ResponseHttp>;

    public class GetMerchandiseTypeByIdQueryHandler : IRequestHandler<GetMerchandiseTypeByIdQuery, ResponseHttp>
    {
        private readonly IMerchandiseTypeRepository _repository;
        private readonly IMapper _mapper;

        public GetMerchandiseTypeByIdQueryHandler(IMerchandiseTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(GetMerchandiseTypeByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
                if (entity == null)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status404NotFound,
                        Fail_Messages = "Type de marchandise non trouvé."
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