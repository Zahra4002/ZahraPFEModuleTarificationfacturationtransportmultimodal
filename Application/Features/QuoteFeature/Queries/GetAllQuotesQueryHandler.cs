using Application.Features.QuoteFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.QuoteFeature.Queries
{
    public class GetAllQuotesQueryHandler : IRequestHandler<GetAllQuotesQuery, ResponseHttp>
    {
        private readonly IQuoteRepository _quoteRepository;
        private readonly IMapper _mapper;

        public GetAllQuotesQueryHandler(IQuoteRepository quoteRepository, IMapper mapper)
        {
            _quoteRepository = quoteRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(GetAllQuotesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var pagedQuotes = await _quoteRepository.GetAllWithDetailsAsync(
                    request.PageNumber,
                    request.PageSize,
                    cancellationToken);

                // ✅ Correction ici : pagedQuotes.Items.Count au lieu de comparer avec int
                if (pagedQuotes == null || !pagedQuotes.Items.Any())
                {
                    return new ResponseHttp
                    {
                        Fail_Messages = "Aucun devis trouvé",
                        Status = StatusCodes.Status404NotFound
                    };
                }

                var quotesToReturn = _mapper.Map<PagedList<QuoteDto>>(pagedQuotes);

                return new ResponseHttp
                {
                    Resultat = quotesToReturn,
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
