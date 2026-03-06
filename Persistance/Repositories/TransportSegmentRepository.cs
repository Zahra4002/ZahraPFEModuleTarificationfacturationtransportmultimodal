using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistance.Data;
using System;
using System.Threading.Tasks;

namespace Persistance.Repositories
{
    public class TransportSegmentRepository : GenericRepository<TransportSegment>, ITransportSegmentRepository
    {
        private readonly CleanArchitecturContext _context;

        public TransportSegmentRepository(CleanArchitecturContext context) : base(context)
        {
            _context = context;
        }

        public async Task Delete(TransportSegment segment)
        {
            _context.TransportSegments.Remove(segment);
            await Task.CompletedTask;
        }
    }
}