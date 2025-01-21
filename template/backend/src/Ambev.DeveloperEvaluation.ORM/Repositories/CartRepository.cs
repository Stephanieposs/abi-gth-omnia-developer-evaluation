using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class CartRepository : ICartRepository
{
    private readonly DefaultContext _yourContext;
    public CartRepository(DefaultContext yourContext)
    {
        _yourContext = yourContext;
    }

    public async Task<Cart> AddCartAsync(Cart cart)
    {
        await ValidateProductReferences(cart.CartProductsList);
        foreach (var cartProduct in cart.CartProductsList)
        {
            //cartProduct.Id = cart.Id;
            _yourContext.CartProducts.Add(cartProduct);
        }

        await _yourContext.Carts.AddAsync(cart);
        await _yourContext.SaveChangesAsync();
        return cart;
    }

    public async Task<(IEnumerable<Cart> Carts, int TotalItems)> GetPagedCartsAsync(int page, int size, string order)
    {
        var query = _yourContext.Carts.AsQueryable();

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
        var carts = await query.Skip((page - 1) * size).Take(size).ToListAsync();
        return (carts, totalItems);
    }

    public async Task<(IEnumerable<Cart> Items, int TotalItems)> GetFilteredAndOrderedCartsAsync(
    int page, int size, string order, Dictionary<string, string> filters)
    {
        //var query = _yourContext.Carts.AsQueryable();
        var query = _yourContext.Carts
        .Include(c => c.CartProductsList) // Ensure related data is included
        .AsQueryable(); 


        // Apply Filters
        foreach (var filter in filters)
        {
            var field = filter.Key;
            var value = filter.Value;

            if (value.Contains("*"))
            {
                var pattern = value.Replace("*", ""); // Remove asterisk for partial matches
                query = query.Where(p => EF.Functions.Like(EF.Property<string>(p, field), $"%{pattern}%"));
            }
            else if (field.StartsWith("_min") || field.StartsWith("_max"))
            {
                var baseField = field.Replace("_min", "").Replace("_max", "");
                if (field.StartsWith("_min"))
                    query = query.Where(p => EF.Property<decimal>(p, baseField) >= decimal.Parse(value));
                else if (field.StartsWith("_max"))
                    query = query.Where(p => EF.Property<decimal>(p, baseField) <= decimal.Parse(value));
            }
            else
            {
                query = query.Where(p => EF.Property<string>(p, field) == value);
            }
        }

        // Apply Ordering
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
        else
        {
            query = query.OrderBy(p => p.Id); // Default order by ID
        }

        // Pagination
        var totalItems = await query.CountAsync();
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync();

        return (items, totalItems);
    }


    public async Task<Cart> DeleteCartAsync(int id)
    {
        var cart = await GetCartByIdAsync(id);
        if (cart != null)
        {
            _yourContext.Carts.Remove(cart);
            await _yourContext.SaveChangesAsync();
        }

        return cart;
    }

    public async Task<Cart> GetCartByIdAsync(int id)
    {
        //await _yourContext.Products.Include(p => p.Id).FirstOrDefaultAsync(p => p.Id == id);
        /*
        
        var cart = await _yourContext.Carts
        .Include(cp => cp.CartProductsList)
        .FirstOrDefaultAsync(c => c.Id == id);
        */

        var cart = await _yourContext.Carts
        .Include(c => c.CartProductsList)
        .ThenInclude(cp => cp.Product)
        .FirstOrDefaultAsync(c => c.Id == id);

        //var cart = await _yourContext.Carts.FirstOrDefaultAsync(c => c.Id == id);
        if (!cart.CartProductsList.Any())
        {
            throw new Exception($"Cart with Id {id} has no products.");
        }
        if (cart == null)
        {
            throw new Exception($"No cart found with Id {id}");
        }

        

        return cart;
    }

    public async Task<IEnumerable<Cart>> GetCartsAsync()
    {
        //return await _yourContext.Carts.Include(p => p.Id).ToListAsync();
        return await _yourContext.Carts
        .Include(c => c.CartProductsList)
            .ThenInclude(cp => cp.Product)
        .ToListAsync();

    }

    public async Task<Cart?> UpdateCartAsync(Cart cart)
    {

        var existingCart = await GetCartByIdAsync(cart.Id);
        if (existingCart == null) return null;

        // Remove existing cart products
        //_yourContext.CartProducts.RemoveRange(existingCart.CartProductsList);

        // Update cart properties
        existingCart.UserId = cart.UserId;
        existingCart.Date = cart.Date;
        existingCart.CartProductsList = cart.CartProductsList;

        // Validate new product references
        //await ValidateProductReferences(existingCart.CartProductsList);

        _yourContext.Carts.Update(existingCart);
        await _yourContext.SaveChangesAsync();
        return existingCart;
    }



    public async Task<(IEnumerable<Cart> Items, int TotalCount)> GetCartsAsync(
        int page = 1,
        int size = 10,
        string orderBy = "id")
    {
        // Start with base query including necessary relations
        var query = _yourContext.Carts
            .Include(c => c.CartProductsList)
            .AsQueryable();

        // Get total count before applying pagination
        var totalCount = await query.CountAsync();

        // Apply ordering
        query = ApplyOrdering(query, orderBy);

        // Apply pagination
        var items = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return (items, totalCount);
    }

    private IQueryable<Cart> ApplyOrdering(IQueryable<Cart> query, string orderBy)
    {
        // Split order clauses (e.g., "id desc, userId asc")
        var orderClauses = orderBy.Split(',', StringSplitOptions.RemoveEmptyEntries);
        var orderedQuery = query;
        var isFirstClause = true;

        foreach (var clause in orderClauses)
        {
            var parts = clause.Trim().Split(' ');
            var propertyName = parts[0].ToLower();
            var isDescending = parts.Length > 1 && parts[1].ToLower() == "desc";

            // Apply ordering based on property name
            switch (propertyName)
            {
                case "id":
                    orderedQuery = ApplyOrder(orderedQuery, c => c.Id, isDescending, isFirstClause);
                    break;
                case "userid":
                    orderedQuery = ApplyOrder(orderedQuery, c => c.UserId, isDescending, isFirstClause);
                    break;
                case "date":
                    orderedQuery = ApplyOrder(orderedQuery, c => c.Date, isDescending, isFirstClause);
                    break;
            }

            isFirstClause = false;
        }

        return orderedQuery;
    }

    private IQueryable<Cart> ApplyOrder<TKey>(
        IQueryable<Cart> query,
        Expression<Func<Cart, TKey>> orderBy,
        bool isDescending,
        bool isFirstClause)
    {
        if (isFirstClause)
        {
            return isDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
        }
        else
        {
            var orderedQuery = query as IOrderedQueryable<Cart>;
            return isDescending ? orderedQuery.ThenByDescending(orderBy) : orderedQuery.ThenBy(orderBy);
        }
    }

    private async Task ValidateProductReferences(IEnumerable<CartProduct> cartProducts)
    {
        var productIds = cartProducts.Select(cp => cp.ProductId).Distinct();
        var existingProducts = await _yourContext.Products
            .Where(p => productIds.Contains(p.Id))
            .Select(p => p.Id)
            .ToListAsync();

        var missingProducts = productIds.Except(existingProducts).ToList();
        if (missingProducts.Any())
        {
            throw new InvalidOperationException(
                $"Products with IDs {string.Join(", ", missingProducts)} do not exist.");
        }
    }
}
