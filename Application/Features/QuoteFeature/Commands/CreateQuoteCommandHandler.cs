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
    public class CreateQuoteCommandHandler : IRequestHandler<CreateQuoteCommand, ResponseHttp>
    {
        private readonly IQuoteRepository _quoteRepository;
        private readonly IMapper _mapper;

        public CreateQuoteCommandHandler(
            IQuoteRepository quoteRepository,
            IMapper mapper)
        {
            _quoteRepository = quoteRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(CreateQuoteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Générer le numéro de devis automatiquement si non fourni
                var quoteNumber = !string.IsNullOrWhiteSpace(request.QuoteNumber)
                    ? request.QuoteNumber
                    : $"DEV-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";

                var quote = new Quote
                {
                    Id = Guid.NewGuid(),
                    QuoteNumber = quoteNumber,
                    ClientId = request.ClientId,
                    OriginAddress = request.OriginAddress,
                    DestinationAddress = request.DestinationAddress,
                    WeightKg = request.WeightKg,
                    VolumeM3 = request.VolumeM3,
                    MerchandiseTypeId = request.MerchandiseTypeId,
                    TotalHT = request.TotalHT,
                    TotalTTC = request.TotalTTC,
                    CurrencyCode = request.CurrencyCode,
                    ValidUntil = request.ValidUntil,
                    IsAccepted = false,
                    Notes = request.Notes,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "System"
                };

                await _quoteRepository.AddAsync(quote, cancellationToken);
                await _quoteRepository.SaveChangesAsync(cancellationToken);

                var quoteDto = _mapper.Map<QuoteDto>(quote);

                return new ResponseHttp
                {
                    Resultat = quoteDto,
                    Status = StatusCodes.Status201Created
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