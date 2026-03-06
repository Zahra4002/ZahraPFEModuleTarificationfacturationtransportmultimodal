using Application.Setting;
using MediatR;
using System;

namespace Application.Features.InvoiceFeature.Commands
{
    public record EmitInvoiceCommand : IRequest<ResponseHttp>
    {
        public EmitInvoiceCommand(Guid id)
        {
            Id = id;
            InvoiceDate = DateTime.UtcNow;
            DueDate = DateTime.UtcNow.AddDays(30);
        }

        public Guid Id { get; init; }
        public DateTime InvoiceDate { get; init; }
        public DateTime DueDate { get; init; }
    }
}