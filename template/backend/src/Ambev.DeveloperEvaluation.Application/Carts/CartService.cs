using Ambev.DeveloperEvaluation.Application.Carts.DTOs;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services;
//using Ambev.DeveloperEvaluation.ORM.Repositories;
//using Ambev.DeveloperEvaluation.WebApi.Common;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ambev.DeveloperEvaluation.Application.Carts;

public class CartService : ICartService
{
    public readonly ICartRepository _repo;
    public readonly IMapper _mapper;
    public readonly HttpClient _httpClient;

    public CartService(ICartRepository repo)
    {
        _repo = repo;
    }

    public async Task<Cart> AddCartAsync(Cart cart)
    {
        var addedCart = await _repo.AddCartAsync(cart);
        return addedCart;
    }

    public async Task<Cart> DeleteCartAsync(int id)
    {
        //return await _repo.DeleteCartAsync(id);
        return await _repo.DeleteCartAsync(id);
    }

    public async Task<Cart> GetCartByIdAsync(int id)
    {
        var cart = await _repo.GetCartByIdAsync(id);
        return cart;
    }

    public async Task<Cart> UpdateCartAsync(Cart cart)
    {
        var updatedCart = await _repo.UpdateCartAsync(cart);
        return updatedCart ;
    }




    public async Task<IEnumerable<Cart>> GetCartsAsync()
    {
        // Get paginated data from repository
        return await _repo.GetCartsAsync();

    }

    




}
