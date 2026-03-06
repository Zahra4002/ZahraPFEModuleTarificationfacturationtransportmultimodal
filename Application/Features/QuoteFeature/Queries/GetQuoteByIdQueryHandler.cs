using Application.Features.QuoteFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.QuoteFeature.Queries
{
    public class GetQuoteByIdQueryHandler : IRequestHandler<GetQuoteByIdQuery, ResponseHttp>
    {
        private readonly IQuoteRepository _quoteRepository;
        private readonly IMapper _mapper;

        public GetQuoteByIdQueryHandler(IQuoteRepository quoteRepository, IMapper mapper)
        {
            _quoteRepository = quoteRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(GetQuoteByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var quote = await _quoteRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken);

                if (quote == null)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status404NotFound,
                        Fail_Messages = $"Devis avec ID {request.Id} non trouvé"
                    };
                }

                var quoteDto = _mapper.Map<QuoteDto>(quote);

                return new ResponseHttp
                {
                    Resultat = quoteDto,
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