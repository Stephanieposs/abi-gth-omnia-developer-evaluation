using Ambev.DeveloperEvaluation.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.CreateProduct;

public class CreateProductRequest
{
    [Required]
    [StringLength(255)]
    public string Title { get; set; } = null!;

    [Required]
    public decimal Price { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    public string? Category { get; set; }

    public string? Image { get; set; }

    [Required]
    public CreateProductRatingRequest Rating { get; set; }
}
