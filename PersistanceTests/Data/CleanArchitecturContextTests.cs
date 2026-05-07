using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Persistance.Data;
using Microsoft.EntityFrameworkCore.InMemory;

namespace PersistanceTests.Data
{
    [TestClass]
    public class CleanArchitecturContextTests
    {
        [TestMethod]
        public void CleanArchitecturContext_Should_Create_Context()
        {
            var options = new DbContextOptionsBuilder<CleanArchitecturContext>()
                .UseInMemoryDatabase(nameof(CleanArchitecturContext_Should_Create_Context))
                .Options;

            using var context = new CleanArchitecturContext(options);

            Assert.IsNotNull(context);
        }

        [TestMethod]
        public void CleanArchitecturContext_Should_Expose_DbSets()
        {
            var options = new DbContextOptionsBuilder<CleanArchitecturContext>()
                .UseInMemoryDatabase(nameof(CleanArchitecturContext_Should_Expose_DbSets))
                .Options;

            using var context = new CleanArchitecturContext(options);

            Assert.IsNotNull(context.Users);
            Assert.IsNotNull(context.Clients);
            Assert.IsNotNull(context.Quotes);
            Assert.IsNotNull(context.Shipments);
        }

        [TestMethod]
        public void CleanArchitecturContext_Should_Allow_Add_Entity()
        {
            var options = new DbContextOptionsBuilder<CleanArchitecturContext>()
                .UseInMemoryDatabase(nameof(CleanArchitecturContext_Should_Allow_Add_Entity))
                .Options;

            using var context = new CleanArchitecturContext(options);

            context.Tests.Add(new Domain.Entities.Test());
            context.SaveChanges();

            var count = context.Tests.CountAsync().Result;

            Assert.AreEqual(1, count);
        }
    }
}