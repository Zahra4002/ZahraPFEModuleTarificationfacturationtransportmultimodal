// Application/Interfaces/ITariffGridRepository.cs
using Domain.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ITariffGridRepository : IGenericRepository<TariffGrid>
    {
        Task<PagedList<TariffGrid>> GetAllWithFiltersAsync(
            int? pageNumber,
            int? pageSize,
            string? sortBy,
            bool sortDescending,
            string? searchTerm,
            CancellationToken cancellationToken);

        Task<TariffGrid?> GetByCodeAsync(string code);
        Task<TariffGrid?> GetByIdWithLinesAsync(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<TariffGrid>> GetByTransportModeAsync(string mode, CancellationToken cancellationToken);
        Task<IEnumerable<TariffGrid>> GetHistoryByCodeAsync(string code, CancellationToken cancellationToken);
        Task<TariffGrid?> GetActiveGridByCodeAsync(string code, DateTime date);
        Task<int> GetNextVersionAsync(string code);
        Task<TariffGrid> CloneGridAsync(Guid id, string newCode, string? newName);

        // Tariff Lines methods
        Task<TariffLine?> GetLineByIdAsync(Guid lineId, CancellationToken cancellationToken);
        Task<IEnumerable<TariffLine>> GetLinesByGridIdAsync(Guid gridId, CancellationToken cancellationToken);
        Task<TariffLine> AddLineAsync(TariffLine line);
        Task AddLinesBulkAsync(List<TariffLine> lines);
        Task UpdateLineAsync(TariffLine line);
        Task<bool> DeleteLineAsync(Guid lineId);
        Task<bool> LineExistsInGridAsync(Guid gridId, Guid? zoneFromId, Guid? zoneToId, Guid? excludeLineId = null);
    }
}