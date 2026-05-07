using System;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Persistance.Data;

namespace PersistanceTests.Data
{
    [TestClass]
    public class UserUpdateTests
    {
        private static DbContextOptions<CleanArchitecturContext> CreateOptions()
        {
            return new DbContextOptionsBuilder<CleanArchitecturContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        [TestMethod]
        public async Task UserCrud_Should_Update_User()
        {
            var options = CreateOptions();

            using var context = new CleanArchitecturContext(options);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "update.user@test.com",
                PasswordHash = "hashed",
                FirstName = "Old",
                LastName = "User",
                Role = UserRole.Operateur,
                IsActive = true
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            user.FirstName = "Updated";
            user.Role = UserRole.Gestionnaire;

            await context.SaveChangesAsync();

            var updated = await context.Users.SingleOrDefaultAsync(u => u.Email == user.Email);

            Assert.IsNotNull(updated);
            Assert.AreEqual("Updated", updated.FirstName);
            Assert.AreEqual(UserRole.Gestionnaire, updated.Role);
        }
    }
}
