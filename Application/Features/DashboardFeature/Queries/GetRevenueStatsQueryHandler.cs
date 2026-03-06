using Application.Features.DashboardFeature.Dtos;
using Application.Interfaces;
using Application.Setting;
using AutoMapper;
using Domain.Entities;
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
    public class GetRevenueStatsQueryHandler : IRequestHandler<GetRevenueStatsQuery, ResponseHttp>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IMapper _mapper;

        public GetRevenueStatsQueryHandler(
            IInvoiceRepository invoiceRepository,
            IShipmentRepository shipmentRepository,
            IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _shipmentRepository = shipmentRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(GetRevenueStatsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Définir la période (par défaut : 30 derniers jours)
                var to = request.To?.Date.AddDays(1).AddSeconds(-1) ?? DateTime.UtcNow;
                var from = request.From?.Date ?? to.AddDays(-30);

                // 1️⃣ Récupérer les factures de la période
                var invoices = await _invoiceRepository.GetInvoicesByDateRangeAsync(from, to, cancellationToken);

                // 2️⃣ Calculer les totaux
                var totalRevenue = invoices.Sum(i => i.TotalTTC);
                var totalCost = invoices.Sum(i => i.TotalHT); // Coût approximatif
                var totalMargin = totalRevenue - totalCost;
                var marginPercent = totalRevenue > 0 ? (totalMargin / totalRevenue) * 100 : 0;

                // 3️⃣ Calculer le breakdown mensuel
                var monthlyBreakdown = invoices
                    .GroupBy(i => new { i.InvoiceDate.Year, i.InvoiceDate.Month })
                    .Select(g => new MonthlyBreakdownDto
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Revenue = g.Sum(i => i.TotalTTC),
                        Cost = g.Sum(i => i.TotalHT),
                        Margin = g.Sum(i => i.TotalTTC) - g.Sum(i => i.TotalHT)
                    })
                    .OrderBy(m => m.Year).ThenBy(m => m.Month)
                    .ToList();

                // 4️⃣ Retourner les statistiques
                var stats = new RevenueStatsDto
                {
                    From = from,
                    To = to,
                    TotalRevenue = totalRevenue,
                    TotalCost = totalCost,
                    TotalMargin = totalMargin,
                    MarginPercent = Math.Round(marginPercent, 2),
                    MonthlyBreakdown = monthlyBreakdown
                };

                return new ResponseHttp
                {
                    Resultat = stats,
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