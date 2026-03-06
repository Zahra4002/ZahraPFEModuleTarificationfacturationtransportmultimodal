using Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ITransportSegmentRepository : IGenericRepository<TransportSegment>
    {
        Task Delete(TransportSegment segment);

    }
}