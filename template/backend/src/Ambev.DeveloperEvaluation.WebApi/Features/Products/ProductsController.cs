using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;
using Ambev.DeveloperEvaluation.Application.Products.DeleteProduct;
using Ambev.DeveloperEvaluation.Application.Products.GetAllCategories;
using Ambev.DeveloperEvaluation.Application.Products.GetAllProducts;
using Ambev.DeveloperEvaluation.Application.Products.GetByCategory;
using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.Application.Products.UpdateProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.CreateProduct;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.GetByIdProduct;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.UpdateProduct;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products;

[ApiController]
[Route("api/[controller]")]
// [Authorize]
public class ProductsController : ControllerBase
{
    //private readonly IProductService _productService;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductsController> _logger;
    private readonly IMediator _mediator;

    public ProductsController( IMapper mapper, ILogger<ProductsController> logger, IMediator mediator) //IProductService productService
    {
        //_productService = productService;
        _mapper = mapper;
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Roles = "Admin, Manager, Customer")]
    public async Task<ActionResult<GetAllProductsResponse>> GetAll(
        [FromQuery] int _page = 1,
        [FromQuery] int _size = 10,
        [FromQuery] string _order = "id asc")
    {
        try
        {
            // Extract filters from request object (if needed)
            var filtersExtract = Request.Query
                .Where(q => !q.Key.StartsWith("_")) // Exclude pagination and order keys
                .ToDictionary(q => q.Key, q => q.Value.ToString());

            // Send command via MediatR
            var query = new GetAllProductsQuery(_page, _size, _order, filtersExtract);
            var result = await _mediator.Send(query);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching products.");
            return StatusCode(500, "Internal Server Error");
        }
    }


    [HttpGet("{id}")]
    [Authorize(Roles = "Admin, Manager, Customer")]
    public async Task<ActionResult<GetByIdProductResponse>> GetById(int id)
    {
        // Send command via MediatR
        var command = new GetProductCommand(id);
        var result = await _mediator.Send(command);

        if (result == null)
        {
            _logger.LogWarning("Product {id} not found", id);
            return NotFound("Product not found");
        }

        return Ok(result);
    }

    // return all categories 
    [HttpGet("categories")]
    [Authorize(Roles = "Admin, Manager, Customer")]
    public async Task<ActionResult<IEnumerable<string>>> GetAllCategories()
    {
        try
        {
            // Send command via MediatR
            var query = new GetAllCategoriesQuery();
            var result = await _mediator.Send(query);

            return Ok(result);
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
    string category)
    {
        // Send command via MediatR
        var command = new GetByCategoryCommand(category);
        var result = await _mediator.Send(command);

        if (result == null)
        {
            _logger.LogWarning("Category {category} not found", category);
            return NotFound("Category not found");
        }

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin, Manager")]
    public async Task<ActionResult<CreateProductResponse>> Create(CreateProductRequest request)
    {
        // Validating
        var validator = new CreateProductRequestValidator();
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed when creating product: {request}", request);
            return BadRequest(validationResult.Errors);
        }

        // Send command via MediatR
        var command = _mapper.Map<CreateProductCommand>(request);
        var createdProduct = await _mediator.Send(command);

        if (createdProduct == null)
        {
            _logger.LogError("MediatR returned null for CreateProductCommand");
            return StatusCode(500, "Failed to create product.");
        }

        // Map response model
        var response = _mapper.Map<CreateProductResponse>(createdProduct);

        if (response == null)
        {
            _logger.LogError("AutoMapper failed to map CreateProductResponse.");
            return StatusCode(500, "Failed to create product response.");
        }

        return Ok(response);

    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin, Manager")]
    public async Task<ActionResult<UpdateProductResponse>> Update(int id, UpdateProductRequest request)
    {
        // Validating
        var validator = new UpdateProductRequestValidator();
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed when updating product: {request}", request);
            return BadRequest(validationResult.Errors);
        }

        
        // Send command via MediatR
        var command = _mapper.Map<UpdateProductCommand>(request);
        command.Id = id;

        var updatedProduct = await _mediator.Send(command);

        if (updatedProduct == null)
        {
            _logger.LogError("MediatR returned null for UpdatedProductCommand");
            return StatusCode(500, "Failed to update product.");
        }

        // Map response model
        var response = _mapper.Map<UpdateProductResponse>(updatedProduct);

        if (response == null)
        {
            _logger.LogError("AutoMapper failed to map UpdatedProductResponse.");
            return StatusCode(500, "Failed to update product response.");
        }

        return Ok(response);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        // Send command via MediatR
        var command = new DeleteProductCommand(id);
        var result = await _mediator.Send(command);

        if (!result)
        {
            _logger.LogWarning("Product {id} not found", id);
            return NotFound("Product not found");
        }

        return Ok("Product deleted successfully");
    }
}
