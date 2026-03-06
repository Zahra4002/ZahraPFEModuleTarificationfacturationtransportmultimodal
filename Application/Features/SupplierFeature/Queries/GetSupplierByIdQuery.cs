using Application.Setting;
using MediatR;
using System;

namespace Application.Features.SupplierFeature.Queries
{
    public record GetSupplierByIdQuery(Guid Id) : IRequest<ResponseHttp>;
}