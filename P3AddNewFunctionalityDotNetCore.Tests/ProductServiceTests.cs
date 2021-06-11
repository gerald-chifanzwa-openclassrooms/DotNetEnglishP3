using System.Collections.Generic;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Localization;
using Moq;
using P3AddNewFunctionalityDotNetCore.Models;
using P3AddNewFunctionalityDotNetCore.Models.Entities;
using P3AddNewFunctionalityDotNetCore.Models.Repositories;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using Xunit;

namespace P3AddNewFunctionalityDotNetCore.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<ICart> _cartMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IStringLocalizer<ProductService>> _localizerMock;

        public ProductServiceTests()
        {
            _cartMock = new Mock<ICart>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _localizerMock = new Mock<IStringLocalizer<ProductService>>(MockBehavior.Loose);

            _localizerMock.Setup(localizer => localizer[It.IsAny<string>()])
                          .Returns((string name) => new LocalizedString(name, name, true));
        }

        private ProductService GetProductsService()
        {
            var cartMock = _cartMock.Object;
            var productRepositoryMock = _productRepositoryMock.Object;
            var orderRepositoryMock = _orderRepositoryMock.Object;
            var localizerMock = _localizerMock.Object;

            return new ProductService(cartMock, productRepositoryMock, orderRepositoryMock, localizerMock);
        }

        [Fact]
        public void CheckProductErrors_WhenNameIsEmpty_ShouldHaveMissingNameError()
        {
            // Arrange
            var faker = new Faker<ProductViewModel>()
                       .RuleFor(x => x.Name, string.Empty)
                       .RuleFor(x => x.Description, f => f.Commerce.ProductDescription())
                       .RuleFor(x => x.Details, f => f.Commerce.ProductName())
                       .RuleFor(x => x.Price, f => f.Commerce.Price())
                       .RuleFor(x => x.Stock, f => f.Random.Int(1, 200).ToString())
                       .RuleFor(x => x.Id, 0);

            var productViewModel = faker.Generate();
            var testSubject = GetProductsService();

            // Act
            var errors = testSubject.CheckProductModelErrors(productViewModel);

            // Assert
            errors.Should().NotBeEmpty();
            errors.Should().Contain("MissingName");
        }

        [Fact]
        public void CheckProductErrors_WhenPriceIsEmpty_ShouldHaveMissingPriceError()
        {
            // Arrange
            var faker = new Faker<ProductViewModel>()
                       .RuleFor(x => x.Name, f => f.Commerce.ProductName())
                       .RuleFor(x => x.Description, f => f.Commerce.ProductDescription())
                       .RuleFor(x => x.Details, f => f.Commerce.ProductMaterial())
                       .RuleFor(x => x.Price, string.Empty)
                       .RuleFor(x => x.Stock, f => f.Random.Int(1, 200).ToString())
                       .RuleFor(x => x.Id, 0);

            var productViewModel = faker.Generate();
            var testSubject = GetProductsService();

            // Act
            var errors = testSubject.CheckProductModelErrors(productViewModel);

            // Assert
            errors.Should().NotBeEmpty();
            errors.Should().Contain("MissingPrice");
        }

        [Fact]
        public void CheckProductErrors_WhenPriceIsNotANumber_ShouldHavePriceNotANumberError()
        {
            // Arrange
            var faker = new Faker<ProductViewModel>()
                       .RuleFor(x => x.Name, f => f.Commerce.ProductName())
                       .RuleFor(x => x.Description, f => f.Commerce.ProductDescription())
                       .RuleFor(x => x.Details, f => f.Commerce.ProductMaterial())
                       .RuleFor(x => x.Price, f => f.Random.String2(1, 5))
                       .RuleFor(x => x.Stock, f => f.Random.Int(1, 200).ToString())
                       .RuleFor(x => x.Id, 0);

            var productViewModel = faker.Generate();
            var testSubject = GetProductsService();

            // Act
            var errors = testSubject.CheckProductModelErrors(productViewModel);

            // Assert
            errors.Should().NotBeEmpty();
            errors.Should().Contain("PriceNotANumber");
        }

        [Fact]
        public void CheckProductErrors_WhenPriceIsNotGreaterThanZero_ShouldHavePriceNotGreaterThanZeroError()
        {
            // Arrange
            var faker = new Faker<ProductViewModel>()
                       .RuleFor(x => x.Name, f => f.Commerce.ProductName())
                       .RuleFor(x => x.Description, f => f.Commerce.ProductDescription())
                       .RuleFor(x => x.Details, f => f.Commerce.ProductMaterial())
                       .RuleFor(x => x.Price, "0")
                       .RuleFor(x => x.Stock, f => f.Random.Int(1, 200).ToString())
                       .RuleFor(x => x.Id, 0);

            var productViewModel = faker.Generate();
            var testSubject = GetProductsService();

            // Act
            var errors = testSubject.CheckProductModelErrors(productViewModel);

            // Assert
            errors.Should().NotBeEmpty();
            errors.Should().Contain("PriceNotGreaterThanZero");
        }

        [Fact]
        public void CheckProductErrors_WhenStockIsEmpty_ShouldHaveMissingQuantityError()
        {
            // Arrange
            var faker = new Faker<ProductViewModel>()
                       .RuleFor(x => x.Name, f => f.Commerce.ProductName())
                       .RuleFor(x => x.Description, f => f.Commerce.ProductDescription())
                       .RuleFor(x => x.Details, f => f.Commerce.ProductMaterial())
                       .RuleFor(x => x.Price, f => f.Commerce.Price())
                       .RuleFor(x => x.Stock, string.Empty)
                       .RuleFor(x => x.Id, 0);

            var productViewModel = faker.Generate();
            var testSubject = GetProductsService();

            // Act
            var errors = testSubject.CheckProductModelErrors(productViewModel);

            // Assert
            errors.Should().NotBeEmpty();
            errors.Should().Contain("MissingStock");
        }

        [Fact]
        public void CheckProductErrors_WhenStockIsNotANumber_ShouldHaveStockNotAnIntegerError()
        {
            // Arrange
            var faker = new Faker<ProductViewModel>()
                       .RuleFor(x => x.Name, f => f.Commerce.ProductName())
                       .RuleFor(x => x.Description, f => f.Commerce.ProductDescription())
                       .RuleFor(x => x.Details, f => f.Commerce.ProductMaterial())
                       .RuleFor(x => x.Price, f => f.Commerce.Price())
                       .RuleFor(x => x.Stock, f => f.Random.String2(1, 5))
                       .RuleFor(x => x.Id, 0);

            var productViewModel = faker.Generate();
            var testSubject = GetProductsService();

            // Act
            var errors = testSubject.CheckProductModelErrors(productViewModel);

            // Assert
            errors.Should().NotBeEmpty();
            errors.Should().Contain("StockNotAnInteger");
        }

        [Fact]
        public void CheckProductErrors_WhenStockIsNotGreaterThanZero_ShouldHaveStockNotGreaterThanZeroError()
        {
            // Arrange
            var faker = new Faker<ProductViewModel>()
                       .RuleFor(x => x.Name, f => f.Commerce.ProductName())
                       .RuleFor(x => x.Description, f => f.Commerce.ProductDescription())
                       .RuleFor(x => x.Details, f => f.Commerce.ProductMaterial())
                       .RuleFor(x => x.Price, f => f.Commerce.Price())
                       .RuleFor(x => x.Stock, "0")
                       .RuleFor(x => x.Id, 0);

            var productViewModel = faker.Generate();
            var testSubject = GetProductsService();

            // Act
            var errors = testSubject.CheckProductModelErrors(productViewModel);

            // Assert
            errors.Should().NotBeEmpty();
            errors.Should().Contain("StockNotGreaterThanZero");
        }

        [Fact]
        public void CreateProduct_WhenValidProductIsPassed_ShouldSaveProduct()
        {
            // Arrange
            var faker = new Faker<ProductViewModel>()
                       .RuleFor(x => x.Name, f => f.Commerce.ProductName())
                       .RuleFor(x => x.Description, f => f.Commerce.ProductDescription())
                       .RuleFor(x => x.Details, f => f.Commerce.ProductMaterial())
                       .RuleFor(x => x.Price, f => f.Commerce.Price())
                       .RuleFor(x => x.Stock, f => f.Random.Number(10, 50).ToString())
                       .RuleFor(x => x.Id, 0);

            var productViewModel = faker.Generate();
            var fakeProducts = new List<Product>();
            _productRepositoryMock.Setup(repository => repository.SaveProduct(It.IsNotNull<Product>()))
                                  .Callback<Product>((product) => { fakeProducts.Add(product); });
            _productRepositoryMock.Setup(repository => repository.GetAllProducts())
                                  .Returns(fakeProducts);
            var testSubject = GetProductsService();

            // Act
            testSubject.SaveProduct(productViewModel);
            var savedProducts = testSubject.GetAllProducts();

            // Assert
            savedProducts.Should().HaveCount(1);
            savedProducts.Should().ContainSingle(p => p.Name == productViewModel.Name);
        }
    }
}