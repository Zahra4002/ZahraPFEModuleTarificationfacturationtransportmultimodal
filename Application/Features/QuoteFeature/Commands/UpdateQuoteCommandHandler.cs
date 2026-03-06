using Application.Features.QuoteFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.QuoteFeature.Commands
{
    public class UpdateQuoteCommandHandler : IRequestHandler<UpdateQuoteCommand, ResponseHttp>  // ← public !
    {
        private readonly IQuoteRepository _quoteRepository;
        private readonly IMapper _mapper;

        public UpdateQuoteCommandHandler(IQuoteRepository quoteRepository, IMapper mapper)
        {
            _quoteRepository = quoteRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(UpdateQuoteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Récupérer le devis existant
                var quote = await _quoteRepository.GetByIdAsync(request.Id, cancellationToken);
                if (quote == null)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status404NotFound,
                        Fail_Messages = $"Devis avec ID {request.Id} non trouvé"
                    };
                }

                // 2️⃣ Mettre à jour uniquement les champs fournis
                if (!string.IsNullOrEmpty(request.QuoteNumber))
                    quote.QuoteNumber = request.QuoteNumber;

                if (request.ClientId.HasValue)
                    quote.ClientId = request.ClientId;

                if (request.OriginAddress != null)
                    quote.OriginAddress = request.OriginAddress;

                if (request.DestinationAddress != null)
                    quote.DestinationAddress = request.DestinationAddress;

                if (request.WeightKg.HasValue)
                    quote.WeightKg = request.WeightKg;

                if (request.VolumeM3.HasValue)
                    quote.VolumeM3 = request.VolumeM3;

                if (request.MerchandiseTypeId.HasValue)
                    quote.MerchandiseTypeId = request.MerchandiseTypeId;

                if (request.TotalHT.HasValue)
                    quote.TotalHT = request.TotalHT.Value;

                if (request.TotalTTC.HasValue)
                    quote.TotalTTC = request.TotalTTC.Value;

                if (!string.IsNullOrEmpty(request.CurrencyCode))
                    quote.CurrencyCode = request.CurrencyCode;

                if (request.ValidUntil.HasValue)
                    quote.ValidUntil = request.ValidUntil.Value;

                if (request.IsAccepted.HasValue)
                    quote.IsAccepted = request.IsAccepted.Value;

                if (request.AcceptedAt.HasValue)
                    quote.AcceptedAt = request.AcceptedAt;

                if (request.Notes != null)
                    quote.Notes = request.Notes;

                quote.ModifiedDate = DateTime.UtcNow;
                quote.ModifiedBy = "System";

                // 3️⃣ Sauvegarder
                await _quoteRepository.UpdateAsync(quote, cancellationToken);
                await _quoteRepository.SaveChangesAsync(cancellationToken);

                // 4️⃣ Retourner le DTO
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