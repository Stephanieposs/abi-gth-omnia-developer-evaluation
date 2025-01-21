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

    /*
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CartDTO>>> GetAll()
    {

        var carts = await _cartService.GetCartsAsync();
        return Ok(_mapper.Map<IEnumerable<CartDTO>>(carts));

    }
    
    [HttpGet]
    public async Task<ActionResult<object>> GetAll(
    [FromQuery] int _page = 1,
    [FromQuery] int _size = 10,
    [FromQuery] string _order = "title asc")
    {
        var (carts, totalItems) = await _cartService.GetPagedCartsAsync(_page, _size, _order);
        var totalPages = (int)Math.Ceiling(totalItems / (double)_size);

        return Ok(new
        {
            data = carts,
            totalItems,
            currentPage = _page,
            totalPages
        });
    }*/

    [HttpGet]
    public async Task<ActionResult<object>> GetAll(
    [FromQuery] int _page = 1,
    [FromQuery] int _size = 10,
    [FromQuery] string _order = "id asc"   //,
    //[FromQuery] Dictionary<string, string> filters = null
        )
    {
        // Extract filters from query parameters
        var filtersExtract = Request.Query
            .Where(q => !q.Key.StartsWith("_")) // Exclude pagination and order keys
            .ToDictionary(q => q.Key, q => q.Value.ToString());

        // Call service to get filtered, ordered, and paginated cart
        var (items, totalItems) = await _cartService.GetFilteredAndOrderedCartsAsync(_page, _size, _order, filtersExtract);
        var totalPages = (int)Math.Ceiling(totalItems / (double)_size);

        // Return paginated and filtered response
        return Ok(new
        {
            data = items,
            totalItems,
            currentPage = _page,
            totalPages
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CartDTO>> GetById(int id)
    {
        var cart = await _cartService.GetCartByIdAsync(id);

        var cartDto = _mapper.Map<CartDTO>(cart);

        if (cart == null || cartDto ==null)
        {
            return NotFound("Cart Not Found");
        }
        return Ok(cartDto);
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

        foreach (var productDto in cartDto.Products)
        {

            var cartProduct = new CartProduct
            {
                ProductId = productDto.ProductId,
                Quantity = productDto.Quantity
            };
            //cart.CartProductsList.Add(cartProduct);
        }


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
