using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SwiftScale.BuildingBlocks;
using SwiftScale.BuildingBlocks.Auth;
using SwiftScale.BuildingBlocks.Storage;
using SwiftScale.Modules.Catalog.Application.Interfaces;
using SwiftScale.Modules.Catalog.Application.Products.CreateProduct;
using SwiftScale.Modules.Catalog.Application.UploadImage;
using SwiftScale.Modules.Catalog.Domain;
using SwiftScale.Modules.Catalog.Infrastructure;

namespace UnitTests.Catalog;

public class CatalogApplicationTests
{
    private static CatalogDbContext CreateInMemoryCatalogContext()
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var publisherMock = new Mock<IPublisher>();

        return new CatalogDbContext(options, publisherMock.Object);
    }

    [Fact]
    public async Task CreateProductCommandHandler_ShouldCreateProduct_WhenRequestIsValid()
    {
        // Arrange
        using var context = CreateInMemoryCatalogContext();
        var currentUserMock = new Mock<ICurrentUserProvider>();
        currentUserMock.SetupGet(x => x.UserId).Returns(Guid.NewGuid());

        var loggerMock = new Mock<ILogger<CreateProductCommand>>();

        var handler = new CreateProductCommandHandler(context, currentUserMock.Object, loggerMock.Object);

        var command = new CreateProductCommand(
            "New Product",
            "Description",
            99.99m,
            "ABCD1234");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();

        var productInDb = await context.Products.FindAsync(result.Value);
        productInDb.Should().NotBeNull();
        productInDb!.Name.Should().Be("New Product");
    }

    [Fact]
    public async Task CreateProductCommandHandler_ShouldReturnFailure_WhenMoneyCreationFails()
    {
        // Arrange
        using var context = CreateInMemoryCatalogContext();
        var currentUserMock = new Mock<ICurrentUserProvider>();
        currentUserMock.SetupGet(x => x.UserId).Returns(Guid.NewGuid());

        var loggerMock = new Mock<ILogger<CreateProductCommand>>();

        var handler = new CreateProductCommandHandler(context, currentUserMock.Object, loggerMock.Object);

        var command = new CreateProductCommand(
            "New Product",
            "Description",
            -10m, // invalid negative price
            "ABCD1234");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
        result.Error.Message.Should().Be("Amount cannot be negative.");

        context.Products.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateProductCommandHandler_ShouldReturnFailure_WhenSkuCreationFails()
    {
        // Arrange
        using var context = CreateInMemoryCatalogContext();
        var currentUserMock = new Mock<ICurrentUserProvider>();
        currentUserMock.SetupGet(x => x.UserId).Returns(Guid.NewGuid());

        var loggerMock = new Mock<ILogger<CreateProductCommand>>();

        var handler = new CreateProductCommandHandler(context, currentUserMock.Object, loggerMock.Object);

        var command = new CreateProductCommand(
            "New Product",
            "Description",
            10m,
            "BAD"); // invalid SKU length

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
        result.Error.Message.Should().Be("SKU must be exactly 8 characters.");

        context.Products.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateProductCommandHandler_ShouldReturnFailure_WhenProductCreationFails()
    {
        // Arrange
        using var context = CreateInMemoryCatalogContext();
        var currentUserMock = new Mock<ICurrentUserProvider>();
        currentUserMock.SetupGet(x => x.UserId).Returns(Guid.NewGuid());

        var loggerMock = new Mock<ILogger<CreateProductCommand>>();

        var handler = new CreateProductCommandHandler(context, currentUserMock.Object, loggerMock.Object);

        var command = new CreateProductCommand(
            "", // invalid name
            "Description",
            10m,
            "ABCD1234");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
        result.Error.Message.Should().Be("Product name is required.");

        context.Products.Should().BeEmpty();
    }

    [Fact]
    public async Task UploadProductImageCommandHandler_ShouldReturnFailure_WhenProductDoesNotExist()
    {
        // Arrange
        using var context = CreateInMemoryCatalogContext();

        var storageMock = new Mock<IFileStorageService>();
        var currentUserMock = new Mock<ICurrentUserProvider>();
        currentUserMock.SetupGet(x => x.UserId).Returns(Guid.NewGuid());

        var loggerMock = new Mock<ILogger<UploadProductImageCommand>>();

        var handler = new UploadProductImageCommandHandler(
            context,
            storageMock.Object,
            currentUserMock.Object,
            loggerMock.Object);

        using var stream = new MemoryStream(new byte[] { 1, 2, 3 });

        var command = new UploadProductImageCommand(
            Guid.NewGuid(), // non-existing product
            stream,
            "image.jpg");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
        result.Error.Message.Should().Be("Product not found.");

        storageMock.Verify(s => s.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UploadProductImageCommandHandler_ShouldUploadImageAndUpdateProduct_WhenProductExists()
    {
        // Arrange
        using var context = CreateInMemoryCatalogContext();

        var moneyResult = Money.Create(199.99m, "INR");
        var skuResult = Sku.Create("ABCD1234");
        var productResult = Product.Create("Test Product", "Description", moneyResult.Value, skuResult.Value);
        var product = productResult.Value;

        context.Products.Add(product);
        await context.SaveChangesAsync();

        var storageMock = new Mock<IFileStorageService>();
        storageMock
            .Setup(s => s.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("images/uploaded.jpg");

        var currentUserMock = new Mock<ICurrentUserProvider>();
        currentUserMock.SetupGet(x => x.UserId).Returns(Guid.NewGuid());

        var loggerMock = new Mock<ILogger<UploadProductImageCommand>>();

        var handler = new UploadProductImageCommandHandler(
            context,
            storageMock.Object,
            currentUserMock.Object,
            loggerMock.Object);

        using var stream = new MemoryStream(new byte[] { 1, 2, 3 });

        var command = new UploadProductImageCommand(
            product.Id,
            stream,
            "image.jpg");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("images/uploaded.jpg");

        var updatedProduct = await context.Products.FindAsync(product.Id);
        updatedProduct.Should().NotBeNull();
        updatedProduct!.ImagePath.Should().Be("images/uploaded.jpg");

        storageMock.Verify(s => s.UploadAsync(It.IsAny<Stream>(), "image.jpg", It.IsAny<CancellationToken>()), Times.Once);
    }
}

