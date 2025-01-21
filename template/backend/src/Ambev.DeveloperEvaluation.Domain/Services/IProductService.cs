using Ambev.DeveloperEvaluation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Domain.Services;

public interface IProductService
{
    Task<Product> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetAllAsync();
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(int id);
    Task<IEnumerable<string>> GetAllProductCategoriesAsync();

    Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category);
    Task<(IEnumerable<Product> Products, int TotalItems)> GetPagedProductsAsync(int page, int size, string order);
    Task<(IEnumerable<Product> Products, int TotalItems)> GetPagedProductsByCategoryAsync(string category, int page, int size, string order);
    Task<(IEnumerable<Product> Items, int TotalItems)> GetFilteredAndOrderedProductsAsync(
    int page, int size, string order, Dictionary<string, string> filters);


}
