using Application.Interfaces;
using Application.Setting;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.QuoteFeature.Commands
{
    public class DeleteQuoteCommandHandler : IRequestHandler<DeleteQuoteCommand, ResponseHttp>
    {
        private readonly IQuoteRepository _quoteRepository;

        public DeleteQuoteCommandHandler(IQuoteRepository quoteRepository)
        {
            _quoteRepository = quoteRepository;
        }

        public async Task<ResponseHttp> Handle(DeleteQuoteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // ✅ Ajout du cancellationToken
                var quote = await _quoteRepository.GetByIdAsync(request.Id, cancellationToken);

                if (quote == null)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status404NotFound,
                        Fail_Messages = $"Devis avec ID {request.Id} non trouvé"
                    };
                }

                // ✅ Utilisation de DeleteAsync
                await _quoteRepository.Delete(request.Id);
                await _quoteRepository.SaveChangesAsync(cancellationToken);

                return new ResponseHttp
                {
                    Status = StatusCodes.Status204NoContent
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