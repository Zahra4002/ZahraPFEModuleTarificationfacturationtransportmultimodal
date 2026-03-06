using Application.Setting;
using MediatR;

namespace Application.Features.SupplierFeature.Queries
{
    public record GetAllSuppliersQuery(int? PageNumber, int? PageSize) : IRequest<ResponseHttp>;
}