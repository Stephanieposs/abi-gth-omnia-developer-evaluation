using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly DefaultContext _yourContext;

    public ProductRepository(DefaultContext yourContext)
    {
        _yourContext = yourContext;
    }

    public async Task<Product> AddProduct(Product product)
    {
        await _yourContext.Products.AddAsync(product);
        await _yourContext.SaveChangesAsync();
        return product;
    }

    public async Task<IEnumerable<Product>> GetAllProducts()
    {
        return await _yourContext.Products.Include(p => p.Rating).ToListAsync();
    }

    public async Task<IEnumerable<string>> GetAllProductsCategories()
    {
        return await _yourContext.Products
            .Select(p => p.Category)
            .Distinct()
            .ToListAsync();
    }

    public async Task<Product> GetProductById(int id)
    {
        return await _yourContext.Products
            .Include(p => p.Rating)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public Task<IEnumerable<Product>> GetProductsFromCategory(string category)
    {
        throw new NotImplementedException();
    }


    public async Task<Product> DeleteProduct(int id)
    {
        var product = await GetProductById(id);
        if (product != null)
        {
            _yourContext.Products.Remove(product);
            await _yourContext.SaveChangesAsync();
        }

        return product;
    }

    public async Task<Rating> UpdateRating(int productId, double newRate)
    {
        // Encontra o produto com a Rating associada
        var product = await _yourContext.Products
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product == null)
        {
            product.Rating = new Rating
            {
                Rate = newRate,
                Count = 1 // Inicializa o Count com 1
            };

            await _yourContext.SaveChangesAsync();
            return product.Rating;
        }

        // Tenta encontrar uma Rating associada ao novo valor
        var existingRating = product.Rating;
        
        if (existingRating != null && existingRating.Rate == newRate)
        {
            // Se a Rating já existir, incrementa a contagem
            existingRating.IncrementCount();
        }
        else
        {
            // Cria uma nova Rating associada ao produto
            existingRating = new Rating
            {
                Rate = newRate,
                Count = 1 // Inicializa o Count com 1
            };

            // Atualiza a Rating do produto
            product.Rating = existingRating;
        }

        // Salva as alterações no banco de dados
        await _yourContext.SaveChangesAsync();

        // Retorna a Rating atualizada ou recém-criada
        return existingRating;
    }
    public async Task<Product> UpdateProduct(Product product)
    {
        if (product != null)
        {
            _yourContext.Products.Update(product);
            await _yourContext.SaveChangesAsync();
        }


        return product;
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category)
    {
        return await _yourContext.Products
            .Include(p => p.Rating)
            .Where(p => p.Category.ToLower() == category.ToLower())
            .ToListAsync();
    }

    public async Task<(IEnumerable<Product> Products, int TotalItems)> GetPagedProductsAsync(int page, int size, string order)
    {
        var query = _yourContext.Products.AsQueryable();

        if (!string.IsNullOrEmpty(order))
        {
            foreach (var orderClause in order.Split(','))
            {
                var parts = orderClause.Trim().Split(' ');
                var property = parts[0];
                var direction = parts.Length > 1 && parts[1].ToLower() == "desc" ? "descending" : "ascending";
                query = query.OrderBy($"{property} {direction}");
            }
        }

        var totalItems = await query.CountAsync();
        var products = await query.Skip((page - 1) * size).Take(size).ToListAsync();
        return (products, totalItems);
    }

    public async Task<(IEnumerable<Product> Products, int TotalItems)> GetPagedProductsByCategoryAsync(string category, int page, int size, string order)
    {
        var query = _yourContext.Products.Where(p => p.Category == category);

        if (!string.IsNullOrEmpty(order))
        {
            foreach (var orderClause in order.Split(','))
            {
                var parts = orderClause.Trim().Split(' ');
                var property = parts[0];
                var direction = parts.Length > 1 && parts[1].ToLower() == "desc" ? "descending" : "ascending";
                query = query.OrderBy($"{property} {direction}");
            }
        }

        var totalItems = await query.CountAsync();
        var products = await query.Skip((page - 1) * size).Take(size).ToListAsync();
        return (products, totalItems);
    }

}
