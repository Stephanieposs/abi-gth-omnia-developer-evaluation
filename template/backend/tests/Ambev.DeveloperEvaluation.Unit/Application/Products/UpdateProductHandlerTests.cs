using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;
using Ambev.DeveloperEvaluation.Application.Products.UpdateProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.UpdateProduct;
using AutoMapper;
using Castle.Core.Logging;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Products;

public class UpdateProductHandlerTests
{
    private readonly Mock<IProductRepository> _repoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UpdateProductHandler _handler;
    private readonly Mock<ILogger<UpdateProductHandler>> _loggerMock;

    public UpdateProductHandlerTests()
    {
        _repoMock = new Mock<IProductRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<UpdateProductHandler>>();
        _handler = new UpdateProductHandler(_repoMock.Object, _mapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnUpdateProductResult_WhenCommandIsValid()
    {
        // Arrange
        var command = new UpdateProductCommand
        {
            Id = 1,
            Title = "Updated Test",
            Description = "Updated Test",
            Category = "Updated Test",
            Image = "Updated Test",
            Price = 8,
            Rating = new Rating { Count = 1, Rate = 1 }
        };

        var product = new Product { Id = 1, Title = "Test", Description = "Test", Category = "Test", Image = "Test", Price = 4, Rating = new Rating { Count = 1, Rate = 1 } };
        
        var updatedProduct = new Product // Product AFTER update (what we expect)
        {
            Id = 1,
            Title = command.Title, // Values from the command
            Description = command.Description,
            Category = command.Category,
            Image = command.Image,
            Price = command.Price,
            Rating = command.Rating
        };

        _mapperMock.Setup(m => m.Map<Product>(command)).Returns(product); // Map to the initial product

        _repoMock.Setup(r => r.GetProductById(command.Id)).ReturnsAsync(product);

        // *** KEY CHANGE: Setup UpdateProduct to return the UPDATED Product ***
        _repoMock.Setup(r => r.UpdateProduct(It.Is<Product>(p => p.Id == command.Id && p.Title == command.Title))) // Check updated properties
            .ReturnsAsync(updatedProduct); // Return the UPDATED product


        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull(); // Check for null before accessing properties
        result.Id.Should().Be(1);
        result.Title.Should().Be("Updated Test");

        _repoMock.Verify(r => r.GetProductById(command.Id), Times.Once); // Verify GetProductById was called
        _repoMock.Verify(r => r.UpdateProduct(It.Is<Product>(p => p.Id == command.Id)), Times.Once);

    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenValidationFails()
    {
        // Arrange
        var command = new UpdateProductCommand
        {
            Price = -10.0m, // Invalid Price
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _repoMock.Verify(r => r.UpdateProduct(It.IsAny<Product>()), Times.Never);
    }
}
