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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.DashboardFeature.Queries
{
    public class GetInvoiceStatusStatsQueryHandler : IRequestHandler<GetInvoiceStatusStatsQuery, ResponseHttp>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        public GetInvoiceStatusStatsQueryHandler(IInvoiceRepository invoiceRepository, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(GetInvoiceStatusStatsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Récupérer toutes les factures non supprimées
                var invoices = await _invoiceRepository.GetAllInvoicesAsync(cancellationToken);

                var now = DateTime.UtcNow;

                // 2️⃣ Compter par statut
                var draftCount = invoices.Count(i => i.Status == InvoiceStatus.Brouillon);
                var emittedCount = invoices.Count(i => i.Status == InvoiceStatus.Emise || i.Status == InvoiceStatus.Envoyee);
                var paidCount = invoices.Count(i => i.Status == InvoiceStatus.Payee);
                int disputedCount = 0;



                // 3️⃣ Calculer les factures en retard
                var overdueInvoices = invoices.Where(i =>
                    i.Status == InvoiceStatus.Envoyee &&
                    i.DueDate < now &&
                    i.TotalTTC > i.AmountPaid
                ).ToList();

                var overdueCount = overdueInvoices.Count;
                var totalOverdue = overdueInvoices.Sum(i => i.TotalTTC - i.AmountPaid);

                // 4️⃣ Calculer le total des impayés
                var outstandingInvoices = invoices.Where(i =>
                    (i.Status == InvoiceStatus.Envoyee || i.Status == InvoiceStatus.Emise) &&
                    i.TotalTTC > i.AmountPaid
                ).ToList();

                var totalOutstanding = outstandingInvoices.Sum(i => i.TotalTTC - i.AmountPaid);

                // 5️⃣ Créer le DTO de réponse
                var stats = new InvoiceStatusStatsDto
                {
                    TotalInvoices = invoices.Count,
                    DraftCount = draftCount,
                    EmittedCount = emittedCount,
                    PaidCount = paidCount,
                    OverdueCount = overdueCount,
                    DisputedCount = disputedCount,
                    TotalOutstanding = Math.Round(totalOutstanding, 2),
                    TotalOverdue = Math.Round(totalOverdue, 2)
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