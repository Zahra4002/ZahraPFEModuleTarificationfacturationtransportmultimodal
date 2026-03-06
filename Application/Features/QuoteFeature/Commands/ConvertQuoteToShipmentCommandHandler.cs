using Application.Features.QuoteFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.QuoteFeature.Commands
{
    public class ConvertQuoteToShipmentCommandHandler : IRequestHandler<ConvertQuoteToShipmentCommand, ResponseHttp>
    {
        private readonly IQuoteRepository _quoteRepository;
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IMapper _mapper;

        public ConvertQuoteToShipmentCommandHandler(
            IQuoteRepository quoteRepository,
            IShipmentRepository shipmentRepository,
            IMapper mapper)
        {
            _quoteRepository = quoteRepository;
            _shipmentRepository = shipmentRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(ConvertQuoteToShipmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Récupérer le devis avec tous ses détails
                var quote = await _quoteRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken);

                if (quote == null)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status404NotFound,
                        Fail_Messages = $"Devis avec ID {request.Id} non trouvé"
                    };
                }

                // 2️⃣ Vérifier que le devis peut être converti
                if (!quote.IsAccepted)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Fail_Messages = "Seuls les devis acceptés peuvent être convertis en expédition"
                    };
                }

                // 3️⃣ Vérifier que le devis n'a pas déjà été converti
                var existingShipment = await _shipmentRepository.GetByQuoteIdAsync(request.Id);
                if (existingShipment != null)
                {
                    return new ResponseHttp
                    {
                        Status = StatusCodes.Status409Conflict,
                        Fail_Messages = "Ce devis a déjà été converti en expédition"
                    };
                }

                // 4️⃣ Créer l'expédition à partir du devis
                var shipment = new Shipment
                {
                    Id = Guid.NewGuid(),
                    ShipmentNumber = GenerateShipmentNumber(),
                    QuoteId = quote.Id,
                    ClientId = quote.ClientId,
                    OriginAddress = quote.OriginAddress,
                    DestinationAddress = quote.DestinationAddress,
                    WeightKg = quote.WeightKg,
                    VolumeM3 = quote.VolumeM3,
                    MerchandiseTypeId = quote.MerchandiseTypeId,
                    Status = ShipmentStatus.Draft,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "System"
                };

                // 5️⃣ Sauvegarder l'expédition
                await _shipmentRepository.AddAsync(shipment);
                await _shipmentRepository.SaveChangesAsync(cancellationToken);

                // 6️⃣ Retourner le DTO
                var resultDto = new ConvertedShipmentDto
                {
                    Id = shipment.Id,
                    ShipmentNumber = shipment.ShipmentNumber,
                    ClientId = shipment.ClientId,
                    ClientName = quote.Client?.Name,
                    OriginAddress = shipment.OriginAddress,
                    DestinationAddress = shipment.DestinationAddress,
                    GoodsDescription = "Marchandises diverses", // À personnaliser
                    WeightKg = shipment.WeightKg,
                    VolumeM3 = shipment.VolumeM3,
                    NumberOfPackages = 1, // Valeur par défaut
                    Status = shipment.Status.ToString(),
                    RequestedPickupDate = DateTime.UtcNow.AddDays(1) // Valeur par défaut
                };

                return new ResponseHttp
                {
                    Resultat = resultDto,
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

        private string GenerateShipmentNumber()
        {
            return $"SHP-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
        }
    }
}
