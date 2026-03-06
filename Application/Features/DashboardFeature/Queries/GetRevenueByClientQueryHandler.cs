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
    public class GetRevenueByClientQueryHandler : IRequestHandler<GetRevenueByClientQuery, ResponseHttp>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        public GetRevenueByClientQueryHandler(IInvoiceRepository invoiceRepository, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(GetRevenueByClientQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Définir la période (par défaut : 30 derniers jours)
                var to = request.To?.Date.AddDays(1).AddSeconds(-1) ?? DateTime.UtcNow;
                var from = request.From?.Date ?? to.AddDays(-30);

                // 1️⃣ Récupérer les factures de la période avec les clients
                var invoices = await _invoiceRepository.GetInvoicesWithClientsByDateRangeAsync(from, to, cancellationToken);

                if (!invoices.Any())
                {
                    return new ResponseHttp
                    {
                        Resultat = new List<ClientRevenueDto>(),
                        Status = StatusCodes.Status200OK
                    };
                }

                // 2️⃣ Calculer le revenu total
                var totalRevenue = invoices.Sum(i => i.TotalTTC);

                // 3️⃣ Grouper par client
                var revenueByClient = invoices
                    .Where(i => i.ClientId.HasValue)
                    .GroupBy(i => new { i.ClientId, ClientName = i.Client != null ? i.Client.Name : "Client inconnu" })
                    .Select(g => new ClientRevenueDto
                    {
                        ClientId = g.Key.ClientId ?? Guid.Empty,
                        ClientName = g.Key.ClientName,
                        Revenue = g.Sum(i => i.TotalTTC),
                        Percentage = totalRevenue > 0 ? Math.Round((g.Sum(i => i.TotalTTC) / totalRevenue) * 100, 2) : 0
                    })
                    .OrderByDescending(c => c.Revenue)
                    .ToList();

                // 4️⃣ Ajouter les factures sans client
                var invoicesWithoutClient = invoices.Where(i => !i.ClientId.HasValue);
                if (invoicesWithoutClient.Any())
                {
                    revenueByClient.Add(new ClientRevenueDto
                    {
                        ClientId = Guid.Empty,
                        ClientName = "Sans client",
                        Revenue = invoicesWithoutClient.Sum(i => i.TotalTTC),
                        Percentage = totalRevenue > 0 ? Math.Round((invoicesWithoutClient.Sum(i => i.TotalTTC) / totalRevenue) * 100, 2) : 0
                    });
                }

                return new ResponseHttp
                {
                    Resultat = revenueByClient,
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