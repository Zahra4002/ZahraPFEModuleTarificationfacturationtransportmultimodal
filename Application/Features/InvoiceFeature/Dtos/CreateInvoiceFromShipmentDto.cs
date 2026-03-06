namespace Application.Features.InvoiceFeature.Dtos
{
    public class CreateInvoiceFromShipmentDto
    {
        public Guid ShipmentId { get; set; }
        public Guid? CurrencyId { get; set; }
    }
}