using Domain.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<PagedList<User>> GetAllWithTypesAsync(int? pageNumber, int? pageSize, CancellationToken cancellationToken);

        Task AddAsync(User user);
    }
}
