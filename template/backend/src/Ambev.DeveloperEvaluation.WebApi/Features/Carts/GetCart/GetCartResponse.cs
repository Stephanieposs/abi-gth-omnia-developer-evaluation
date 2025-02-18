namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.GetCart;

public class GetCartResponse
{
    public int UserId { get; set; }
    public DateTime Date { get; set; }
    public List<GetCartProductResponse> Products { get; set; } = new List<GetCartProductResponse>();
}
