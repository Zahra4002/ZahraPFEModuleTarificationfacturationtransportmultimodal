using System;

namespace Application.Features.SupplierFeature.Dtos
{
    public class TransportSegmentDto
    {
        public Guid Id { get; set; }
        public int Sequence { get; set; }
        public int TransportMode { get; set; }
        public string TransportModeName { get; set; } = string.Empty;

    }
}