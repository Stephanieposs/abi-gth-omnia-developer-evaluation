using Ambev.DeveloperEvaluation.Application.Carts.CreateCart;
using Ambev.DeveloperEvaluation.Application.Carts.DeleteCart;
using Ambev.DeveloperEvaluation.Application.Carts.GetAllCarts;
using Ambev.DeveloperEvaluation.Application.Carts.GetCart;
using Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;
using Ambev.DeveloperEvaluation.Application.Products;
using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services;
using Ambev.DeveloperEvaluation.WebApi.Features.Carts.CreateCart;
using Ambev.DeveloperEvaluation.WebApi.Features.Carts.UpdateCart;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    public readonly IMapper _mapper;
    public readonly ILogger<CartController> _logger;
    private readonly IMediator _mediator;

    public CartController(IMapper mapper, ILogger<CartController> logger, IMediator mediator)
    {
        _mapper = mapper;
        _logger = logger;
        _mediator = mediator;
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

            //var (items, totalItems) = await _cartService.GetFilteredAndOrderedCartsAsync(_page, _size, _order, filtersExtract); //filters ?? new Dictionary<string, string>()
            //var totalPages = (int)Math.Ceiling(totalItems / (double)_size);

            var query = new GetAllCartsQuery(_page, _size, _order, filtersExtract ?? new Dictionary<string, string>());
            var result = await _mediator.Send(query);

            return Ok(result);

            // return customized json
            /*return Ok(new
            {
                data = items,
                totalItems,
                currentPage = _page,
                totalPages
            });*/
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching carts.");
            return StatusCode(500, "Internal Server Error");
        }

    }


    [HttpGet("{id}")]
    [Authorize(Roles = "Admin, Manager, Customer")]
    public async Task<ActionResult<Cart>> GetById(int id)
    {
        //var cart = await _cartService.GetCartByIdAsync(id);
        var query = new GetCartQuery(id);
        var cart = await _mediator.Send(query);



        if (cart == null)
        {
            _logger.LogWarning("Cart com ID {id} não encontrado.", id);
            return NotFound("Cart Not Found");
        }
        return Ok(cart);
    }

    [HttpPost]
    [Authorize(Roles = "Admin, Manager, Customer")]
    public async Task<ActionResult> Create(CreateCartRequest request)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("ModelState is not valid when creating {cartDto}", request);
            return BadRequest();
        }

        try
        {
            
            var cart = _mapper.Map<Cart>(request);
            if (!cart.CartProductsList.Any())
            {
                _logger.LogWarning("No products specified for the cart {cart}", cart);
                return BadRequest(cart);
            }
            
            //cart.UserId = request.UserId;
            var command = _mapper.Map<CreateCartCommand>(request);
            var createdCart = await _mediator.Send(command);

            return Ok(createdCart);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occourred while creating cart");
            return StatusCode(500, "Internal Server Error");
        }
    }

    /*
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin, Manager, Customer")]
    public async Task<ActionResult> Update(int id, UpdateCartRequest updatedCart)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("ModelState is not valid when updating to {updatedCart}", updatedCart);
            return BadRequest();
        }

        try
        {
            var command = _mapper.Map<UpdateCartCommand>(updatedCart);
            var createdCart = await _mediator.Send(command);
            //var existingCart = await _cartService.GetCartByIdAsync(id);
            if (existingCart == null)
            {
                _logger.LogWarning("Cart {id} wasn't found", id);
                return NotFound();
            }

            //existingCart.Date = updatedCart.Date;
            //existingCart.UserId = updatedCart.UserId;
             
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

       
    }*/


    [HttpPut("{id}")]
    [Authorize(Roles = "Admin, Manager, Customer")]
    public async Task<ActionResult> Update(int id, UpdateCartRequest updatedCart)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("ModelState is not valid when updating {updatedCart}", updatedCart);
            return BadRequest();
        }

        try
        {
            var command = new UpdateCartCommand(
                id,
                updatedCart.UserId,
                updatedCart.Date,
                updatedCart.Products.Select(product => new UpdateCartProductResult
                {
                    ProductId = product.ProductId,
                    Quantity = product.Quantity
                }).ToList()
            );

            // Send command using Mediator
            var updatedCartResult = await _mediator.Send(command);

            if (updatedCartResult == null)
            {
                _logger.LogWarning("Cart {id} wasn't found", id);
                return NotFound();
            }

            return Ok(updatedCartResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating the cart");
            return StatusCode(500, "Internal Server Error");
        }
    }


    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin, Manager")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var query = new GetCartQuery(id);
            var cart = await _mediator.Send(query);
            //var cart = await _cartService.GetCartByIdAsync(id);
            if (cart == null)
            {
                _logger.LogWarning("Cart {id} wasn't found", id);
                return NotFound();
            }

            var command = new DeleteCartCommand(id);
            await _mediator.Send(command);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occourred while deleting cart");
            return StatusCode(500, "Internal Server Error");
        }

    }

}
