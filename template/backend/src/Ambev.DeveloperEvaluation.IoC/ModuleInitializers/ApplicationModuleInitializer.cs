using Ambev.DeveloperEvaluation.Application.Carts;
using Ambev.DeveloperEvaluation.Application.Carts.CreateCart;
using Ambev.DeveloperEvaluation.Application.Carts.DeleteCart;
using Ambev.DeveloperEvaluation.Application.Carts.GetAllCarts;
using Ambev.DeveloperEvaluation.Application.Carts.GetCart;
using Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;
using Ambev.DeveloperEvaluation.Application.Products;
using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;
using Ambev.DeveloperEvaluation.Application.Products.DeleteProduct;
using Ambev.DeveloperEvaluation.Application.Products.GetAllCategories;
using Ambev.DeveloperEvaluation.Application.Products.GetAllProducts;
using Ambev.DeveloperEvaluation.Application.Products.GetByCategory;
using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.Application.Products.UpdateProduct;
using Ambev.DeveloperEvaluation.Application.Sales;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Services;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.IoC.ModuleInitializers;

public class ApplicationModuleInitializer : IModuleInitializer
{
    public void Initialize(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

        //builder.Services.AddScoped<ICartService, CartService>();
        builder.Services.AddScoped<ISaleService, SaleService>();
        //builder.Services.AddScoped<IProductService, ProductService>();

        // Register Products Handlers
        builder.Services.AddTransient<IRequestHandler<CreateProductCommand, CreateProductResult>, CreateProductHandler>();
        builder.Services.AddTransient<IRequestHandler<UpdateProductCommand, Product>, UpdateProductHandler>();
        builder.Services.AddTransient<IRequestHandler<DeleteProductCommand, bool>, DeleteProductHandler>();
        builder.Services.AddTransient<IRequestHandler<GetProductCommand, Product>, GetProductHandler>();
        builder.Services.AddTransient<IRequestHandler<GetAllProductsQuery, GetAllProductsResponse>, GetAllProductsHandler>();
        builder.Services.AddTransient<IRequestHandler<GetByCategoryCommand, IEnumerable<Product>>, GetByCategoryHandler>();
        builder.Services.AddTransient<IRequestHandler<GetAllCategoriesQuery, IEnumerable<string>>, GetAllCategoriesHandler>();

        // Register Carts Handlers
        builder.Services.AddTransient<IRequestHandler<CreateCartCommand, CreateCartResult>,CreateCartHandler>();
        builder.Services.AddTransient<IRequestHandler<DeleteCartCommand, bool>, DeleteCartHandler>();
        builder.Services.AddTransient<IRequestHandler<GetAllCartsQuery, GetAllCartsPagedResponse<GetAllCartsResponse>>, GetAllCartsHandler>();
        builder.Services.AddTransient<IRequestHandler<GetCartQuery, GetCartResponse>, GetCartHandler>();
        builder.Services.AddTransient<IRequestHandler<UpdateCartCommand, UpdateCartResult>, UpdateCartHandler>();
    }
}