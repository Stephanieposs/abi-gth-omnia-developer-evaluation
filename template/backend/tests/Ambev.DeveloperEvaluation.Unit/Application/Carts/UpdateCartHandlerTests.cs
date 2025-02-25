using Ambev.DeveloperEvaluation.Application.Carts.CreateCart;
using Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;
using Ambev.DeveloperEvaluation.Application.Products.UpdateProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Ambev.DeveloperEvaluation.Unit.Application.Carts;

public class UpdateCartHandlerTests
{

    private readonly Mock<ICartRepository> _repoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UpdateCartHandler _handler;
    private readonly Mock<ILogger<UpdateCartHandler>> _loggerMock;

    public UpdateCartHandlerTests()
    {
        _repoMock = new Mock<ICartRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<UpdateCartHandler>>();
        _handler = new UpdateCartHandler(_repoMock.Object, _mapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnUpdateCartResult_WhenCommandIsValid()
    {
        // Arrange
        var cartId = 1;
        var userId = 1;
        var date = DateTime.Now;

        var products = new List<UpdateCartProductResult>
        {
            new UpdateCartProductResult { ProductId = 1, Quantity = 1 },
            new UpdateCartProductResult { ProductId = 2, Quantity = 2 }
        };

        var command = new UpdateCartCommand(cartId, userId, date, products);

        var existingCartProducts = new List<CartProduct>
        {
            new CartProduct { ProductId = 1, Quantity = 2, CartId = cartId },
            new CartProduct { ProductId = 3, Quantity = 1, CartId = cartId }
        };

        var existingCart = new Cart
        {
            Id = cartId,
            UserId = userId,
            Date = DateTime.Now.AddDays(-1),
            CartProductsList = existingCartProducts
        };

        _repoMock.Setup(r => r.GetCartByIdAsync(cartId))
            .ReturnsAsync(existingCart);

        _repoMock.Setup(r => r.UpdateCartAsync(It.IsAny<Cart>()))
            .ReturnsAsync(existingCart);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(cartId);
        result.UserId.Should().Be(userId);
        result.Date.Should().Be(date);

        result.Products.Should().HaveCount(2);
        result.Products.Should().Contain(p => p.ProductId == 1 && p.Quantity == 1);
        result.Products.Should().Contain(p => p.ProductId == 2 && p.Quantity == 2);

        _repoMock.Verify(r => r.GetCartByIdAsync(cartId), Times.Once);
        _repoMock.Verify(r => r.UpdateCartAsync(It.Is<Cart>(c =>
            c.Id == cartId &&
            c.UserId == userId &&
            c.Date == date)),
            Times.Once);
    }


    [Fact]
    public async Task Handle_ShouldReturnNull_WhenValidationFails()
    {
        // Arrange
        var products = new List<UpdateCartProductResult>
            {
                new UpdateCartProductResult { ProductId = 1, Quantity =1 },
                new UpdateCartProductResult { ProductId=2, Quantity = 2 }
             };

        var command = new UpdateCartCommand(1, 1, DateTime.Now, products);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _repoMock.Verify(r => r.UpdateCartAsync(It.IsAny<Cart>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenCartNotFound()
    {
        // Arrange
        var products = new List<UpdateCartProductResult>
        {
            new UpdateCartProductResult { ProductId = 1, Quantity = 1 }
        };

        var command = new UpdateCartCommand(1, 1, DateTime.Now, products);

        _repoMock.Setup(r => r.GetCartByIdAsync(command.CartId))
            .ReturnsAsync((Cart)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _repoMock.Verify(r => r.GetCartByIdAsync(command.CartId), Times.Once);
        _repoMock.Verify(r => r.UpdateCartAsync(It.IsAny<Cart>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldUpdateExistingProducts_WhenProductsAlreadyExist()
    {
        // Arrange
        var cartId = 1;
        var products = new List<UpdateCartProductResult>
        {
            new UpdateCartProductResult { ProductId = 1, Quantity = 5 }  // Updated quantity
        };

        var command = new UpdateCartCommand(cartId, 1, DateTime.Now, products);

        var existingCart = new Cart
        {
            Id = cartId,
            CartProductsList = new List<CartProduct>
            {
                new CartProduct { ProductId = 1, Quantity = 1, CartId = cartId }
            }
        };

        _repoMock.Setup(r => r.GetCartByIdAsync(cartId))
            .ReturnsAsync(existingCart);

        _repoMock.Setup(r => r.UpdateCartAsync(It.IsAny<Cart>()))
            .ReturnsAsync(existingCart);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Products.Should().HaveCount(1);
        result.Products.Should().Contain(p => p.ProductId == 1 && p.Quantity == 5);
    }

    [Fact]
    public async Task Handle_ShouldRemoveProducts_WhenProductsNotInUpdateList()
    {
        // Arrange
        var cartId = 1;
        var products = new List<UpdateCartProductResult>
        {
            new UpdateCartProductResult { ProductId = 1, Quantity = 1 }
        };

        var command = new UpdateCartCommand(cartId, 1, DateTime.Now, products);

        var existingCart = new Cart
        {
            Id = cartId,
            CartProductsList = new List<CartProduct>
            {
                new CartProduct { ProductId = 1, Quantity = 1, CartId = cartId },
                new CartProduct { ProductId = 2, Quantity = 2, CartId = cartId }  // Should be removed
            }
        };

        _repoMock.Setup(r => r.GetCartByIdAsync(cartId))
            .ReturnsAsync(existingCart);

        _repoMock.Setup(r => r.UpdateCartAsync(It.IsAny<Cart>()))
            .ReturnsAsync(existingCart);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Products.Should().HaveCount(1);
        result.Products.Should().Contain(p => p.ProductId == 1);
        result.Products.Should().NotContain(p => p.ProductId == 2);
    }









}
