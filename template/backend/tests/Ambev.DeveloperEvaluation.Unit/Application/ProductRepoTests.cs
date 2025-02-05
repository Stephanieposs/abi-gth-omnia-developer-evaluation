using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Ambev.DeveloperEvaluation.ORM;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class ProductRepoTests
{
    private readonly Mock<DefaultContext> _mockContext;
    private readonly IProductRepository _repository;

    public ProductRepoTests()
    {
        _mockContext = new Mock<DefaultContext>();
        _repository = new ProductRepository(_mockContext.Object);
    }

    /*
    [Fact]
    public async Task AddProduct_ShouldAddProductAndSaveChanges()
    {
        // Arrange
        var product = new Product { Title = "Test Product", Price = 10.00M };
        var mockDbSet = new Mock<DbSet<Product>>();
        _mockContext.Setup(m => m.Products).Returns(mockDbSet.Object);

        mockDbSet.Setup(m => m.Add(It.IsAny<Product>()))
            .Returns((Product product) => new EntityEntry<Product>(product));


        //mockDbSet.Setup(m => m.Add(It.IsAny<Product>())).Returns(Task.CompletedTask);

        // Act
        await _repository.AddProduct(product);

        // Assert
        mockDbSet.Verify(m => m.Add(product), Times.Once);
        _mockContext.Verify(m => m.SaveChanges(), Times.Once);
    }
    */


}
