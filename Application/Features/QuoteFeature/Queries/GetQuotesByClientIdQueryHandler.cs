using Application.Features.QuoteFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.QuoteFeature.Queries
{
    public class GetQuotesByClientIdQueryHandler : IRequestHandler<GetQuotesByClientIdQuery, ResponseHttp>
    {
        private readonly IQuoteRepository _quoteRepository;
        private readonly IMapper _mapper;

        public GetQuotesByClientIdQueryHandler(IQuoteRepository quoteRepository, IMapper mapper)
        {
            _quoteRepository = quoteRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(GetQuotesByClientIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Récupérer tous les devis du client
                var quotes = await _quoteRepository.GetByClientIdAsync(request.ClientId, cancellationToken);

                if (quotes == null || !quotes.Any())
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = $"Aucun devis trouvé pour le client {request.ClientId}",
                        Status = StatusCodes.Status404NotFound
                    };
                }

                // Mapper vers DTO
                var quotesDto = _mapper.Map<List<QuoteDto>>(quotes);

                return new ResponseHttp
                {
                    Resultat = quotesDto,
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