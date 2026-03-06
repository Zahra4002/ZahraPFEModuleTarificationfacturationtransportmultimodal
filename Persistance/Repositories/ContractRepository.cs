using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistance.Data;
using System;
using System.Threading.Tasks;

namespace Persistance.Repositories
{
    public class ContractRepository : GenericRepository<Contract>, IContractRepository
    {
        private readonly CleanArchitecturContext _context;

        public ContractRepository(CleanArchitecturContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Contract?> GetByNumberAsync(string contractNumber)
        {
            return await _context.Contracts
                .FirstOrDefaultAsync(c => c.ContractNumber == contractNumber && !c.IsDeleted);
        }

        public async Task Delete(Contract contract)
        {
            _context.Contracts.Remove(contract);
            await Task.CompletedTask;
        }

        Task<Contract?> IContractRepository.GetActiveContractForClientAsync(Guid clientId)
        {
            throw new NotImplementedException();
        }
        public async Task DeleteAsync(Contract contract, CancellationToken cancellationToken = default)
        {
            _context.Contracts.Remove(contract);
            await Task.CompletedTask;
        }
    }
}