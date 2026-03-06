using Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IZoneRepository
    {
        Task<Zone?> GetByIdAsync(Guid id);
    }
}