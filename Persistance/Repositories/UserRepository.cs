// Persistence/Repositories/UserRepository.cs
using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistance.Data;

namespace Persistance.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly CleanArchitecturContext _context;

        public UserRepository(CleanArchitecturContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Récupère un utilisateur par son email
        /// </summary>
        /// <param name="email">Email de l'utilisateur</param>
        /// <returns>Utilisateur ou null</returns>
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        }

        /// <summary>
        /// Récupère tous les utilisateurs avec filtres, pagination et tri
        /// </summary>
        /// <param name="pageNumber">Numéro de page</param>
        /// <param name="pageSize">Taille de la page</param>
        /// <param name="sortBy">Champ de tri</param>
        /// <param name="sortDescending">Tri descendant</param>
        /// <param name="searchTerm">Terme de recherche</param>
        /// <param name="cancellationToken">Token d'annulation</param>
        /// <returns>Liste paginée des utilisateurs</returns>
        public async Task<PagedList<User>> GetAllWithFiltersAsync(
            int? pageNumber,
            int? pageSize,
            string? sortBy,
            bool sortDescending,
            string? searchTerm,
            CancellationToken cancellationToken)
        {
            IQueryable<User> query = _context.Users
                .Where(u => !u.IsDeleted);

            // Appliquer la recherche
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(u =>
                    u.Email.Contains(searchTerm) ||
                    u.FirstName.Contains(searchTerm) ||
                    u.LastName.Contains(searchTerm) ||
                    (u.PhoneNumber != null && u.PhoneNumber.Contains(searchTerm)));
            }

            // Appliquer le tri
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "email":
                        query = sortDescending
                            ? query.OrderByDescending(u => u.Email)
                            : query.OrderBy(u => u.Email);
                        break;
                    case "firstname":
                        query = sortDescending
                            ? query.OrderByDescending(u => u.FirstName)
                            : query.OrderBy(u => u.FirstName);
                        break;
                    case "lastname":
                        query = sortDescending
                            ? query.OrderByDescending(u => u.LastName)
                            : query.OrderBy(u => u.LastName);
                        break;
                    case "role":
                        query = sortDescending
                            ? query.OrderByDescending(u => u.Role)
                            : query.OrderBy(u => u.Role);
                        break;
                    case "createdat":
                        query = sortDescending
                            ? query.OrderByDescending(u => u.CreatedAt)
                            : query.OrderBy(u => u.CreatedAt);
                        break;
                    case "lastloginat":
                        query = sortDescending
                            ? query.OrderByDescending(u => u.LastLoginAt)
                            : query.OrderBy(u => u.LastLoginAt);
                        break;
                    case "isactive":
                        query = sortDescending
                            ? query.OrderByDescending(u => u.IsActive)
                            : query.OrderBy(u => u.IsActive);
                        break;
                    default:
                        query = query.OrderBy(u => u.CreatedAt);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(u => u.CreatedAt);
            }

            // Compter le nombre total d'éléments
            var totalCount = await query.CountAsync(cancellationToken);

            // Appliquer la pagination
            var page = pageNumber ?? 1;
            var size = pageSize ?? 10;

            var items = await query
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync(cancellationToken);

            return new PagedList<User>(items, totalCount, page, size);
        }

        /// <summary>
        /// Compte le nombre d'administrateurs actifs
        /// </summary>
        /// <returns>Nombre d'administrateurs</returns>
        public async Task<int> GetAdministratorCountAsync()
        {
            return await _context.Users
                .CountAsync(u => u.Role == Domain.Enums.UserRole.Administrateur && !u.IsDeleted);
        }

        /// <summary>
        /// Ajoute un nouvel utilisateur
        /// </summary>
        /// <param name="user">Utilisateur à ajouter</param>
        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        /// <summary>
        /// Met à jour le dernier login d'un utilisateur
        /// </summary>
        /// <param name="userId">ID de l'utilisateur</param>
        public async Task UpdateLastLoginAsync(Guid userId)
        {
            // Utilisation de GetByIdAsync avec CancellationToken
            var user = await GetByIdAsync(userId, CancellationToken.None);
            if (user != null)
            {
                user.LastLoginAt = DateTime.UtcNow;
                user.FailedLoginAttempts = 0;
                user.LockoutEnd = null;
                await Update(user);
            }
        }

        /// <summary>
        /// Enregistre une tentative de connexion échouée
        /// </summary>
        /// <param name="email">Email de l'utilisateur</param>
        public async Task RecordFailedLoginAttemptAsync(string email)
        {
            var user = await GetByEmailAsync(email);
            if (user != null)
            {
                user.FailedLoginAttempts++;

                // Verrouiller le compte après 5 tentatives échouées
                if (user.FailedLoginAttempts >= 5)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(30);
                }

                await Update(user);
            }
        }

        /// <summary>
        /// Vérifie si un utilisateur est verrouillé
        /// </summary>
        /// <param name="email">Email de l'utilisateur</param>
        /// <returns>True si verrouillé, False sinon</returns>
        public async Task<bool> IsUserLockedOutAsync(string email)
        {
            var user = await GetByEmailAsync(email);
            return user != null && user.IsLockedOut;
        }

        /// <summary>
        /// Met à jour le refresh token d'un utilisateur
        /// </summary>
        /// <param name="userId">ID de l'utilisateur</param>
        /// <param name="refreshToken">Nouveau refresh token</param>
        /// <param name="expiryTime">Date d'expiration</param>
        public async Task UpdateRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiryTime)
        {
            // Utilisation de GetByIdAsync avec CancellationToken
            var user = await GetByIdAsync(userId, CancellationToken.None);
            if (user != null)
            {
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = expiryTime;
                await Update(user);
            }
        }

        /// <summary>
        /// Révoque le refresh token d'un utilisateur
        /// </summary>
        /// <param name="userId">ID de l'utilisateur</param>
        public async Task RevokeRefreshTokenAsync(Guid userId)
        {
            // Utilisation de GetByIdAsync avec CancellationToken
            var user = await GetByIdAsync(userId, CancellationToken.None);
            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                await Update(user);
            }
        }

        /// <summary>
        /// Récupère un utilisateur par son refresh token
        /// </summary>
        /// <param name="refreshToken">Refresh token</param>
        /// <returns>Utilisateur ou null</returns>
        public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && !u.IsDeleted);
        }

        /// <summary>
        /// Active ou désactive un utilisateur
        /// </summary>
        /// <param name="userId">ID de l'utilisateur</param>
        /// <param name="isActive">Statut d'activation</param>
        public async Task SetUserActiveStatusAsync(Guid userId, bool isActive)
        {
            // Utilisation de GetByIdAsync avec CancellationToken
            var user = await GetByIdAsync(userId, CancellationToken.None);
            if (user != null)
            {
                user.IsActive = isActive;
                user.UpdatedAt = DateTime.UtcNow;
                await Update(user);
            }
        }

        /// <summary>
        /// Change le mot de passe d'un utilisateur
        /// </summary>
        /// <param name="userId">ID de l'utilisateur</param>
        /// <param name="newPasswordHash">Nouveau hash de mot de passe</param>
        public async Task ChangePasswordAsync(Guid userId, string newPasswordHash)
        {
            // Utilisation de GetByIdAsync avec CancellationToken
            var user = await GetByIdAsync(userId, CancellationToken.None);
            if (user != null)
            {
                user.PasswordHash = newPasswordHash;
                user.UpdatedAt = DateTime.UtcNow;
                user.RefreshToken = null; // Invalider tous les refresh tokens
                user.RefreshTokenExpiryTime = null;
                await Update(user);
            }
        }

        /// <summary>
        /// Récupère tous les utilisateurs actifs
        /// </summary>
        /// <returns>Liste des utilisateurs actifs</returns>
        public async Task<IEnumerable<User>> GetAllActiveAsync()
        {
            return await _context.Users
                .Where(u => !u.IsDeleted && u.IsActive)
                .OrderBy(u => u.FirstName)
                .ToListAsync();
        }

        /// <summary>
        /// Récupère les utilisateurs par rôle
        /// </summary>
        /// <param name="role">Rôle à rechercher</param>
        /// <returns>Liste des utilisateurs avec ce rôle</returns>
        public async Task<IEnumerable<User>> GetUsersByRoleAsync(Domain.Enums.UserRole role)
        {
            return await _context.Users
                .Where(u => !u.IsDeleted && u.Role == role)
                .OrderBy(u => u.FirstName)
                .ToListAsync();
        }

        /// <summary>
        /// Recherche des utilisateurs par nom ou email
        /// </summary>
        /// <param name="searchTerm">Terme de recherche</param>
        /// <returns>Liste des utilisateurs correspondants</returns>
        public async Task<IEnumerable<User>> SearchUsersAsync(string searchTerm)
        {
            return await _context.Users
                .Where(u => !u.IsDeleted && (
                    u.Email.Contains(searchTerm) ||
                    u.FirstName.Contains(searchTerm) ||
                    u.LastName.Contains(searchTerm)))
                .OrderBy(u => u.FirstName)
                .Take(10) // Limiter les résultats
                .ToListAsync();
        }

        /// <summary>
        /// Récupère les utilisateurs verrouillés
        /// </summary>
        /// <returns>Liste des utilisateurs verrouillés</returns>
        public async Task<IEnumerable<User>> GetLockedOutUsersAsync()
        {
            return await _context.Users
                .Where(u => !u.IsDeleted && u.LockoutEnd > DateTime.UtcNow)
                .ToListAsync();
        }

        /// <summary>
        /// Déverrouille un utilisateur
        /// </summary>
        /// <param name="userId">ID de l'utilisateur</param>
        public async Task UnlockUserAsync(Guid userId)
        {
            // Utilisation de GetByIdAsync avec CancellationToken
            var user = await GetByIdAsync(userId, CancellationToken.None);
            if (user != null)
            {
                user.LockoutEnd = null;
                user.FailedLoginAttempts = 0;
                user.UpdatedAt = DateTime.UtcNow;
                await Update(user);
            }
        }
        public async Task<bool> RestoreUser(Guid userId)
        {
            return await Restore(userId);
        }

        public async Task<User?> GetByIdIncludingDeletedAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken); // Inclut les supprimés
        }
    }


}