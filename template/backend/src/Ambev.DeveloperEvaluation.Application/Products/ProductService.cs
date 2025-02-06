using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Products;

public class ProductService : IProductService
{
    public readonly IProductRepository _repo;
    public readonly ILogger<ProductService> _logger;
    public ProductService(IProductRepository repo, ILogger<ProductService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task AddAsync(Product product)
    {
        
        await _repo.AddProduct(product);
        await _repo.UpdateRating(product.Id, product.Rating.Rate);
    }

    public async Task DeleteAsync(int id)
    {
        var product = await GetByIdAsync(id);

         await _repo.DeleteProduct(id);
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _repo.GetAllProducts();
    }

    public async Task<IEnumerable<string>> GetAllProductCategoriesAsync()
    {
        var categories = await _repo.GetAllProductsCategories();
        return categories;
    }

    public async Task<(IEnumerable<Product> Products, int TotalItems)> GetPagedProductsAsync(int page, int size, string order)
    {
        return await _repo.GetPagedProductsAsync(page, size, order);
    }

    public async Task<(IEnumerable<Product> Products, int TotalItems)> GetPagedProductsByCategoryAsync(string category, int page, int size, string order)
    {
        return await _repo.GetPagedProductsByCategoryAsync(category, page, size, order);
    }

    public async Task<Product> GetByIdAsync(int id)
    {
        return await _repo.GetProductById(id);
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        var existingProduct = await _repo.GetProductById(product.Id);
        if (existingProduct == null)
        {
            _logger.LogWarning($"Product with ID {product.Id} not found.");
            //throw new KeyNotFoundException($"Product with ID {product.Id} not found.");
            return null;
        }

        existingProduct.Title = product.Title;
        existingProduct.Price = product.Price;
        existingProduct.Description = product.Description;
        existingProduct.Category = product.Category;
        existingProduct.Image = product.Image;
        existingProduct.Rating.Rate = product.Rating.Rate;

        return await _repo.UpdateProduct(existingProduct);
    }


    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category) 
    {
        var products = await _repo.GetProductsByCategoryAsync(category);
        return products;
    }

    public async Task<(IEnumerable<Product> Items, int TotalItems)> GetFilteredAndOrderedProductsAsync(
    int page, int size, string order, Dictionary<string, string> filters)
    {
        return await _repo.GetFilteredAndOrderedProductsAsync(page, size, order, filters);
    }

}
