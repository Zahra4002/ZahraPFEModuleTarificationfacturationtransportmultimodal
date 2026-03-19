using Domain.Entities;
using Domain.Enums;
using System.Linq.Expressions;

namespace Application.Interfaces
{
    public interface IShipmentRepository : IGenericRepository<Shipment>
    {
        
        Task<Shipment?> GetByIdWithDetailsAsync(Guid id);

        Task<Shipment?> GetShipmentWithIncludesAsync(Guid id, string[] includes, CancellationToken cancellationToken = default);
        Task<Shipment?> GetByQuoteIdAsync(Guid quoteId);
        Task<List<Shipment>> GetAllWithTypeAsync
           (
           int? pageNumber,
              int? pageSize,
              string? sortBy,
              bool sortDescending = false,
              string? searchTerm = null,
              CancellationToken cancellationToken = default
           );

        Task AddAsync(Shipment shipment);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<Dictionary<bool, string>> IsExitAsync(Guid clientId, Guid quoteId, Guid merchandiseTypeId);

        Task<Dictionary<bool, string>> IsExitAsync(Guid clientId);

        Task<List<Shipment>> SelectManyAsync(Expression<Func<Shipment, bool>> predicate, CancellationToken cancellationToken = default);

        Task<Shipment> GetShipementWithIncules(Guid shipmentId, string[] includes, CancellationToken cancellation);


        Task<List<Shipment>> GetShipmentsByDateRangeWithSegmentsAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);

        Task<List<Shipment>> GetShipmentsForPriceCalculationAsync(
            Guid clientId,
            Guid zoneFromId,
            Guid zoneToId, 
            TransportMode transportMode,
            decimal weightKg,
            decimal volumeM3,
            ContainerType containerType, 
            int containerCount, 
            Guid merchandiseTypeId, 
            DateTime? date, CancellationToken 
            cancellationToken = default
            );

        Task<Shipment> GetShipmentByIdWithDetailsAsync(Guid shipmentId, CancellationToken cancellationToken );




    }
}
