// Persistance/Repositories/TariffGridRepository.cs
using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistance.Data;

namespace Persistance.Repositories
{
    public class TariffGridRepository : GenericRepository<TariffGrid>, ITariffGridRepository
    {
        private readonly CleanArchitecturContext _context;

        public TariffGridRepository(CleanArchitecturContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PagedList<TariffGrid>> GetAllWithFiltersAsync(
            int? pageNumber,
            int? pageSize,
            string? sortBy,
            bool sortDescending,
            string? searchTerm,
            CancellationToken cancellationToken)
        {
            IQueryable<TariffGrid> query = _context.Set<TariffGrid>()
                .Where(t => !t.IsDeleted);

            // Apply search
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(t =>
                    t.Code.Contains(searchTerm) ||
                    t.Name.Contains(searchTerm) ||
                    (t.Description != null && t.Description.Contains(searchTerm)));
            }

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "code":
                        query = sortDescending
                            ? query.OrderByDescending(t => t.Code)
                            : query.OrderBy(t => t.Code);
                        break;
                    case "name":
                        query = sortDescending
                            ? query.OrderByDescending(t => t.Name)
                            : query.OrderBy(t => t.Name);
                        break;
                    case "transportmode":
                        query = sortDescending
                            ? query.OrderByDescending(t => t.TransportMode)
                            : query.OrderBy(t => t.TransportMode);
                        break;
                    case "validfrom":
                        query = sortDescending
                            ? query.OrderByDescending(t => t.ValidFrom)
                            : query.OrderBy(t => t.ValidFrom);
                        break;
                    case "validto":
                        query = sortDescending
                            ? query.OrderByDescending(t => t.ValidTo)
                            : query.OrderBy(t => t.ValidTo);
                        break;
                    case "createdat":
                        query = sortDescending
                            ? query.OrderByDescending(t => t.CreatedDate)
                            : query.OrderBy(t => t.CreatedDate);
                        break;
                    case "version":
                        query = sortDescending
                            ? query.OrderByDescending(t => t.Version)
                            : query.OrderBy(t => t.Version);
                        break;
                    default:
                        query = query.OrderBy(t => t.CreatedDate);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(t => t.CreatedDate);
            }

            // Count total items
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply pagination
            var page = pageNumber ?? 1;
            var size = pageSize ?? 10;

            var items = await query
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync(cancellationToken);

            // Note: TariffLinesCount n'existe pas dans l'entité TariffGrid
            // Nous ne pouvons pas définir cette propriété ici
            // Elle sera gérée dans le DTO

            return new PagedList<TariffGrid>(items, totalCount, page, size);
        }

        public async Task<TariffGrid?> GetByCodeAsync(string code)
        {
            return await _context.Set<TariffGrid>()
                .FirstOrDefaultAsync(t => t.Code == code && !t.IsDeleted);
        }

