using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Localization;
using Moq;
using P3AddNewFunctionalityDotNetCore.Models;
using P3AddNewFunctionalityDotNetCore.Models.Entities;
using P3AddNewFunctionalityDotNetCore.Models.Repositories;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using Xunit;

namespace P3AddNewFunctionalityDotNetCore.Tests.ServiceTests
{
    public class ProductsServiceTests
    {
        [Fact]
        public void GetProducts_ShouldReturnMappedItemsFromRepository()
        {
            // Arrange
            var mockCart = new Mock<ICart>();
            var mockProductRepository = new Mock<IProductRepository>();
            var mockOrderRepository = new Mock<IOrderRepository>();
            var localizerMock = new Mock<IStringLocalizer<ProductService>>();
            var productService = new ProductService(mockCart.Object,
                                                    mockProductRepository.Object,
                                                    mockOrderRepository.Object,
                                                    localizerMock.Object);

            var productsFaker = new Faker<Product>()
                               .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
                               .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                               .RuleFor(p => p.Details, f => f.Commerce.Product())
                               .RuleFor(p => p.Id, f => f.IndexFaker)
                               .RuleFor(p => p.Price, f => f.Random.Number(1, 100))
                               .RuleFor(p => p.Quantity, f => f.Random.Number(10, 50));
            var products = productsFaker.Generate(20);
            mockProductRepository.Setup(r => r.GetAllProducts())
                                 .Returns(products);

            // Act
            var result = productService.GetAllProducts();

            // Assert
            result.Should().HaveCount(20);
        }

        [Fact]
        public void CreateProduct()
        {
           
        }
        
    }
}