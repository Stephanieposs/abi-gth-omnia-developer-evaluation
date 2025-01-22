using Ambev.DeveloperEvaluation.Application.Carts.DTOs;
using Ambev.DeveloperEvaluation.Application.Products;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.WebApi.Features;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : Controller
{
    public readonly ICartService _cartService;
    public readonly IMapper _mapper;

    public CartController(ICartService cartService, IMapper mapper)
    {
        _cartService = cartService;
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<object>> GetAll(
    [FromQuery] int _page = 1,
    [FromQuery] int _size = 10,
    [FromQuery] string _order = "id asc")
    {
        // Extract filters from query parameters
        var filtersExtract = Request.Query
            .Where(q => !q.Key.StartsWith("_")) // Exclude pagination and order keys
            .ToDictionary(q => q.Key, q => q.Value.ToString());

        var (items, totalItems) = await _cartService.GetFilteredAndOrderedCartsAsync(_page, _size, _order, filtersExtract); //filters ?? new Dictionary<string, string>()
        var totalPages = (int)Math.Ceiling(totalItems / (double)_size);

        // return customized json
        return Ok(new
        {
            data = items,
            totalItems,
            currentPage = _page,
            totalPages
        });
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<CartDTO>> GetById(int id)
    {
        var cart = await _cartService.GetCartByIdAsync(id);

        if (cart == null)
        {
            return NotFound("Cart Not Found");
        }
        return Ok(cart);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult> Create(CartDTO cartDto)
    {
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => new ValidationFailure("", e.ErrorMessage)));
        }

        var cart = _mapper.Map<Cart>(cartDto);

        if (!cart.CartProductsList.Any())
        {
            return BadRequest("No products specified for the cart.");
        }

        var createdCart = await _cartService.AddCartAsync(cart);
        return Ok(createdCart);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult> Update(int id, CartDTO updatedCart)
    {
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => new ValidationFailure("", e.ErrorMessage)));
        }

        var existingCart = await _cartService.GetCartByIdAsync(id);
        if (existingCart == null)
        {
            return NotFound();
        }

        existingCart.Date = updatedCart.Date;
        existingCart.UserId = updatedCart.UserId;

        // Update products in the cart
        var updatedProductIds = updatedCart.Products.Select(p => p.ProductId).ToHashSet();

        // Remove products that are no longer in the updated cart
        existingCart.CartProductsList.RemoveAll(p => !updatedProductIds.Contains(p.ProductId));

        // Add or update products
        foreach (var productDto in updatedCart.Products)
        {
            var product = existingCart.CartProductsList
                .FirstOrDefault(p => p.ProductId == productDto.ProductId);

            if (product == null)
            {
                // Add new product if it doesn't exist
                existingCart.CartProductsList.Add(new CartProduct
                {
                    ProductId = productDto.ProductId,
                    Quantity = productDto.Quantity,
                    CartId = existingCart.Id // Ensure the CartId is set
                });
            }
            else
            {
                // Update existing product
                product.Quantity = productDto.Quantity;
            }
        }

        await _cartService.UpdateCartAsync(existingCart);
        return Ok(existingCart);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> Delete(int id)
    {
        var cart = await _cartService.GetCartByIdAsync(id);
        if (cart == null)
        {
            return NotFound();
        }

        await _cartService.DeleteCartAsync(id);
        return Ok(cart);
    }

}
