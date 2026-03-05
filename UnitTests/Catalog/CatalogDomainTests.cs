using FluentAssertions;
using SwiftScale.BuildingBlocks;
using SwiftScale.Modules.Catalog.Domain;

namespace UnitTests.Catalog;

public class CatalogDomainTests
{
    [Fact]
    public void Money_Create_ShouldReturnSuccess_WhenAmountIsNonNegative()
    {
        // Act
        var result = Money.Create(100m, "INR");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Amount.Should().Be(100m);
        result.Value.Currency.Should().Be("INR");
    }

    [Fact]
    public void Money_Create_ShouldReturnFailure_WhenAmountIsNegative()
    {
        // Act
        var result = Money.Create(-1m, "INR");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
        result.Error.Message.Should().Be("Amount cannot be negative.");
    }

    [Fact]
    public void Sku_Create_ShouldReturnSuccess_WhenValueIsExactlyEightCharacters()
    {
        // Arrange
        var value = "ABCDEFGH";

        // Act
        var result = Sku.Create(value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData("SHORT")]
    [InlineData("TOO-LONG-STRING")]
    public void Sku_Create_ShouldReturnFailure_WhenValueIsInvalid(string value)
    {
        // Act
        var result = Sku.Create(value);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be("SKU must be exactly 8 characters.");
    }

    [Fact]
    public void Product_Create_ShouldReturnSuccess_AndRaiseDomainEvent_WhenInputIsValid()
    {
        // Arrange
        var moneyResult = Money.Create(199.99m, "INR");
        var skuResult = Sku.Create("ABCD1234");

        // Act
        var productResult = Product.Create("Test Product", "Description", moneyResult.Value, skuResult.Value);

        // Assert
        productResult.IsSuccess.Should().BeTrue();
        var product = productResult.Value;

        product.Id.Should().NotBeEmpty();
        product.Name.Should().Be("Test Product");
        product.Description.Should().Be("Description");
        product.Price.Should().Be(moneyResult.Value);
        product.Sku.Should().Be(skuResult.Value);

        product.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ProductCreatedDomainEvent>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Product_Create_ShouldReturnFailure_WhenNameIsInvalid(string name)
    {
        // Arrange
        var moneyResult = Money.Create(199.99m, "INR");
        var skuResult = Sku.Create("ABCD1234");

        // Act
        var productResult = Product.Create(name, "Description", moneyResult.Value, skuResult.Value);

        // Assert
        productResult.IsFailure.Should().BeTrue();
        productResult.Error.Should().NotBeNull();
        productResult.Error.Message.Should().Be("Product name is required.");
    }

    [Fact]
    public void Product_Create_ShouldReturnFailure_WhenSkuIsNull()
    {
        // Arrange
        var moneyResult = Money.Create(199.99m, "INR");

        // Act
        var productResult = Product.Create("Test Product", "Description", moneyResult.Value, null!);

        // Assert
        productResult.IsFailure.Should().BeTrue();
        productResult.Error.Message.Should().Be("SKU is required.");
    }

    [Fact]
    public void Product_UpdateImagePath_ShouldSetImagePath()
    {
        // Arrange
        var moneyResult = Money.Create(199.99m, "INR");
        var skuResult = Sku.Create("ABCD1234");
        var productResult = Product.Create("Test Product", "Description", moneyResult.Value, skuResult.Value);
        var product = productResult.Value;

        // Act
        product.UpdateImagePath("images/test.jpg");

        // Assert
        product.ImagePath.Should().Be("images/test.jpg");
    }
}

