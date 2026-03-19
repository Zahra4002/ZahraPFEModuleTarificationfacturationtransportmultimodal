using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Persistance.Data;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Persistance.Repositories
{
    public class ShipmentRepository : GenericRepository<Shipment>, IShipmentRepository
    {
        public ShipmentRepository(CleanArchitecturContext context) : base(context)
        {
            
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
            // Vérification explicite
            if (_context == null)
                throw new InvalidOperationException("_context est null dans ShipmentRepository");

            if (_context.Shipments == null)
                throw new InvalidOperationException("_context.Shipments est null");

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
        public async Task<List<Shipment>> GetAllWithTypeAsync(int? pageNumber, int? pageSize, string? sortBy, bool sortDescending = false, string? searchTerm = null, CancellationToken cancellationToken = default)
        {
            var Query = _context.Shipments
                .AsNoTracking()
                .AsQueryable();
            // Filtering by search term (searching in ShipmentNumber, Origin City, and Destination City)
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                Query = Query.Where(s =>
                    s.ShipmentNumber.Contains(searchTerm) ||
                    s.OriginAddress.City.Contains(searchTerm) ||
                    s.DestinationAddress.City.Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                Query = sortBy switch
                {
                    "ShipmentNumber" => sortDescending ? Query.OrderByDescending(s => s.ShipmentNumber) : Query.OrderBy(s => s.ShipmentNumber),
                    "OriginCity" => sortDescending ? Query.OrderByDescending(s => s.OriginAddress.City) : Query.OrderBy(s => s.OriginAddress.City),
                    "DestinationCity" => sortDescending ? Query.OrderByDescending(s => s.DestinationAddress.City) : Query.OrderBy(s => s.DestinationAddress.City),
                    _ => Query
                };
            }

            // Optional Pagination
            if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
            {
                int skip = (pageNumber.Value - 1) * pageSize.Value;
                Query = Query.Skip(skip).Take(pageSize.Value);
            }

            return await Query.ToListAsync(cancellationToken);
        }

        public Task<Shipment> GetShipementWithIncules(Guid shipmentId, string[] includes, CancellationToken cancellation)
        {

            var shipment = _context.Shipments
                .AsNoTracking()
                .Where(s => s.Id == shipmentId)
                .AsQueryable();

            foreach (var include in includes)
            {
                shipment = shipment.Include(include);
            }

            return shipment.FirstOrDefaultAsync();
        }

        public async Task<Dictionary<bool, string>> IsExitAsync(Guid clientId, Guid quoteId, Guid merchandiseTypeId)
        {
            bool clientExists = await _context.Clients.AnyAsync(c => c.Id == clientId);
            if (!clientExists)
            {
                return new Dictionary<bool, string>
                {
                    { false, "ClientId does not exist." }
                };
            }

            bool quoteExists = await _context.Quotes.AnyAsync(q => q.Id == quoteId);
            if (!quoteExists)
            {
                return new Dictionary<bool, string>
                {
                    { false, "QuoteId does not exist." }
                };
            }

            bool merchandiseTypeExists = await _context.MerchandiseTypes.AnyAsync(m => m.Id == merchandiseTypeId);
            if (!merchandiseTypeExists)
            {
                return new Dictionary<bool, string>
                {
                    { false, "MerchandiseTypeId does not exist." }
                };
            }

            return new Dictionary<bool, string>
            {
                { true, "All entities exist." }
            };
        }

        public async Task<Dictionary<bool, string>> IsExitAsync(Guid clientId)
        {
            bool clientExists = await _context.Clients.AnyAsync(c => c.Id == clientId);

            return clientExists
                ? new Dictionary<bool, string> { { true, "ClientId exists." } }
                : new Dictionary<bool, string> { { false, "ClientId does not exist." } };
        }

        public async Task<List<Shipment>> SelectManyAsync(Expression<Func<Shipment, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _context.Shipments
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync(cancellationToken);
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

        public Task<Shipment?> GetShipmentWithIncludesAsync(Guid id, string[] includes, CancellationToken cancellationToken = default)
        {
             var shipmentQuery = _context.Shipments
                .AsNoTracking()
                .Where(s => s.Id == id)
                .AsQueryable();

            foreach (var shipment in shipmentQuery) {
                foreach (var include in includes)
                {
                    shipmentQuery = shipmentQuery.Include(include);
                }
            }

            return shipmentQuery.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<Shipment>> GetShipmentsForPriceCalculationAsync(
        Guid clientId,
        Guid zoneFromId,
        Guid zoneToId,
        TransportMode transportMode,
        decimal weightKg,
        decimal volumeM3,
        ContainerType containerType,
        int containerCount,
        Guid merchandiseTypeId,
        DateTime? date,
        CancellationToken cancellationToken = default)
        {
            return await _context.Shipments
                .AsNoTracking()
                .Where(s => !s.IsDeleted
                            && s.ClientId == clientId
                            && s.MerchandiseTypeId == merchandiseTypeId
                            && s.WeightKg == weightKg
                            && s.VolumeM3 == volumeM3
                            && s.ContainerType == containerType
                            && s.ContainerCount == containerCount
                            && s.Segments.Any(seg => seg.ZoneFromId == zoneFromId
                                                    && seg.ZoneToId == zoneToId
                                                    && seg.TransportMode == transportMode))
                // Shipment relations
                .Include(s => s.Client)
                .Include(s => s.Quote)
                .Include(s => s.MerchandiseType)

                // Segments et leurs relations
                .Include(s => s.Segments)
                    .ThenInclude(seg => seg.Supplier)
                .Include(s => s.Segments)
                    .ThenInclude(seg => seg.ZoneFrom)
                .Include(s => s.Segments)
                    .ThenInclude(seg => seg.ZoneTo)

                // Invoices et leurs relations
                .Include(s => s.Invoices)

                .ToListAsync(cancellationToken);
        }

        public Task<Shipment> GetShipmentByIdWithDetailsAsync(Guid shipmentId, CancellationToken cancellationToken )
        {
            return _context.Shipments
                .AsNoTracking()
                .Where(s => s.Id == shipmentId)
                .Include(s => s.Segments) 
                .Include(s => s.Invoices)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}