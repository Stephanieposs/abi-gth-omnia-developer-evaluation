using Ambev.DeveloperEvaluation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllProducts();
    Task<Product> GetProductById(int id);
    Task<Product> AddProduct(Product product);
    Task<Product> UpdateProduct(Product product);
    Task<Product> DeleteProduct(int id);

    // return all categories
    Task<IEnumerable<string>> GetAllProductsCategories(); 

    // return all products from a determined category
    //Task<IEnumerable<Product>> GetProductsFromCategory(string category);

    Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category);

    Task<Rating> UpdateRating(int id,double newRate);

    Task<(IEnumerable<Product> Products, int TotalItems)> GetPagedProductsAsync(int page, int size, string order);
    Task<(IEnumerable<Product> Products, int TotalItems)> GetPagedProductsByCategoryAsync(string category, int page, int size, string order);

    Task<(IEnumerable<Product> Items, int TotalItems)> GetFilteredAndOrderedProductsAsync(
    int page, int size, string order, Dictionary<string, string> filters);


}
