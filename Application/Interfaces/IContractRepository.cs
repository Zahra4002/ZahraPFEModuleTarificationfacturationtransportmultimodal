using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
   public interface IContractRepository: IGenericRepository<Contract>
    {
        Task<Contract?> GetActiveContractForClientAsync(Guid clientId);
        Task<Contract?> GetByNumberAsync(string contractNumber);
        Task Delete(Contract contract);
        Task DeleteAsync(Contract contract, CancellationToken cancellationToken = default);
    }
}
