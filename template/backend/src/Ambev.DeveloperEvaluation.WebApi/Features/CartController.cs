using Ambev.DeveloperEvaluation.Application.Carts.DTOs;
using Ambev.DeveloperEvaluation.Application.Products;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

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

        var carts = await _cartService.GetCartsAsync();
        return Ok(_mapper.Map<IEnumerable<CartDTO>>(carts));


        //var carts = await _cartService.GetCartsAsync();
        //return Ok(carts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CartDTO>> GetById(int id)
    {
        var cart = await _cartService.GetCartByIdAsync(id);
        if (cart == null)
        {
            return NotFound();
        }
        return Ok(_mapper.Map<CartDTO>(cart));
    }

    [HttpPost]
    public async Task<ActionResult> Create(CartDTO cartDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); // Return validation errors if any
        }

        var cart = _mapper.Map<Cart>(cartDto);
        await _cartService.AddCartAsync(cart);
        return CreatedAtAction(nameof(GetById), new { id = cart.Id }, _mapper.Map<CartDTO>(cart));

        //await _cartService.AddCartAsync(cartDto);
        //return CreatedAtAction(nameof(GetById), new { id = cartDto.Id }, cartDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, CartDTO updatedCart)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); // Return validation errors if any
        }

        var existingCart = await _cartService.GetCartByIdAsync(id);
        if (existingCart == null)
        {
            return NotFound();
        }

        _mapper.Map(updatedCart, existingCart); // Update existing cart entity
        await _cartService.UpdateCartAsync(existingCart);
        return Ok(_mapper.Map<CartDTO>(existingCart));
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
