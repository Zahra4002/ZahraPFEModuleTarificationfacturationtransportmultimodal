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
        // private readonly IClientRepository _clientRepository; // ❌ Commenté temporairement
        private readonly IMapper _mapper;

        public CreateQuoteCommandHandler(
            IQuoteRepository quoteRepository,
            // IClientRepository clientRepository, // ❌ Commenté temporairement
            IMapper mapper)
        {
            _quoteRepository = quoteRepository;
            // _clientRepository = clientRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(CreateQuoteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // ✅ Temporairement, on ne vérifie pas l'existence du client
                // if (client == null)
                // {
                //     return new ResponseHttp
                //     {
                //         Status = StatusCodes.Status400BadRequest,
                //         Fail_Messages = $"Client avec ID {request.ClientId} non trouvé"
                //     };
                // }

                // Créer l'entité Quote
                var quote = new Quote
                {
                    Id = Guid.NewGuid(),
                    QuoteNumber = request.QuoteNumber,
                    ClientId = request.ClientId, // On suppose que le client existe
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

                // Sauvegarder
                await _quoteRepository.AddAsync(quote, cancellationToken); // ✅ Ajout cancellationToken
                await _quoteRepository.SaveChangesAsync(cancellationToken);

                // Retourner le DTO
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