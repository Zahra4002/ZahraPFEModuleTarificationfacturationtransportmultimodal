using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Features.ClientFeature.Queries;
using Domain.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IClientRepository : IGenericRepository<Client>
    {
        Task<PagedList<Client>> GetAllWithTypesAsync(int? pageNumber, int? pageSize, CancellationToken cancellationToken);
        Task<Client?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    }
}
