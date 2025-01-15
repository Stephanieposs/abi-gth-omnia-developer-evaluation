using Ambev.DeveloperEvaluation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Ambev.DeveloperEvaluation.Domain.Services;

public interface ICartService
{
    Task<IEnumerable<Cart>> GetCartsAsync();
    //Task<IEnumerable<CartDTO>> GetCartsAsync();
    Task<Cart> GetCartByIdAsync(int id);
    Task<Cart> AddCartAsync(Cart cart);
    Task<Cart> UpdateCartAsync(Cart cart);
    Task<Cart> DeleteCartAsync(int id);
}
