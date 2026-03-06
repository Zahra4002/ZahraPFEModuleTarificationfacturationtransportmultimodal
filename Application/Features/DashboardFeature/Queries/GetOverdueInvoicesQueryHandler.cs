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
    public class GetOverdueInvoicesQueryHandler : IRequestHandler<GetOverdueInvoicesQuery, ResponseHttp>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        public GetOverdueInvoicesQueryHandler(IInvoiceRepository invoiceRepository, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        public async Task<ResponseHttp> Handle(GetOverdueInvoicesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var now = DateTime.UtcNow;

                // 1️⃣ Récupérer les factures en retard
                var overdueInvoices = await _invoiceRepository.GetOverdueInvoicesWithDetailsAsync(now, cancellationToken);

                if (overdueInvoices == null || !overdueInvoices.Any())
                {
                    return new ResponseHttp
                    {
                        Resultat = new List<OverdueInvoiceDto>(),
                        Status = StatusCodes.Status200OK
                    };
                }

                // 2️⃣ Mapper vers DTO
                var overdueDtos = overdueInvoices.Select(inv => new OverdueInvoiceDto
                {

                    Id = inv.Id,
                    InvoiceNumber = inv.InvoiceNumber,
                    InvoiceDate = inv.InvoiceDate,
                    DueDate = inv.DueDate,
                    IsSupplierInvoice = inv.IsSupplierInvoice,
                    ClientId = inv.ClientId,
                    ClientName = inv.Client?.Name,
                    SupplierId = inv.SupplierId,
                    SupplierName = inv.Supplier?.Name,
                    ShipmentId = inv.ShipmentId,
                    ShipmentNumber = inv.ShipmentNumber,
                    TotalHT = inv.TotalHT,
                    TotalVAT = inv.TotalVAT,
                    TotalTTC = inv.TotalTTC,
                    AmountPaid = inv.AmountPaid,
                    Balance = inv.TotalTTC - inv.AmountPaid,
                    CurrencyCode = inv.CurrencyCode ?? "EUR",
                    Status = inv.Status.ToString(),
                    IsOverdue = true,
                    DaysOverdue = (now - inv.DueDate).Days,
                    CreatedAt = inv.CreatedDate ?? inv.InvoiceDate
                }).ToList();

                return new ResponseHttp
                {
                    Resultat = overdueDtos,
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