using Ambev.DeveloperEvaluation.Application.Carts.DTOs;
using Ambev.DeveloperEvaluation.Application.Products;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.WebApi.Features;

[ApiController]
[Route("api/[controller]")]
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
    public async Task<ActionResult<IEnumerable<CartDTO>>> GetAll()
    {
        //_mapper.Map<IEnumerable<CartDTO>>(carts)
        var carts = await _cartService.GetCartsAsync();
        return Ok(carts);

    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CartDTO>> GetById(int id)
    {
        var cart = await _cartService.GetCartByIdAsync(id);

        //var cartDto = _mapper.Map<CartDTO>(cart);

        if (cart == null)
        {
            return NotFound("Cart Not Found");
        }
        return Ok(cart);
    }

    [HttpPost]
    public async Task<ActionResult> Create(CartDTO cartDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); 
        }

        var cart = _mapper.Map<Cart>(cartDto);

        if (!cart.CartProductsList.Any())
        {
            return BadRequest("No products specified for the cart.");
        }

        /*
        foreach (var productDto in cartDto.Products)
        {

            var cartProduct = new CartProduct
            {
                ProductId = productDto.ProductId,
                Quantity = productDto.Quantity
            };
            cart.CartProductsList.Add(cartProduct);
        }
        */

        var createdCart = await _cartService.AddCartAsync(cart);
        return Ok(createdCart);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, CartDTO updatedCart)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); 
        }

        var existingCart = await _cartService.GetCartByIdAsync(id);
        if (existingCart == null)
        {
            return NotFound();
        }
        
        //_mapper.Map(updatedCart, existingCart);

        existingCart.Date = updatedCart.Date;
        existingCart.UserId = updatedCart.UserId;

        foreach (var productDto in updatedCart.Products)
        {
            var product = existingCart.CartProductsList
                .FirstOrDefault(p => p.ProductId == productDto.ProductId);
            product.ProductId = productDto.ProductId;
            product.Quantity = productDto.Quantity;
            
        }

        await _cartService.UpdateCartAsync(existingCart);
        return Ok(existingCart);
        
    }

    [HttpDelete("{id}")]
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
