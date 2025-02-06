using Ambev.DeveloperEvaluation.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.UpdateProduct;

public class UpdateProductResponse
{
    [StringLength(255)]
    public string Title { get; set; } = null!;

    public decimal Price { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    public string? Category { get; set; }

    public string? Image { get; set; }

    public Rating? Rating { get; set; }
}
