using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistance.Data;

namespace Persistance.Repositories
{
    public class ClientRepository : GenericRepository<Client>, IClientRepository
    {
        public ClientRepository(CleanArchitecturContext context) : base(context)
        {
        }

        public async Task<PagedList<Client>> GetAllWithTypesAsync(int? pageNumber, int? pageSize, CancellationToken cancellationToken)
        {
            var query = await _context.Clients
                                        .AsQueryable()
                                        .AsNoTracking()
                                        .Where(e => e.IsDeleted == false)
                                        .ToListAsync(cancellationToken);
            int totalRows = query.AsEnumerable().Count();
            var customer = query
           .Skip(pageNumber.HasValue ? (pageNumber.Value - 1) * pageSize.GetValueOrDefault(1) : 0)
           .Take(pageSize.GetValueOrDefault(int.MaxValue)).ToList();

            return new PagedList<Client>(customer, totalRows, pageNumber, pageSize);
        }

        public async Task<Client?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            return await _context.Clients
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Code == code && !c.IsDeleted, cancellationToken);
        }

        
    }
}
