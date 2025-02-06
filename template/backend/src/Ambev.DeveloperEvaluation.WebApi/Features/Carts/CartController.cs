using Ambev.DeveloperEvaluation.Application.Carts.DTOs;
using Ambev.DeveloperEvaluation.Application.Products;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    public readonly ICartService _cartService;
    public readonly IMapper _mapper;
    public readonly ILogger<CartController> _logger;

    public CartController(ICartService cartService, IMapper mapper, ILogger<CartController> logger)
    {
        _cartService = cartService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "Admin, Manager, Customer")]
    public async Task<ActionResult<object>> GetAll(
    [FromQuery] int _page = 1,
    [FromQuery] int _size = 10,
    [FromQuery] string _order = "id asc")
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching carts.");
            return StatusCode(500, "Internal Server Error");
        }

    }

    [Authorize(Roles = "Admin, Manager, Customer")]
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<CartDTO>> GetById(int id)
    {
        var cart = await _cartService.GetCartByIdAsync(id);

        if (cart == null)
        {
            _logger.LogWarning("Cart com ID {id} não encontrado.", id);
            return NotFound("Cart Not Found");
        }
        return Ok(cart);
    }

    [HttpPost]
    [Authorize(Roles = "Admin, Manager, Customer")]
    public async Task<ActionResult> Create(CartDTO cartDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("ModelState is not valid when creating {cartDto}", cartDto);
            return BadRequest();
        }

        try
        {
            var cart = _mapper.Map<Cart>(cartDto);

            if (!cart.CartProductsList.Any())
            {
                _logger.LogWarning("No products specified for the cart {cartDto}", cartDto);
                return BadRequest();
            }

            var createdCart = await _cartService.AddCartAsync(cart);
            return Ok(createdCart);
        }
        catch(Exception ex) 
        {
            _logger.LogError(ex, "An error occourred while creating cart");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin, Manager, Customer")]
    public async Task<ActionResult> Update(int id, CartDTO updatedCart)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("ModelState is not valid when updating to {updatedCart}", updatedCart);
            return BadRequest();
        }

        try
        {
            var existingCart = await _cartService.GetCartByIdAsync(id);
            if (existingCart == null)
            {
                _logger.LogWarning("Cart {id} wasn't found", id);
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occourred while updating cart");
            return StatusCode(500, "Internal Server Error");
        }

       
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin, Manager")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var cart = await _cartService.GetCartByIdAsync(id);
            if (cart == null)
            {
                _logger.LogWarning("Cart {id} wasn't found", id);
                return NotFound();
            }

            await _cartService.DeleteCartAsync(id);
            return Ok(cart);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occourred while deleting cart");
            return StatusCode(500,"Internal Server Error");
        }
        
    }

}
