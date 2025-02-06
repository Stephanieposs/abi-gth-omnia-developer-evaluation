using Ambev.DeveloperEvaluation.Application.Carts.DTOs;
using Ambev.DeveloperEvaluation.Application.Products.DTOs;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Services;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products;

[ApiController]
[Route("api/[controller]")]
//[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService productService, IMapper mapper, ILogger<ProductsController> logger)
    {
        _productService = productService;
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

            var (items, totalItems) = await _productService.GetFilteredAndOrderedProductsAsync(_page, _size, _order, filtersExtract); //filters ?? new Dictionary<string, string>()
            var totalPages = (int)Math.Ceiling(totalItems / (double)_size);

            // Return customized json
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
            _logger.LogError(ex, "An error occurred while fetching products.");
            return StatusCode(500, "Internal Server Error");
        }
        
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin, Manager, Customer")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
        {
            _logger.LogWarning("Product {id} wasn't found", id);
            return NotFound();
        }
        return Ok(product);
    }

    // return all categories 
    [HttpGet("categories")]
    [Authorize(Roles = "Admin, Manager, Customer")]
    public async Task<ActionResult<IEnumerable<string>>> GetAllCategories()
    {
        try
        {
            var categories = await _productService.GetAllProductCategoriesAsync();
            return Ok(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching categories.");
            return StatusCode(500, "Internal Server Error");
        }
       
    }

    // return products with a determined category
    [HttpGet("category/{category}")]
    [Authorize(Roles = "Admin, Manager, Customer")]
    public async Task<ActionResult<object>> GetByCategory(
    string category,
    [FromQuery] int _page = 1,
    [FromQuery] int _size = 10,
    [FromQuery] string _order = "title asc")
    {
        try
        {
            var (products, totalItems) = await _productService.GetPagedProductsByCategoryAsync(category, _page, _size, _order);
            var totalPages = (int)Math.Ceiling(totalItems / (double)_size);

            return Ok(new
            {
                data = products,
                totalItems,
                currentPage = _page,
                totalPages
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching products by category.");
            return StatusCode(500, "Internal Server Error");
        }

    }

    [HttpPost]
    [Authorize(Roles = "Admin, Manager")]
    public async Task<ActionResult> Create(ProductDto productDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("ModelState is not valid when creating {productDto}", productDto);
            return BadRequest();
        }

        try
        {
            var product = _mapper.Map<Product>(productDto);
            await _productService.AddAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating a product.");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin, Manager")]
    public async Task<ActionResult> Update(int id, ProductDto updatedProductDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("ModelState is not valid when updating {updatedProductDto}", updatedProductDto);
            return BadRequest();
        }

        try
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product {id} wasn't found", id);
                return NotFound();
            }

            var updatedProduct = _mapper.Map<Product>(updatedProductDto);
            updatedProduct.Id = id;
            await _productService.UpdateAsync(updatedProduct);
            return Ok(updatedProduct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating the product.");
            return StatusCode(500, "Internal Server Error");
        }
        
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product {id} wasn't found", id);
                return NotFound();
            }

            await _productService.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting the product.");
            return StatusCode(500, "Internal Server Error");
        }
    }
}