        public async Task<TariffGrid?> GetByIdWithLinesAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Set<TariffGrid>()
                .Include(t => t.TariffLines.Where(l => !l.IsDeleted))
                    .ThenInclude(l => l.ZoneFrom)
                .Include(t => t.TariffLines.Where(l => !l.IsDeleted))
                    .ThenInclude(l => l.ZoneTo)
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, cancellationToken);
        }

        public async Task<IEnumerable<TariffGrid>> GetByTransportModeAsync(string mode, CancellationToken cancellationToken)
        {
            // Parse transport mode string to enum
            if (!Enum.TryParse<Domain.Enums.TransportMode>(mode, true, out var transportMode))
            {
                return new List<TariffGrid>();
            }

            return await _context.Set<TariffGrid>()
                .Where(t => t.TransportMode == transportMode && !t.IsDeleted && t.IsActive)
                .OrderBy(t => t.Code)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TariffGrid>> GetHistoryByCodeAsync(string code, CancellationToken cancellationToken)
        {
            return await _context.Set<TariffGrid>()
                .Where(t => t.Code == code)
                .OrderByDescending(t => t.Version)
                .ToListAsync(cancellationToken);
        }

        public async Task<TariffGrid?> GetActiveGridByCodeAsync(string code, DateTime date)
        {
            return await _context.Set<TariffGrid>()
                .FirstOrDefaultAsync(t => t.Code == code &&
                    !t.IsDeleted &&
                    t.IsActive &&
                    t.ValidFrom <= date &&
                    (t.ValidTo == null || t.ValidTo >= date));
        }

        public async Task<int> GetNextVersionAsync(string code)
        {
            var maxVersion = await _context.Set<TariffGrid>()
                .Where(t => t.Code == code)
                .MaxAsync(t => (int?)t.Version) ?? 0;

            return maxVersion + 1;
        }

        public async Task<TariffGrid> CloneGridAsync(Guid id, string newCode, string? newName)
        {
            var sourceGrid = await GetByIdWithLinesAsync(id, CancellationToken.None);
            if (sourceGrid == null)
                throw new Exception($"Tariff grid with id {id} not found");

            var clonedGrid = new TariffGrid
            {
                Id = Guid.NewGuid(),
                Code = newCode,
                Name = newName ?? $"{sourceGrid.Name} (Copy)",
                Description = sourceGrid.Description,
                TransportMode = sourceGrid.TransportMode,
                Version = await GetNextVersionAsync(newCode),
                ValidFrom = sourceGrid.ValidFrom,
                ValidTo = sourceGrid.ValidTo,
                IsActive = sourceGrid.IsActive,
                CurrencyCode = sourceGrid.CurrencyCode,
                CreatedDate = DateTime.UtcNow,
                TariffLines = new List<TariffLine>()
            };

            // Clone lines - enlevant les propriétés qui n'existent pas
            foreach (var line in sourceGrid.TariffLines.Where(l => !l.IsDeleted))
            {
                clonedGrid.TariffLines.Add(new TariffLine
                {
                    Id = Guid.NewGuid(),
                    TariffGridId = clonedGrid.Id,
                    ZoneFromId = line.ZoneFromId,
                    ZoneToId = line.ZoneToId,
                    PricePerKg = line.PricePerKg,
                    MinWeight = line.MinWeight,
                    MaxWeight = line.MaxWeight,
                    PricePerM3 = line.PricePerM3,
                    MinVolume = line.MinVolume,
                    MaxVolume = line.MaxVolume,
                    PricePerContainer20ft = line.PricePerContainer20ft,
                    PricePerContainer40ft = line.PricePerContainer40ft,
                    // PricePerContainer40ftHC n'existe pas - supprimé
                    // PricePerLinearMeter n'existe pas - supprimé
                    BasePrice = line.BasePrice,
                    TransitDays = line.TransitDays,
                    // MerchandiseTypeId n'existe pas dans votre entité - supprimé
                    CreatedDate = DateTime.UtcNow
                });
            }

            await _context.Set<TariffGrid>().AddAsync(clonedGrid);
            return clonedGrid;
        }

        #region Tariff Lines Methods

        public async Task<TariffLine?> GetLineByIdAsync(Guid lineId, CancellationToken cancellationToken)
        {
            return await _context.Set<TariffLine>()
                .Include(l => l.ZoneFrom)
                .Include(l => l.ZoneTo)
                .FirstOrDefaultAsync(l => l.Id == lineId && !l.IsDeleted, cancellationToken);
        }

        public async Task<IEnumerable<TariffLine>> GetLinesByGridIdAsync(Guid gridId, CancellationToken cancellationToken)
        {
            return await _context.Set<TariffLine>()
                .Include(l => l.ZoneFrom)
                .Include(l => l.ZoneTo)
                .Where(l => l.TariffGridId == gridId && !l.IsDeleted)
                .OrderBy(l => l.ZoneFrom != null ? l.ZoneFrom.Name : "")
                .ThenBy(l => l.ZoneTo != null ? l.ZoneTo.Name : "")
                .ToListAsync(cancellationToken);
        }

        public async Task<TariffLine> AddLineAsync(TariffLine line)
        {
            await _context.Set<TariffLine>().AddAsync(line);
            return line;
        }

        public async Task AddLinesBulkAsync(List<TariffLine> lines)
        {
            await _context.Set<TariffLine>().AddRangeAsync(lines);
        }

        public async Task UpdateLineAsync(TariffLine line)
        {
            _context.Set<TariffLine>().Update(line);
            await Task.CompletedTask;
        }

        public async Task<bool> DeleteLineAsync(Guid lineId)
        {
            var line = await _context.Set<TariffLine>().FindAsync(lineId);
            if (line == null)
                return false;

            if (line is ISoftDeleteable softDeletableEntity)
            {
                softDeletableEntity.IsDeleted = true;
                softDeletableEntity.DeletedDate = DateTime.UtcNow;
                _context.Entry(line).State = EntityState.Modified;
                return true;
            }

            return false;
        }

        public async Task<bool> LineExistsInGridAsync(Guid gridId, Guid? zoneFromId, Guid? zoneToId, Guid? excludeLineId = null)
        {
            var query = _context.Set<TariffLine>()
                .Where(l => l.TariffGridId == gridId &&
                           !l.IsDeleted &&
                           l.ZoneFromId == zoneFromId &&
                           l.ZoneToId == zoneToId);

            if (excludeLineId.HasValue)
            {
                query = query.Where(l => l.Id != excludeLineId.Value);
            }

            return await query.AnyAsync();
        }

        #endregion
    }
}