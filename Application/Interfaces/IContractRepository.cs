using Domain.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IContractRepository : IGenericRepository<Contract>
    {
        Task<Contract?> GetActiveContractForClientAsync(Guid clientId);
        Task<Contract?> GetByNumberAsync(string contractNumber);
        Task Delete(Contract contract);
        Task DeleteAsync(Contract contract, CancellationToken cancellationToken = default);
        Task<PagedList<Contract>> GetAllWithTypesAsync(int? pageNumber, int? pageSize, CancellationToken cancellationToken);
        Task<IReadOnlyList<Contract>> GetContractsByClientIdAsync(
            Guid clientId,
            CancellationToken cancellationToken = default);
        Task<List<Contract>> GetByClientIdAsync(Guid clientId, CancellationToken ct = default);
        Task<List<Contract>> GetExpiringAsync(int days, CancellationToken ct = default);
        Task<Contract?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default);
        Task UpdateAsync(Contract entity, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
        Task<Contract?> GetByIdAsync(Guid id, CancellationToken ct = default);

        Task<bool> ExistsAsync(Guid contractId, CancellationToken ct = default);
        Task AddContractPricingAsync(ContractPricing pricing, CancellationToken ct = default);

        Task<IReadOnlyList<ContractPricing>> GetContractPricingByContractIdAsync(Guid contractId, CancellationToken ct = default);

        Task<ContractPricing?> GetContractPricingByIdAsync(Guid ContractId,Guid CpId, CancellationToken ct = default);

        Task<Contract?> GetActiveContractForClientAsync(Guid clientId, CancellationToken cancellationToken = default);
    }
}
