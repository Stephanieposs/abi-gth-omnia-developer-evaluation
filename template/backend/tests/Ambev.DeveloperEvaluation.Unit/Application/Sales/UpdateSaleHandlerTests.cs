using Ambev.DeveloperEvaluation.Application.Carts.GetCart;
using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
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

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales
{
        public class UpdateSaleHandlerTests
        {
            private readonly Mock<ISaleRepository> _repoMock;
            private readonly Mock<IMapper> _mapperMock;
            private readonly Mock<ILogger<UpdateSaleHandler>> _loggerMock;
            private readonly Mock<IMediator> _mediatorMock;
            private readonly UpdateSaleHandler _handler;

            public UpdateSaleHandlerTests()
            {
                _repoMock = new Mock<ISaleRepository>();
                _mapperMock = new Mock<IMapper>();
                _loggerMock = new Mock<ILogger<UpdateSaleHandler>>();
                _mediatorMock = new Mock<IMediator>();
                _handler = new UpdateSaleHandler(_repoMock.Object, _mapperMock.Object, _loggerMock.Object, _mediatorMock.Object);
            }

            [Fact]
            public async Task Handle_ShouldReturnNull_WhenSaleNotFound()
            {
                // Arrange
                var command = new UpdateSaleCommand { SaleNumber = 1 };
                _repoMock.Setup(r => r.GetSaleById(command.SaleNumber)).ReturnsAsync((Sale)null);

                // Act
                var result = await _handler.Handle(command, CancellationToken.None);

                // Assert
                result.Should().BeNull();
                _repoMock.Verify(r => r.GetSaleById(command.SaleNumber), Times.Once);
                _repoMock.Verify(r => r.UpdateSale(It.IsAny<Sale>()), Times.Never);
            }

            [Fact]
            public async Task Handle_ShouldUpdateBasicProperties_WhenCartIdRemainsSame()
            {
                // Arrange
                var existingSale = new Sale
                {
                    SaleNumber = 1,
                    CartId = 2,
                    Items = new List<SaleItem>(),
                    Date = DateTime.Now.AddDays(-1),
                    BranchName = "Old Branch",
                    BranchId = 1,
                    CustomerName = "Old Customer"
                };

                var command = new UpdateSaleCommand
                {
                    SaleNumber = 1,
                    CartId = 2,
                    Date = DateTime.Now,
                    BranchName = "New Branch",
                    BranchId = 2,
                    CustomerName = "New Customer"
                };

                _repoMock.Setup(r => r.GetSaleById(command.SaleNumber)).ReturnsAsync(existingSale);
                _repoMock.Setup(r => r.UpdateSale(It.IsAny<Sale>())).ReturnsAsync(existingSale);
                _mapperMock.Setup(m => m.Map<Sale>(command)).Returns(existingSale);

                // Act
                var result = await _handler.Handle(command, CancellationToken.None);

                // Assert
                result.Should().NotBeNull();
                result.BranchName.Should().Be(command.BranchName);
                result.BranchId.Should().Be(command.BranchId);
                result.CustomerName.Should().Be(command.CustomerName);
                result.Date.Should().Be(command.Date);
                _repoMock.Verify(r => r.UpdateSale(It.IsAny<Sale>()), Times.Once);
            }

            [Fact]
            public async Task Handle_ShouldReturnNull_WhenNewCartIsInvalid()
            {
                // Arrange
                var existingSale = new Sale
                {
                    SaleNumber = 1,
                    CartId = 2,
                    Items = new List<SaleItem>()
                };

                var command = new UpdateSaleCommand
                {
                    SaleNumber = 1,
                    CartId = 3  // Different CartId
                };

                _repoMock.Setup(r => r.GetSaleById(command.SaleNumber)).ReturnsAsync(existingSale);
                _mediatorMock.Setup(m => m.Send(It.IsAny<GetCartQuery>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((Cart)null);

                // Act
                var result = await _handler.Handle(command, CancellationToken.None);

                // Assert
                result.Should().BeNull();
                _repoMock.Verify(r => r.UpdateSale(It.IsAny<Sale>()), Times.Never);
            }

            [Fact]
            public async Task Handle_ShouldUpdateSaleWithNewCart_WhenCartIdChanges()
            {
                // Arrange
                var product = new Product { Id = 10, Title = "Product A", Price = 100 };
                var newCart = new Cart
                {
                    Id = 3,
                    UserId = 1,
                    CartProductsList = new List<CartProduct>
            {
                new CartProduct { ProductId = 10, Product = product, Quantity = 2 }
            }
                };

                var existingSale = new Sale
                {
                    SaleNumber = 1,
                    CartId = 2,
                    Items = new List<SaleItem>(),
                    TotalAmount = 0
                };

                var command = new UpdateSaleCommand
                {
                    SaleNumber = 1,
                    CartId = 3,
                    Date = DateTime.Now,
                    BranchName = "New Branch",
                    BranchId = 2,
                    CustomerName = "New Customer"
                };

                _repoMock.Setup(r => r.GetSaleById(command.SaleNumber)).ReturnsAsync(existingSale);
                _mediatorMock.Setup(m => m.Send(It.IsAny<GetCartQuery>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(newCart);
                _mediatorMock.Setup(m => m.Send(It.IsAny<GetProductCommand>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(product);
                _mapperMock.Setup(m => m.Map<Cart>(It.IsAny<Cart>())).Returns(newCart);
                _repoMock.Setup(r => r.UpdateSale(It.IsAny<Sale>())).ReturnsAsync(existingSale);

                // Act
                var result = await _handler.Handle(command, CancellationToken.None);

                // Assert
                result.Should().NotBeNull();
                result.Items.Should().NotBeEmpty();
                result.CustomerId.Should().Be(newCart.UserId);
                result.BranchName.Should().Be(command.BranchName);
                result.BranchId.Should().Be(command.BranchId);
                result.CustomerName.Should().Be(command.CustomerName);
                result.Date.Should().Be(command.Date);
                _repoMock.Verify(r => r.UpdateSale(It.IsAny<Sale>()), Times.Once);
            }

            [Fact]
            public async Task Handle_ShouldReturnNull_WhenProductNotFound()
            {
                // Arrange
                var newCart = new Cart
                {
                    Id = 3,
                    UserId = 1,
                    CartProductsList = new List<CartProduct>
            {
                new CartProduct { ProductId = 10, Quantity = 2 }
            }
                };

                var existingSale = new Sale
                {
                    SaleNumber = 1,
                    CartId = 2,
                    Items = new List<SaleItem>()
                };

                var command = new UpdateSaleCommand
                {
                    SaleNumber = 1,
                    CartId = 3
                };

                _repoMock.Setup(r => r.GetSaleById(command.SaleNumber)).ReturnsAsync(existingSale);
                _mediatorMock.Setup(m => m.Send(It.IsAny<GetCartQuery>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(newCart);
                _mediatorMock.Setup(m => m.Send(It.IsAny<GetProductCommand>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((Product)null);

                // Act
                var result = await _handler.Handle(command, CancellationToken.None);

                // Assert
                result.Should().BeNull();
                _repoMock.Verify(r => r.UpdateSale(It.IsAny<Sale>()), Times.Never);
            }
        }
    }

