using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ShipmentFeature.Dtos
{
    public class InvoiceDto
    {
        public Guid Id { get; set; }


        public string InvoiceNumber { get; set; }


        public DateTime InvoiceDate { get; set; }


        public DateTime DueDate { get; set; }


        public int Status { get; set; }


        public Guid? ClientId { get; set; }


        public string ClientName { get; set; }


        public Guid? SupplierId { get; set; }


        public string SupplierName { get; set; }


        public Guid? ShipmentId { get; set; }


        public string ShipmentNumber { get; set; }


        public Guid? CurrencyId { get; set; }


        public string CurrencyCode { get; set; }


        public decimal TotalHT { get; set; }


        public decimal TotalVAT { get; set; }


        public decimal TotalTTC { get; set; }



        public InvoiceDto(Invoice invoice)
        {
            Id = invoice.Id;
            InvoiceNumber = invoice.InvoiceNumber;
            InvoiceDate = invoice.InvoiceDate;
            DueDate = invoice.DueDate;
            Status = (int)invoice.Status;
            ClientId = invoice.ClientId;
            ClientName = invoice.Client?.Name ?? string.Empty;
            SupplierId = invoice.SupplierId;
            SupplierName = invoice.SupplierName ?? invoice.Supplier?.Name ?? string.Empty;
            ShipmentId = invoice.ShipmentId;
            ShipmentNumber = invoice.ShipmentNumber ?? invoice.Shipment?.ShipmentNumber ?? string.Empty;
            CurrencyId = invoice.CurrencyId;
            CurrencyCode = invoice.CurrencyCode ?? invoice.Currency?.Code ?? string.Empty;
            TotalHT = invoice.TotalHT;
            TotalVAT = invoice.TotalVAT;
            TotalTTC = invoice.TotalTTC;
            
        }
    }
}
