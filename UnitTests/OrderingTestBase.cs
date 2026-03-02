using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using SwiftScale.Modules.Ordering.Infrastructure;

namespace UnitTests
{
    public abstract class OrderingTestBase
    {
        // Centralized helper to create a fresh In-Memory DbContext for every test
        protected OrderingDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<OrderingDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Isolation is key
                .Options;

            // Mocking the Publisher because the DbContext constructor requires it, 
            // but we don't want to trigger real events during unit tests.
            var mockPublisher = new Mock<IPublisher>();

            return new OrderingDbContext(options, mockPublisher.Object);
        }
    }
}
