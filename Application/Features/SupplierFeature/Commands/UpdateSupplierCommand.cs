using Application.Setting;
using Application.Features.SupplierFeature.Dtos;
using MediatR;
using System;
using System.Collections.Generic;

namespace Application.Features.SupplierFeature.Commands
{
    public record UpdateSupplierCommand(
        Guid Id,
        string? Code,
        string? Name,
        string? TaxId,
        string? Email,
        string? Phone,
        string? Address,
        string? DefaultCurrencyCode,
        bool? IsActive,
        List<UpdateContractDto>? Contracts,
        List<UpdateTransportSegmentDto>? TransportSegments
    ) : IRequest<ResponseHttp>;
}