using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SwiftScale.BuildingBlocks.Messaging;
using SwiftScale.Modules.Ordering.Application.Payments.HandlePaymentCompleted;
using SwiftScale.Modules.Ordering.Domain;
using SwiftScale.Modules.Ordering.Infrastructure;
using SwiftScale.Modules.Payment.Application.ProcessPayment;

namespace UnitTests.Domain
{
    public class OrderTests
    {
        [Fact]
        public void MarkAsPaid_ShouldChangeStatusToPaid_WhenStatusIsPending()
        {
            // Arrange
            var order = Order.Create(Guid.NewGuid());

            // Act
            order.MarkAsPaid();

            // Assert
            order.Status.Should().Be(OrderStatus.Paid);
        }

        [Fact]
        public void MarkAsPaid_ShouldRaiseOrderPaidDomainEvent_WhenMarkingAsPaid()
        {
            // Arrange
            var order = Order.Create(Guid.NewGuid());

            // Act
            order.MarkAsPaid();

            // Assert
            order.DomainEvents.Should().Contain(evt => evt.GetType().Name == "OrderPaidDomainEvent");
        }

        [Theory]
        [InlineData(OrderStatus.Paid)]
        [InlineData(OrderStatus.Shipped)]
        [InlineData(OrderStatus.Cancelled)]
        [InlineData(OrderStatus.Completed)]
        public void MarkAsPaid_ShouldThrowInvalidOperationException_WhenStatusIsNotPending(OrderStatus status)
        {
            // Arrange
            var order = Order.Create(Guid.NewGuid());
            // Manually set status to non-pending (simulating different states)
            var reflection = typeof(Order).GetProperty(nameof(Order.Status));
            reflection?.SetValue(order, status);

            // Act & Assert
            var action = () => order.MarkAsPaid();
            action.Should().Throw<InvalidOperationException>()
                .WithMessage("Order cannot be marked as paid in its current state.");
        }

        [Fact]
        public void Create_ShouldInitializeOrderWithPendingStatus()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var order = Order.Create(userId);

            // Assert
            order.UserId.Should().Be(userId);
            order.Status.Should().Be(OrderStatus.Pending);
            order.Id.Should().NotBeEmpty();
            order.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void Create_ShouldRaiseOrderCreatedDomainEvent()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var order = Order.Create(userId);

            // Assert
            order.DomainEvents.Should().HaveCount(1);
            order.DomainEvents.Should().Contain(evt => evt.GetType().Name == "OrderCreatedDomainEvent");
        }

        [Fact]
        public void AddItem_ShouldAddOrderItemToCollection()
        {
            // Arrange
            var order = Order.Create(Guid.NewGuid());
            var productId = Guid.NewGuid();
            var price = 99.99m;
            var quantity = 2;

            // Act
            order.AddItem(productId, price, quantity);

            // Assert
            order.Items.Should().HaveCount(1);
            var item = order.Items.First();
            item.ProductId.Should().Be(productId);
            item.UnitPrice.Should().Be(price);
            item.Quantity.Should().Be(quantity);
        }

        [Fact]
        public void AddItem_ShouldAllowMultipleItems()
        {
            // Arrange
            var order = Order.Create(Guid.NewGuid());
            var productId1 = Guid.NewGuid();
            var productId2 = Guid.NewGuid();

            // Act
            order.AddItem(productId1, 50.00m, 1);
            order.AddItem(productId2, 75.00m, 2);

            // Assert
            order.Items.Should().HaveCount(2);
            order.Items.Should().Contain(item => item.ProductId == productId1);
            order.Items.Should().Contain(item => item.ProductId == productId2);
        }

        //[Fact]
        //public void AddItem_ShouldMaintainOrderItemImmutability_ViaThroughReadOnlyList()
        //{
        //    // Arrange
        //    var order = Order.Create(Guid.NewGuid());
        //    order.AddItem(Guid.NewGuid(), 50.00m, 1);

