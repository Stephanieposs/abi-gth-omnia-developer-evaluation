using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static System.Net.Mime.MediaTypeNames;

namespace Ambev.DeveloperEvaluation.Unit.Application.Products;

public class CreateProductHandlerTests
{
    private readonly Mock<IProductRepository> _repoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateProductHandler _handler;

    public CreateProductHandlerTests()
    {
        _repoMock = new Mock<IProductRepository>();
        _mapperMock = new Mock<IMapper>();

        _handler = new CreateProductHandler(_repoMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnCreateProductResult_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Title = "Test",
            Description = "Test",
            Category = "Test",
            Image = "Test",
            Price = 4,
            Rating = new Rating
            {
                Count = 1,
                Rate = 1
            }
        };

        var product = new Product { Id=1, Title = "Test", Description = "Test",Category = "Test", Image = "Test", Price = 4,Rating = new Rating{Count = 1, Rate = 1} };
        var expectedResult = new CreateProductResult { Id = 1, Title = "Test", Description = "Test",Category = "Test", Image = "Test", Price = 4,Rating = new Rating{Count = 1, Rate = 1}  };

        _mapperMock.Setup(m => m.Map<Product>(command)).Returns(product);
        _repoMock.Setup(r => r.AddProduct(It.IsAny<Product>())).ReturnsAsync(product);
        _mapperMock.Setup(m => m.Map<CreateProductResult>(product)).Returns(expectedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Title.Should().Be("Test");

        _repoMock.Verify(r => r.AddProduct(It.IsAny<Product>()), Times.Once);
        _mapperMock.Verify(m => m.Map<CreateProductResult>(product), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenValidationFails()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Title = "ab", // Invalid Title
            Price = -10.0m, // Invalid Price
            Category = null // Invalid Category
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _repoMock.Verify(r => r.AddProduct(It.IsAny<Product>()), Times.Never);
    }
}
