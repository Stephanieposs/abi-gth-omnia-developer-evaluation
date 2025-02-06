using Ambev.DeveloperEvaluation.Application.Products;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class ProductServiceTest
{
    private readonly Mock<IProductRepository> _mockRepository;
    private readonly ProductService _service;

    public ProductServiceTest()
    {
        _mockRepository = new Mock<IProductRepository>();
        //_service = new ProductService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnProduct_WhenProductExists()
    {
        // Arrange
        var productId = 1;
        var product = new Product { Id = productId, Title = "Test Product", Price = 10.00M };
        _mockRepository.Setup(m => m.GetProductById(productId)).ReturnsAsync(product);

        // Act
        var result = await _service.GetByIdAsync(productId);

        // Assert
        Assert.Equal(product, result);
        _mockRepository.Verify(m => m.GetProductById(productId), Times.Once);
    }

    [Fact]
    public async Task GetProducts_ShouldReturnProducts_WhenProductsExists()
    {
        // Arrange
        var product1 = new Product { Id = 1, Title = "Test Product", Price = 10.00M };
        var product2 = new Product { Id = 2, Title = "Test Product1", Price = 11.00M };
        var expectedProducts = new List<Product> { product1, product2 };

        _mockRepository.Setup(m => m.GetAllProducts()).ReturnsAsync(expectedProducts); // Mock GetAllProductsAsync

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Equal(expectedProducts, result);
        _mockRepository.Verify(m => m.GetAllProducts(), Times.Once); // Verify the correct method was called
    }

    [Fact]
    public async Task PostProduct_ShouldReturnProduct_WhenProductAdded()
    {
        // Arrange
        var product = new Product { Title = "Test Product", Price = 10.00M };
        var expectedProduct = new Product { Id = 1, Title = "Test Product", Price = 10.00M };

        _mockRepository.Setup(m => m.AddProduct(It.IsAny<Product>())).ReturnsAsync(expectedProduct);

        // Act
        //var result = await _service.AddAsync(product);

        _service.AddAsync(product);

        // Assert
        _mockRepository.Verify(repo => repo.AddProduct(product), Times.Once);
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturnNoProduct_WhenOneProductExist()
    {
        // Arrange
        var product = new Product { Id = 1, Title = "Test Product", Price = 10.00M };
        _mockRepository.Setup(m => m.DeleteProduct(product.Id)).ReturnsAsync(product);

        // Act
        _service.DeleteAsync(product.Id);

        // Assert
        _mockRepository.Verify(m => m.DeleteProduct(product.Id), Times.Once);
    }

    [Fact]
    public async Task GetProductsCategories_ShouldReturnProductsCategories()
    {
        // Arrange
        var product1 = new Product { Id = 1, Title = "Test Product", Price = 10.00M, Category = "Products" };
        var product2 = new Product { Id = 2, Title = "Test Product1", Price = 11.00M, Category = "Teste" };
        var expectedProductsCategories = new List<string> { "Products","Teste" };

        _mockRepository.Setup(m => m.GetAllProductsCategories()).ReturnsAsync(expectedProductsCategories); // Mock GetAllProductsAsync

        // Act
        var result = await _service.GetAllProductCategoriesAsync();

        // Assert
        Assert.Equal(expectedProductsCategories, result);
        _mockRepository.Verify(m => m.GetAllProductsCategories(), Times.Once); // Verify the correct method was called
    }

}
