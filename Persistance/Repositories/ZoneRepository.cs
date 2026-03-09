using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistance.Data;
using System.Linq.Expressions;

namespace Persistance.Repositories
{
    public class ZoneRepository : GenericRepository<Zone>, IZoneRepository
    {
        private readonly CleanArchitecturContext _context;

        public ZoneRepository(CleanArchitecturContext context) : base(context)
        {
        }

        public async Task<Zone?> GetByIdAsync(Guid id)
        {
            // Utilise le DbContext pour récupérer l'entité Zone.
            // Adapte le nom du DbSet si nécessaire (ex: _context.Zones).
            return await _context.Set<Zone>().FindAsync(id);
        }
        public async Task<List<Zone>> GetAllWithTypesAsync(
            int? pageNumber = null,
            int? pageSize = null,
            string? sortedBy = null,
            bool sortDescending = false,
            string? searchTerm = null,
            CancellationToken cancellationToken = default)
                {
                    var query = _context.Zones
                        .AsNoTracking()
                .AsQueryable();

            // Optional search filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(z =>
                    z.Name.Contains(searchTerm) ||
                    z.Code.Contains(searchTerm) ||
                    z.Country.Contains(searchTerm));
            }

            // Optional dynamic sorting
            if (!string.IsNullOrWhiteSpace(sortedBy))
            {
                switch (sortedBy.ToLower())
                {
                    case "name":
                        query = sortDescending ? query.OrderByDescending(z => z.Name) : query.OrderBy(z => z.Name);
                        break;
                    case "code":
                        query = sortDescending ? query.OrderByDescending(z => z.Code) : query.OrderBy(z => z.Code);
                        break;
                    case "country":
                        query = sortDescending ? query.OrderByDescending(z => z.Country) : query.OrderBy(z => z.Country);
                        break;
                    default:
                        query = sortDescending ? query.OrderByDescending(z => z.Name) : query.OrderBy(z => z.Name);
                        break;
                }
            }
            else
            {
                query = sortDescending ? query.OrderByDescending(z => z.Name) : query.OrderBy(z => z.Name);
            }

            // Optional pagination
            if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
            {
                int skip = (pageNumber.Value - 1) * pageSize.Value;
                query = query.Skip(skip).Take(pageSize.Value);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<List<Zone>> SelectManyAsync(Expression<Func<Zone, bool>> predicate)
        {
            return await _context.Zones.Where(predicate).ToListAsync();
        }

        public async Task<Zone?> SelectOneAsync(Expression<Func<Zone, bool>> predicate)
        {
            return await _context.Zones.FirstOrDefaultAsync(predicate);
        }

    }
}
