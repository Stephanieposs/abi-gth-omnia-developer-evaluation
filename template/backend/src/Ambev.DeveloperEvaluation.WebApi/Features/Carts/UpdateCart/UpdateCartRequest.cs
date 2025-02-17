using Ambev.DeveloperEvaluation.WebApi.Features.Carts.CreateCart;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.UpdateCart;

public class UpdateCartRequest
{

    public int UserId { get; set; }
    public DateTime Date { get; set; }
    public List<UpdateCartProductRequest> Products { get; set; } = new List<UpdateCartProductRequest>();
}
