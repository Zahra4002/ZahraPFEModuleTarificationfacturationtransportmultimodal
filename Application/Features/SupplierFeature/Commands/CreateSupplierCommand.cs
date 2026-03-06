using Application.Setting;
using Application.Features.SupplierFeature.Dtos;
using MediatR;
using System.Collections.Generic;

namespace Application.Features.SupplierFeature.Commands
{
    public record CreateSupplierCommand(
        string Code,
        string Name,
        string? TaxId,
        string? Email,
        string? Phone,
        string Address,
        string DefaultCurrencyCode,
        bool IsActive,
        List<CreateContractDto>? Contracts,
        List<CreateTransportSegmentDto>? TransportSegments
    ) : IRequest<ResponseHttp>;
}