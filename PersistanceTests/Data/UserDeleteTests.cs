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
    public class UserDeleteTests
    {
        private static DbContextOptions<CleanArchitecturContext> CreateOptions()
        {
            return new DbContextOptionsBuilder<CleanArchitecturContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        [TestMethod]
        public async Task UserCrud_Should_Delete_User()
        {
            var options = CreateOptions();

            using var context = new CleanArchitecturContext(options);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "delete.user@test.com",
                PasswordHash = "hashed",
                FirstName = "Delete",
                LastName = "User",
                Role = UserRole.Operateur,
                IsActive = true
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            context.Users.Remove(user);
            await context.SaveChangesAsync();

            var deleted = await context.Users.SingleOrDefaultAsync(u => u.Email == user.Email);

            Assert.IsNull(deleted);
        }
    }
}
