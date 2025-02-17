using Ambev.DeveloperEvaluation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Ambev.DeveloperEvaluation.Domain.Services;

public interface ICartService
{
    //Task<IEnumerable<Cart>> GetCartsAsync();
    Task<Cart> GetCartByIdAsync(int id);
    Task<Cart> AddCartAsync(Cart cart);
    Task<Cart> UpdateCartAsync(Cart cart);
    Task<Cart> DeleteCartAsync(int id);
    //Task<(IEnumerable<Cart> Carts, int TotalItems)> GetPagedCartsAsync(int page, int size, string order);
    Task<(IEnumerable<Cart> Items, int TotalItems)> GetFilteredAndOrderedCartsAsync(
    int page, int size, string order, Dictionary<string, string> filters);
}
