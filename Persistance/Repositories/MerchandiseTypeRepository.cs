using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistance.Data;
using Persistance.Repositories;

namespace Infrastructure.Persistance.Repositories
{
    public class MerchandiseTypeRepository : GenericRepository<MerchandiseType>, IMerchandiseTypeRepository
    {
        private readonly CleanArchitecturContext _context;
        private readonly DbSet<MerchandiseType> _dbSet;

        public MerchandiseTypeRepository(CleanArchitecturContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<MerchandiseType>();
        }

        public async Task<MerchandiseType?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(x => x.Code == code && !x.IsDeleted, cancellationToken);
        }

        public async Task<List<MerchandiseType>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<PagedList<MerchandiseType>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? sortBy,
            bool sortDescending,
            string? searchTerm,
            bool? isActive,
            CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(x => !x.IsDeleted);

            // Filtre par recherche
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(x =>
                    x.Code.Contains(searchTerm) ||
                    x.Name.Contains(searchTerm) ||
                    (x.Description != null && x.Description.Contains(searchTerm)));
            }

            // Filtre par statut actif
            if (isActive.HasValue)
            {
                query = query.Where(x => x.IsActive == isActive.Value);
            }

            // Tri
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "code":
                        query = sortDescending ? query.OrderByDescending(x => x.Code) : query.OrderBy(x => x.Code);
                        break;
                    case "name":
                        query = sortDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name);
                        break;
                    default:
                        query = query.OrderBy(x => x.Name);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(x => x.Name);
            }

            // Pagination manuelle
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedList<MerchandiseType>(items, totalCount, pageNumber, pageSize);
        }
    }
}