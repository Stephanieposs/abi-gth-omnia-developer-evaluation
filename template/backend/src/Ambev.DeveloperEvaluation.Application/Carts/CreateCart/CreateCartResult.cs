using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Carts.CreateCart;

public class CreateCartResult
{
    public int UserId { get; set; }
    public DateTime Date { get; set; }
    public List<CreateCartProductResult> Products { get; set; } = new List<CreateCartProductResult>();


}
