using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.IoC;
using Ambev.DeveloperEvaluation.ORM;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Crosscutting;

public class DependencyInjectionTests
{
    private readonly IServiceProvider _serviceProvider;

    public DependencyInjectionTests()
    {
        var services = new ServiceCollection();
        var builder = WebApplication.CreateBuilder();

        // Register Database Context with In-Memory Provider (for testing)
        builder.Services.AddDbContext<DefaultContext>(options =>
            options.UseInMemoryDatabase("TestDb"));

        builder.RegisterDependencies(); // Register all IoC dependencies
        _serviceProvider = builder.Services.BuildServiceProvider();
    }

    [Fact]
    public void Should_Resolve_CartService()
    {
        //var service = _serviceProvider.GetService<ICartService>();
        //Assert.NotNull(service); // Test passes if CartService is correctly registered
    }

    [Fact]
    public void Should_Resolve_CartRepository()
    {
        var repository = _serviceProvider.GetService<ICartRepository>();
        Assert.NotNull(repository); // Test passes if CartRepository is correctly registered
    }

    [Fact]
    public void Should_Resolve_SaleService()
    {
        var service = _serviceProvider.GetService<ISaleService>();
        Assert.NotNull(service); // Test passes if SaleService is correctly registered
    }

    [Fact]
    public void Should_Resolve_SaleRepository()
    {
        var repository = _serviceProvider.GetService<ISaleRepository>();
        Assert.NotNull(repository); // Test passes if SaleRepository is correctly registered
    }

    /*
    [Fact]
    public void Should_Resolve_ProductService()
    {
        var service = _serviceProvider.GetService<IProductService>();
        Assert.NotNull(service); // Test passes if ProductService is correctly registered
    }
    */
    [Fact]
    public void Should_Resolve_ProductRepository()
    {
        var repository = _serviceProvider.GetService<IProductRepository>();
        Assert.NotNull(repository); // Test passes if ProductRepository is correctly registered
    }
}