        //    // Act & Assert
        //    var action = () => order.Items.Add(new OrderItem(order.Id, Guid.NewGuid(), 100m, 1));
        //    action.Should().Throw<NotSupportedException>();
        //}
    }

    public class PaymentCompletedInboxHandlerTests : OrderingTestBase
    {
        [Fact]
        public async Task Handle_ShouldSkip_WhenAlreadyInInbox()
        {
            // Arrange
            using var context = CreateContext();
            var handler = new PaymentCompletedInboxHandler(context, Mock.Of<ILogger<PaymentCompletedInboxHandler>>());

            var eventId = Guid.NewGuid();
            context.InboxMessages.Add(new InboxMessage { Id = eventId });
            await context.SaveChangesAsync();

            // Act
            var paymentEvent = new PaymentCompletedIntegrationEvent(eventId, Guid.NewGuid(), 100, DateTime.UtcNow);
            await handler.Handle(paymentEvent, CancellationToken.None);

            // Assert
            context.InboxMessages.Count().Should().Be(1);
        }

        [Fact]
        public async Task Handle_ShouldMarkOrderAsPaid_WhenOrderExists()
        {
            // Arrange
            using var context = CreateContext();
            var handler = new PaymentCompletedInboxHandler(context, Mock.Of<ILogger<PaymentCompletedInboxHandler>>());

            var orderId = Guid.NewGuid();
            var order = Order.Create(Guid.NewGuid());
            // Manually set the ID for testing
            typeof(Order).GetProperty(nameof(Order.Id))?.SetValue(order, orderId);
            
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var eventId = Guid.NewGuid();

            // Act
            var paymentEvent = new PaymentCompletedIntegrationEvent(eventId, orderId, 100, DateTime.UtcNow);
            await handler.Handle(paymentEvent, CancellationToken.None);

            // Assert
            var updatedOrder = await context.Orders.FindAsync(orderId);
            updatedOrder.Should().NotBeNull();
            updatedOrder!.Status.Should().Be(OrderStatus.Paid);
        }

        [Fact]
        public async Task Handle_ShouldRecordInboxMessage_WhenPaymentProcessed()
        {
            // Arrange
            using var context = CreateContext();
            var handler = new PaymentCompletedInboxHandler(context, Mock.Of<ILogger<PaymentCompletedInboxHandler>>());

            var orderId = Guid.NewGuid();
            var order = Order.Create(Guid.NewGuid());
            typeof(Order).GetProperty(nameof(Order.Id))?.SetValue(order, orderId);
            
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var eventId = Guid.NewGuid();

            // Act
            var paymentEvent = new PaymentCompletedIntegrationEvent(eventId, orderId, 100, DateTime.UtcNow);
            await handler.Handle(paymentEvent, CancellationToken.None);

            // Assert
            context.InboxMessages.Should().HaveCount(1);
            var inboxMessage = context.InboxMessages.First();
            inboxMessage.Id.Should().Be(eventId);
            inboxMessage.Type.Should().Be(nameof(PaymentCompletedIntegrationEvent));
            inboxMessage.ProcessedOnUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task Handle_ShouldLogError_WhenOrderNotFound()
        {
            // Arrange
            using var context = CreateContext();
            var mockLogger = new Mock<ILogger<PaymentCompletedInboxHandler>>();
            var handler = new PaymentCompletedInboxHandler(context, mockLogger.Object);

            var orderId = Guid.NewGuid();
            var eventId = Guid.NewGuid();

            // Act
            var paymentEvent = new PaymentCompletedIntegrationEvent(eventId, orderId, 100, DateTime.UtcNow);
            await handler.Handle(paymentEvent, CancellationToken.None);

            // Assert
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Order {orderId} not found")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldNotCreateDuplicateInboxMessages_WhenCalledMultipleTimes()
        {
            // Arrange
            using var context = CreateContext();
            var handler = new PaymentCompletedInboxHandler(context, Mock.Of<ILogger<PaymentCompletedInboxHandler>>());

            var orderId = Guid.NewGuid();
            var order = Order.Create(Guid.NewGuid());
            typeof(Order).GetProperty(nameof(Order.Id))?.SetValue(order, orderId);
            
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var eventId = Guid.NewGuid();
            var paymentEvent = new PaymentCompletedIntegrationEvent(eventId, orderId, 100, DateTime.UtcNow);

            // Act - Process the same event twice
            await handler.Handle(paymentEvent, CancellationToken.None);
            await handler.Handle(paymentEvent, CancellationToken.None);

            // Assert - Only one inbox message should exist
            context.InboxMessages.Count().Should().Be(1);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenOrderAlreadyPaid()
        {
            // Arrange
            using var context = CreateContext();
            var handler = new PaymentCompletedInboxHandler(context, Mock.Of<ILogger<PaymentCompletedInboxHandler>>());

            var orderId = Guid.NewGuid();
            var order = Order.Create(Guid.NewGuid());
            typeof(Order).GetProperty(nameof(Order.Id))?.SetValue(order, orderId);
            order.MarkAsPaid(); // Mark as paid first
            
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var eventId = Guid.NewGuid();

            // Act & Assert
            var paymentEvent = new PaymentCompletedIntegrationEvent(eventId, orderId, 100, DateTime.UtcNow);
            var action = () => handler.Handle(paymentEvent, CancellationToken.None);
            await action.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Order cannot be marked as paid in its current state.");
        }

        [Fact]
        public void Cancel_ShouldThrowException_WhenOrderIsAlreadyPaid()
        {
            // Arrange
            var order = Order.Create(Guid.NewGuid());
            order.MarkAsPaid();

            // Act
            Action act = () => order.Cancel("Customer changed mind");

            // Assert
            act.Should().Throw<InvalidOperationException>()
               .WithMessage("Order cannot be cancelled from the Paid state.");
        }
    }
}