
namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.GetByIdProduct;

public class GetByIdProductResponse
{
    public string? Title { get; set; }
    public decimal? Price { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? Image { get; set; }
    public GetByIdProductRatingResponse? RatingResponse { get; set; }
}
