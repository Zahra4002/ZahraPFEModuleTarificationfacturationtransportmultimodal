namespace Application.Features.InvoiceFeature.Dtos
{
    public class PdfResponseDto
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public string FileContent { get; set; }
    }
}