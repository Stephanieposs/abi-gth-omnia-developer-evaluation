using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.CreateProduct;

public class CreateProductResponse
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Category { get; set; }

    public string? Description { get; set; }

    public Rating? Rating { get; set; }

       
}
