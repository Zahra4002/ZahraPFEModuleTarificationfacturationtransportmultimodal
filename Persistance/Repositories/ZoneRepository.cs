using System;
using System.Threading.Tasks;
using Application.Interfaces;
using Persistance.Data;
using Domain.Entities;

namespace Persistance.Repositories
{
    public class ZoneRepository : IZoneRepository
    {
        private readonly CleanArchitecturContext _context;

        public ZoneRepository(CleanArchitecturContext context)
        {
            _context = context;
        }

        public async Task<Zone?> GetByIdAsync(Guid id)
        {
            // Utilise le DbContext pour récupérer l'entité Zone.
            // Adapte le nom du DbSet si nécessaire (ex: _context.Zones).
            return await _context.Set<Zone>().FindAsync(id);
        }
    }
}
