using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using Persistance.Data;
using Domain.Entities;

namespace Persistance.Repositories
{
    public class ShipmentRepository : IShipmentRepository
    {
        private readonly CleanArchitecturContext _context;

        public ShipmentRepository(CleanArchitecturContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Shipment?> GetByIdWithDetailsAsync(Guid id)
        {
            // Implémentation minimale et sûre : recherche par clé primaire.
            // Si vous devez charger des relations, remplacez par une requête avec Include(...).
            var entity = await _context.Set<Shipment>().FindAsync(id);
            return entity;
        }
        public async Task<Shipment?> GetByQuoteIdAsync(Guid quoteId)
        {
            return await _context.Shipments
                .FirstOrDefaultAsync(s => s.QuoteId == quoteId && !s.IsDeleted);
        }

        public async Task AddAsync(Shipment shipment)
        {
            await _context.Shipments.AddAsync(shipment);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<Shipment>> GetShipmentsByDateRangeWithSegmentsAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
        {
            return await _context.Shipments
                .AsNoTracking()
                .Where(s => !s.IsDeleted
                            && s.CreatedDate >= from
                            && s.CreatedDate <= to)
                .Include(s => s.Segments)
                .ToListAsync(cancellationToken);
        }
    }
}