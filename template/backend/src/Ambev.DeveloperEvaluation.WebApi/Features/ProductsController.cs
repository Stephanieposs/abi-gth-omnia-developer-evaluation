using Ambev.DeveloperEvaluation.Application.Products.DTOs;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Services;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ambev.DeveloperEvaluation.WebApi.Features;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : Controller
{
    private readonly IProductService _productService;
    private readonly IMapper _mapper;

    public ProductsController(IProductService productService, IMapper mapper)
    {
        _productService = productService;
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

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    // return all categories 
    [HttpGet("categories")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<string>>> GetAllCategories()
    {
        var categories = await _productService.GetAllProductCategoriesAsync();
        return Ok(categories);
    }

    // return products with a determined category
    [HttpGet("category/{category}")]
    [Authorize]
    public async Task<ActionResult<object>> GetByCategory(
    string category,
    [FromQuery] int _page = 1,
    [FromQuery] int _size = 10,
    [FromQuery] string _order = "title asc")
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

    [HttpPost]
    [Authorize]
    public async Task<ActionResult> Create(ProductDto productDto)
    {
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => new ValidationFailure("", e.ErrorMessage)));
        }

        var product = _mapper.Map<Product>(productDto);
        await _productService.AddAsync(product);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult> Update(int id, ProductDto updatedProductDto)
    {
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => new ValidationFailure("", e.ErrorMessage)));
        }

        var product = await _productService.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        var updatedProduct = _mapper.Map<Product>(updatedProductDto);
        updatedProduct.Id = id;
        await _productService.UpdateAsync(updatedProduct);
        return Ok(updatedProduct);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> Delete(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        await _productService.DeleteAsync(id);
        return NoContent();
    }
}
