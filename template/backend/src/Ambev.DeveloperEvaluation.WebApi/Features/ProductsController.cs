using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ambev.DeveloperEvaluation.WebApi.Features;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : Controller
{
    public readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /*
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }
    */
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

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetById(int id)
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
    public async Task<ActionResult> Create(Product product)
    {
        await _productService.AddAsync(product);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, Product updatedProduct)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        updatedProduct.Id = id;
        await _productService.UpdateAsync(updatedProduct);
        return NoContent();
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
