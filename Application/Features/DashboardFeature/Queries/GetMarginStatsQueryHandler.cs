using Application.Features.DashboardFeature.Dtos;
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

namespace Application.Features.DashboardFeature.Queries
{
    public class GetMarginStatsQueryHandler : IRequestHandler<GetMarginStatsQuery, ResponseHttp>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IMapper _mapper;

        public GetMarginStatsQueryHandler(
            IInvoiceRepository invoiceRepository,
            IShipmentRepository shipmentRepository,
            IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _shipmentRepository = shipmentRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(GetMarginStatsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Définir la période (par défaut : 30 derniers jours)
                var to = request.To?.Date.AddDays(1).AddSeconds(-1) ?? DateTime.UtcNow;
                var from = request.From?.Date ?? to.AddDays(-30);

                // 1️⃣ Récupérer les factures de la période
                var invoices = await _invoiceRepository.GetInvoicesByDateRangeAsync(from, to, cancellationToken);

                if (!invoices.Any())
                {
                    return new ResponseHttp
                    {
                        Resultat = new MarginStatsDto
                        {
                            From = from,
                            To = to,
                            TotalRevenue = 0,
                            TotalCost = 0,
                            TotalMargin = 0,
                            AverageMarginPercent = 0
                        },
                        Status = StatusCodes.Status200OK
                    };
                }

                // 2️⃣ Calculer les totaux
                var totalRevenue = invoices.Sum(i => i.TotalTTC);

                // Coût total (HT + surcharges)
                var totalCost = invoices.Sum(i => i.TotalHT + i.TotalVAT); // Approximation

                // Alternative plus précise si tu as les shipments :
                // var shipmentIds = invoices.Where(i => i.ShipmentId.HasValue).Select(i => i.ShipmentId.Value);
                // var shipments = await _shipmentRepository.GetShipmentsByIdsAsync(shipmentIds, cancellationToken);
                // var totalCost = shipments.Sum(s => s.TotalCostTTC);

                var totalMargin = totalRevenue - totalCost;
                var averageMarginPercent = totalRevenue > 0
                    ? Math.Round((totalMargin / totalRevenue) * 100, 2)
                    : 0;

                // 3️⃣ Créer le DTO de réponse
                var result = new MarginStatsDto
                {
                    From = from,
                    To = to,
                    TotalRevenue = Math.Round(totalRevenue, 2),
                    TotalCost = Math.Round(totalCost, 2),
                    TotalMargin = Math.Round(totalMargin, 2),
                    AverageMarginPercent = averageMarginPercent
                };

                return new ResponseHttp
                {
                    Resultat = result,
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