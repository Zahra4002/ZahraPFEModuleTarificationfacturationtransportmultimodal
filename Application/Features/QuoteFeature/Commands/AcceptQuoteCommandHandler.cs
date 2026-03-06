using Application.Features.QuoteFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.QuoteFeature.Commands
{
    public class AcceptQuoteCommandHandler : IRequestHandler<AcceptQuoteCommand, ResponseHttp>
    {
        private readonly IQuoteRepository _quoteRepository;
        private readonly IMapper _mapper;

        public AcceptQuoteCommandHandler(IQuoteRepository quoteRepository, IMapper mapper)
        {
            _quoteRepository = quoteRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(AcceptQuoteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Récupérer le devis
                var quote = await _quoteRepository.GetByIdAsync(request.Id, cancellationToken);

                if (quote == null)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status404NotFound,
                        Fail_Messages = $"Devis avec ID {request.Id} non trouvé"
                    };
                }

                // 2️⃣ Vérifier que le devis peut être accepté
                if (quote.IsAccepted)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Fail_Messages = "Ce devis a déjà été accepté"
                    };
                }

                if (quote.ValidUntil < DateTime.UtcNow)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Fail_Messages = "Ce devis a expiré et ne peut plus être accepté"
                    };
                }

                // 3️⃣ Marquer comme accepté
                quote.IsAccepted = true;
                quote.AcceptedAt = DateTime.UtcNow;
                quote.ModifiedDate = DateTime.UtcNow;
                quote.ModifiedBy = "System";

                // 4️⃣ Sauvegarder
                await _quoteRepository.UpdateAsync(quote, cancellationToken);
                await _quoteRepository.SaveChangesAsync(cancellationToken);

                // 5️⃣ Retourner le DTO
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