using Application.Features.DashboardFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.DashboardFeature.Queries
{
    public class GetRevenueByModeQueryHandler : IRequestHandler<GetRevenueByModeQuery, ResponseHttp>
    {
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        public GetRevenueByModeQueryHandler(
            IShipmentRepository shipmentRepository,
            IInvoiceRepository invoiceRepository,
            IMapper mapper)
        {
            _shipmentRepository = shipmentRepository;
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(GetRevenueByModeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Définir la période (par défaut : 30 derniers jours)
                var to = request.To?.Date.AddDays(1).AddSeconds(-1) ?? DateTime.UtcNow;
                var from = request.From?.Date ?? to.AddDays(-30);

                // 1️⃣ Récupérer les expéditions de la période
                var shipments = await _shipmentRepository.GetShipmentsByDateRangeWithSegmentsAsync(from, to, cancellationToken);

                if (!shipments.Any())
                {
                    return new ResponseHttp
                    {
                        Resultat = new List<RevenueByModeDto>(),
                        Status = StatusCodes.Status200OK
                    };
                }

                // 2️⃣ Calculer le revenu total
                var totalRevenue = shipments.Sum(s => s.TotalCostTTC);

                // 3️⃣ Initialiser le dictionnaire pour tous les modes
                var modeRevenue = new Dictionary<TransportMode, (decimal Revenue, HashSet<Guid> ShipmentIds)>();

                // Initialiser avec tous les modes de l'enum
                foreach (TransportMode mode in Enum.GetValues(typeof(TransportMode)))
                {
                    modeRevenue[mode] = (0, new HashSet<Guid>());
                }

                // 4️⃣ Parcourir les expéditions et leurs segments
                foreach (var shipment in shipments)
                {
                    if (shipment.Segments != null && shipment.Segments.Any())
                    {
                        foreach (var segment in shipment.Segments)
                        {
                            if (modeRevenue.ContainsKey(segment.TransportMode))
                            {
                                var (revenue, shipmentIds) = modeRevenue[segment.TransportMode];
                                revenue += segment.TotalCost;
                                shipmentIds.Add(shipment.Id);
                                modeRevenue[segment.TransportMode] = (revenue, shipmentIds);
                            }
                        }
                    }
                    else
                    {
                        // Si pas de segment, utiliser le mode par défaut (Routier)
                        var (revenue, shipmentIds) = modeRevenue[TransportMode.Routier];
                        revenue += shipment.TotalCostTTC;
                        shipmentIds.Add(shipment.Id);
                        modeRevenue[TransportMode.Routier] = (revenue, shipmentIds);
                    }
                }

                // 5️⃣ Convertir en liste de DTO
                var revenueByMode = modeRevenue
                    .Where(kv => kv.Value.Revenue > 0 || kv.Value.ShipmentIds.Count > 0)
                    .Select(kv => new RevenueByModeDto
                    {
                        TransportMode = kv.Key.ToString(),
                        Revenue = Math.Round(kv.Value.Revenue, 2),
                        ShipmentCount = kv.Value.ShipmentIds.Count,
                        Percentage = totalRevenue > 0
                            ? Math.Round((kv.Value.Revenue / totalRevenue) * 100, 2)
                            : 0
                    })
                    .OrderByDescending(m => m.Revenue)
                    .ToList();

                return new ResponseHttp
                {
                    Resultat = revenueByMode,
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