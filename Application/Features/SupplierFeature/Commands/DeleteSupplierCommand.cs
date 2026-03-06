using Application.Setting;
using MediatR;
using System;

namespace Application.Features.SupplierFeature.Commands
{
    public record DeleteSupplierCommand(Guid Id) : IRequest<ResponseHttp>;
}