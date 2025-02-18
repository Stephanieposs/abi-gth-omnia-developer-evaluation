
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;

public class UpdateCartResult 
{
    public UpdateCartResult(int id, int userId, DateTime date, List<UpdateCartProductResult> products) //
    {
        Id = id;
        UserId = userId;
        Date = date;
        Products = products;
    }

    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime Date { get; set; }
    public List<UpdateCartProductResult> Products { get; set; } = new List<UpdateCartProductResult>();
}
