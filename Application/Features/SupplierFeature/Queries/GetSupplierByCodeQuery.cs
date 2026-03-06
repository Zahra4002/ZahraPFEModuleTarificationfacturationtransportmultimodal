using Application.Setting;
using MediatR;

namespace Application.Features.SupplierFeature.Queries
{
    public record GetSupplierByCodeQuery(string Code) : IRequest<ResponseHttp>;
}