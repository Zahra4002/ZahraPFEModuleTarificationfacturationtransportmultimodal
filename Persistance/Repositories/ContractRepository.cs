using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistance.Data;
using System;
using System.Threading.Tasks;

namespace Persistance.Repositories
{
    public class ContractRepository : GenericRepository<Contract>, IContractRepository
    {
        

        public ContractRepository(CleanArchitecturContext context) : base(context)
        {
            
        }
        public async Task<PagedList<Contract>> GetAllWithTypesAsync(int? pageNumber, int? pageSize, CancellationToken cancellationToken)
        {
            var query = await _context.Contracts
                                        .AsQueryable()
                                        .AsNoTracking()
                                        .Where(e => e.IsDeleted == false)
                                        .ToListAsync(cancellationToken);
            int totalRows = query.AsEnumerable().Count();
            var customer = query
           .Skip(pageNumber.HasValue ? (pageNumber.Value - 1) * pageSize.GetValueOrDefault(1) : 0)
           .Take(pageSize.GetValueOrDefault(int.MaxValue)).ToList();

            return new PagedList<Contract>(customer, totalRows, pageNumber, pageSize);
        }
        public async Task<IReadOnlyList<Contract>> GetContractsByClientIdAsync(
          Guid clientId,
          CancellationToken cancellationToken = default)
        {
            return await _context.Contracts
                .AsNoTracking()
                .Where(c => c.ClientId == clientId)

                .OrderByDescending(c => c.ValidFrom)
                .ToListAsync(cancellationToken);
        }
        public async Task<List<Contract>> GetByClientIdAsync(Guid clientId, CancellationToken ct = default)
        {
            return await _context.Contracts
                .AsNoTracking()
                .Where(c => c.ClientId == clientId)
                .OrderByDescending(c => c.ValidFrom)
                .ToListAsync(ct);
        }
        public async Task<List<Contract>> GetExpiringAsync(int days, CancellationToken ct = default)
        {
            return await _context.Contracts
                .Where(c => c.IsActive && c.ValidTo <= DateTime.UtcNow.AddDays(days))
                .OrderBy(c => c.ValidTo)
                .ToListAsync(ct);
        }
        public async Task<Contract?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.Contracts
                .Include(c => c.ContractPricings)
                .Include(c => c.Client)
                .Include(c => c.Supplier)
                .FirstOrDefaultAsync(c => c.Id == id, ct);
        }
        public async Task UpdateAsync(Contract contract, CancellationToken ct)
        {
            _context.Contracts.Update(contract);
            await _context.SaveChangesAsync(ct);
        }
        public async Task SaveChangesAsync(CancellationToken ct)
        {
            await _context.SaveChangesAsync(ct);
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

        public async Task<bool> ExistsAsync(Guid contractId, CancellationToken ct = default)
        {
            return await _context.Contracts.AnyAsync(c => c.Id == contractId && !c.IsDeleted, ct);
        }

        public async Task AddContractPricingAsync(ContractPricing pricing, CancellationToken ct = default)
        {
            await _context.ContractPricings.AddAsync(pricing, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<IReadOnlyList<ContractPricing>> GetContractPricingByContractIdAsync(Guid contractId, CancellationToken ct = default)
        {
            return await _context.ContractPricings
                .Where(cp => cp.ContractId == contractId && !cp.IsDeleted)
                .OrderBy(cp => cp.CreatedDate)
                .ToListAsync(ct);
        }

        public async Task<ContractPricing?> GetContractPricingByIdAsync(Guid ContractId, Guid CpId, CancellationToken ct = default)
        {
            return await _context.ContractPricings
                .FirstOrDefaultAsync(cp => cp.ContractId == ContractId && cp.Id == CpId && !cp.IsDeleted, ct);
        }

        public Task<Contract?> GetActiveContractForClientAsync(Guid clientId, CancellationToken cancellationToken = default)
        {
             return _context.Contracts
                .Where(c => c.ClientId == clientId && c.IsActive && !c.IsDeleted)
                .OrderByDescending(c => c.ValidFrom)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}