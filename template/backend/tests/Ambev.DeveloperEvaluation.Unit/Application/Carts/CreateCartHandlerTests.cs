using Ambev.DeveloperEvaluation.Application.Carts.CreateCart;
using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OneOf.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Carts;

public class CreateCartHandlerTests
{
    private readonly Mock<ICartRepository> _repoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateCartHandler _handler;
    private readonly Mock<ILogger<CreateCartHandler>> _loggerMock;

    public CreateCartHandlerTests()
    {
        _repoMock = new Mock<ICartRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<CreateCartHandler>>();
        _handler = new CreateCartHandler(_repoMock.Object, _mapperMock.Object, _loggerMock.Object);

    }

    [Fact]
    public async Task Handle_ShouldReturnCreateCartResult_WhenCommandIsValid()
    {
        // Arrange
        var products = new List<CreateCartProductResult>
            {
                new CreateCartProductResult { ProductId = 1, Quantity =1 },
                new CreateCartProductResult { ProductId=2, Quantity = 2 }
             };
        var command = new CreateCartCommand
        {
            CartProductsList = products,
            Date = DateTime.Now,
            UserId = 1
        };

        var cart = new Cart { Date = DateTime.Now, UserId = 1 };
        var expectedResult = new CreateCartResult { Date = DateTime.Now, UserId = 1 };

        _mapperMock.Setup(m => m.Map<Cart>(command)).Returns(cart);
        _repoMock.Setup(r => r.AddCartAsync(It.IsAny<Cart>())).ReturnsAsync(cart);
        _mapperMock.Setup(m => m.Map<CreateCartResult>(cart)).Returns(expectedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(1);

        _repoMock.Verify(r => r.AddCartAsync(It.IsAny<Cart>()), Times.Once);
        _mapperMock.Verify(m => m.Map<CreateCartResult>(cart), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenValidationFails()
    {
        // Arrange
        var products = new List<CreateCartProductResult>
            {
                new CreateCartProductResult { ProductId = 1, Quantity =1 },
                new CreateCartProductResult { ProductId=2, Quantity = 2 }
             };

        var command = new CreateCartCommand
        {
            CartProductsList = products,
            UserId = -5
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _repoMock.Verify(r => r.AddCartAsync(It.IsAny<Cart>()), Times.Never);
    }
}
