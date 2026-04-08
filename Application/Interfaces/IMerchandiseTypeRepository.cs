using Domain.Entities;
using Domain.Common;

namespace Application.Interfaces
{
    public interface IMerchandiseTypeRepository : IGenericRepository<MerchandiseType>
    {
        Task<MerchandiseType?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
        Task<List<MerchandiseType>> GetActiveAsync(CancellationToken cancellationToken = default);
        Task<PagedList<MerchandiseType>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? sortBy,
            bool sortDescending,
            string? searchTerm,
            bool? isActive,
            CancellationToken cancellationToken = default
        );
    }
}