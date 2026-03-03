
using Domain.Common;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        // Méthodes de base
        Task<User?> GetByEmailAsync(string email);
        Task<PagedList<User>> GetAllWithFiltersAsync(
            int? pageNumber,
            int? pageSize,
            string? sortBy,
            bool sortDescending,
            string? searchTerm,
            CancellationToken cancellationToken);
        Task<int> GetAdministratorCountAsync();
        Task AddAsync(User user);

        // Méthodes d'authentification
        Task UpdateLastLoginAsync(Guid userId);
        Task RecordFailedLoginAttemptAsync(string email);
        Task<bool> IsUserLockedOutAsync(string email);
        Task UpdateRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiryTime);
        Task RevokeRefreshTokenAsync(Guid userId);
        Task<User?> GetByRefreshTokenAsync(string refreshToken);

        // Méthodes de gestion des utilisateurs
        Task SetUserActiveStatusAsync(Guid userId, bool isActive);
        Task ChangePasswordAsync(Guid userId, string newPasswordHash);
        Task<IEnumerable<User>> GetAllActiveAsync();
        Task<IEnumerable<User>> GetUsersByRoleAsync(Domain.Enums.UserRole role);
        Task<IEnumerable<User>> SearchUsersAsync(string searchTerm);
        Task<IEnumerable<User>> GetLockedOutUsersAsync();
        Task UnlockUserAsync(Guid userId);
        Task<bool> RestoreUser(Guid userId);
        Task<User?> GetByIdIncludingDeletedAsync(Guid id, CancellationToken cancellationToken);

    }
}