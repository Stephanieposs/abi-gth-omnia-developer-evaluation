using Ambev.DeveloperEvaluation.Application.Carts.CreateCart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Carts.GetAllCarts;

public class GetAllCartsResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime Date { get; set; }
    public List<GetAllCartsProductResponse> CartProductsList { get; set; } = new List<GetAllCartsProductResponse>();

}
