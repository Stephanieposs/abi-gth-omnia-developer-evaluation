using Ambev.DeveloperEvaluation.Application.Products.DTOs;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ambev.DeveloperEvaluation.WebApi.Features;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : Controller
{
    private readonly IProductService _productService;
    private readonly IMapper _mapper;

    public ProductsController(IProductService productService, IMapper mapper)
    {
        _productService = productService;
        _mapper = mapper;
    }

    /*
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }
    
    [HttpGet]
    public async Task<ActionResult<object>> GetAll(
    [FromQuery] int _page = 1,
    [FromQuery] int _size = 10,
    [FromQuery] string _order = "title asc")
    {
        var (products, totalItems) = await _productService.GetPagedProductsAsync(_page, _size, _order);
        var totalPages = (int)Math.Ceiling(totalItems / (double)_size);

        return Ok(new
        {
            data = products,
            totalItems,
            currentPage = _page,
            totalPages
        });
    }
*/
    [HttpGet]
    public async Task<ActionResult<object>> GetAll(
    [FromQuery] int _page = 1,
    [FromQuery] int _size = 10,
    [FromQuery] string _order = "id asc" // ,
    //[FromQuery] Dictionary<string, string> filters = null
        )
    {
        // Extract filters from query parameters
        var filtersExtract = Request.Query
            .Where(q => !q.Key.StartsWith("_")) // Exclude pagination and order keys
            .ToDictionary(q => q.Key, q => q.Value.ToString());

        var (items, totalItems) = await _productService.GetFilteredAndOrderedProductsAsync(_page, _size, _order, filtersExtract); //filters ?? new Dictionary<string, string>()
        var totalPages = (int)Math.Ceiling(totalItems / (double)_size);

        return Ok(new
        {
            data = items,
            totalItems,
            currentPage = _page,
            totalPages
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<string>>> GetAllCategories()
    {
        var categories = await _productService.GetAllProductCategoriesAsync();
        return Ok(categories);
    }

    /*
    [HttpGet("category/{category}")]
    public async Task<ActionResult<object>> GetByCategory(
        string category,
        [FromQuery] int _page = 1,
        [FromQuery] int _size = 10,
        [FromQuery] string _order = "title asc")
    {
        var result = await _productService.GetProductsByCategoryAsync(category); //, _page, _size, _order


        return Ok(result);
    }*/

    [HttpGet("category/{category}")]
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
    public async Task<ActionResult> Create(ProductDto productDto)
    {
        var product = _mapper.Map<Product>(productDto);
        await _productService.AddAsync(product);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, ProductDto updatedProductDto)
    {
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
