using Domain.Entities;
using System.Linq.Expressions;

namespace Application.Interfaces
{
    public interface IZoneRepository : IGenericRepository<Zone>
    {
        Task<Zone?> GetByIdAsync(Guid id);
        Task<List<Zone>> GetAllWithTypesAsync(
           int? pageNumber = null,
           int? pageSize = null,
           string? sortedBy = null,
           bool sortDescending = false,
           string? searchTerm = null,
           CancellationToken cancellationToken = default
        );
        Task<Zone?> SelectOneAsync(Expression<Func<Zone, bool>> predicate);

        Task<List<Zone>> SelectManyAsync(Expression<Func<Zone, bool>> predicate);
    }
}