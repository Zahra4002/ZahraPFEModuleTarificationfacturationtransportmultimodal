using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.SupplierFeature.Dtos

{

    public class InvoiceSummaryDTO
    {
        public Guid Id { get; set; }


        public string InvoiceNumber { get; set; }


        public DateTime InvoiceDate { get; set; }


        public DateTime DueDate { get; set; }


        public int Status { get; set; }

    }

}