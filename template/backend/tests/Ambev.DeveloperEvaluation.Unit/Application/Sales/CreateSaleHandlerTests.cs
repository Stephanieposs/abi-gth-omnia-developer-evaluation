using Ambev.DeveloperEvaluation.Application.Carts.CreateCart;
using Ambev.DeveloperEvaluation.Application.Carts.GetCart;
using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CreateSaleHandlerTests
{
    private readonly Mock<ISaleRepository> _repoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateSaleHandler _handler;
    private readonly Mock<ILogger<CreateSaleHandler>> _loggerMock;
    private readonly Mock<IMediator> _mediatorMock;

    public CreateSaleHandlerTests()
    {
        _repoMock = new Mock<ISaleRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<CreateSaleHandler>>();
        _mediatorMock = new Mock<IMediator>();
        _handler = new CreateSaleHandler(_repoMock.Object, _mapperMock.Object, _loggerMock.Object, _mediatorMock.Object);

    }

    [Fact]
    public async Task Handle_ShouldReturnCreateSaleResult_WhenCommandIsValid()
    {
        // Arrange
        var product = new Product { Id = 10, Title = "Product A", Price = 100, Category= "Category", Description= "Descreption" };

        var cart = new Cart
        {
            Id = 2,
            UserId = 1,
            CartProductsList = new List<CartProduct>
            {
                new CartProduct { ProductId = 10, Product = product, Quantity = 2 },
                new CartProduct { ProductId = 10, Product = product, Quantity = 4 }
            },
            Date = DateTime.Now
        };

        var command = new CreateSaleCommand
        {
            SaleNumber = 1,
            Date = DateTime.Now,
            CartId = 2,
            Items = new List<SaleItem>()
        };

        var sale = new Sale { Date = DateTime.Now, CartId = 2,
            Items = new List<SaleItem>(),
            SaleNumber = 1
        };

        _mapperMock.Setup(m => m.Map<Sale>(command)).Returns(sale);
        _mapperMock.Setup(m => m.Map<Cart>(It.IsAny<Cart>())).Returns(cart);
        _repoMock.Setup(r => r.CreateSale(It.IsAny<Sale>())).ReturnsAsync(sale);
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetCartQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
       
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.CartId.Should().Be(2);

        _repoMock.Verify(r => r.CreateSale(It.IsAny<Sale>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenValidationFails()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            SaleNumber = 1,
            CartId = 2
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetCartQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cart)null); // Mocking invalid cart

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _repoMock.Verify(r => r.CreateSale(It.IsAny<Sale>()), Times.Never);
    }
    [Fact]
    public async Task Handle_ShouldReturnNull_WhenCartIsEmpty()
    {
        // Arrange
        var command = new CreateSaleCommand
        {
            SaleNumber = 1,
            CartId = 2
        };

        var cart = new Cart
        {
            UserId = 1,
            CartProductsList = new List<CartProduct>() // Empty cart
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetCartQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cart);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _repoMock.Verify(r => r.CreateSale(It.IsAny<Sale>()), Times.Never);
    }
}
