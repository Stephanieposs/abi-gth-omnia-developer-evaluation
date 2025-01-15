using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services;
using AutoMapper;
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
    //public readonly IProductService _service;

    public ProductService(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task AddAsync(Product product)
    {
        await _repo.UpdateRating(product.Rating.Rate);
        await _repo.AddProduct(product);
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

    public async Task<Product> GetByIdAsync(int id)
    {
        return await _repo.GetProductById(id);
    }

    public async Task UpdateAsync(Product product)
    {
        var existingProduct = await _repo.GetProductById(product.Id);
        if (existingProduct == null)
        {
            throw new KeyNotFoundException($"Product with ID {product.Id} not found.");
        }

        existingProduct.Title = product.Title;
        existingProduct.Price = product.Price;
        existingProduct.Description = product.Description;
        existingProduct.Category = product.Category;
        existingProduct.Image = product.Image;
        existingProduct.Rating.Rate = product.Rating.Rate;
        
        _repo.UpdateRating(product.Rating.Rate);

        await _repo.UpdateProduct(existingProduct);
    }


    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category) // , int page, int size, string order
    {
        var products = await _repo.GetProductsByCategoryAsync(category);
        return products;

        //return JsonSerializer.Serialize(products);


        /*
        if (!string.IsNullOrEmpty(order))
        {
            var orderParams = order.Split(',');
            foreach (var param in orderParams)
            {
                var parts = param.Trim().Split(' ');
                var field = parts[0];
                var direction = parts.Length > 1 && parts[1].ToLower() == "desc" ? "Descending" : "Ascending";

                products = products.AsQueryable().OrderBy($"{field} {direction}").ToList();
            }
        }

        // Pagination
        var totalItems = products.Count();
        var totalPages = (int)Math.Ceiling(totalItems / (double)size);
        var pagedProducts = products.Skip((page - 1) * size).Take(size).ToList();
        
        return new
        {
            data = pagedProducts,
            totalItems,
            currentPage = page,
            totalPages
        };*/



    }

}
