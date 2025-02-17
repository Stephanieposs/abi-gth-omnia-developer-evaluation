using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Carts.DeleteCart;

public class DeleteCartCommand : IRequest<bool>
{
    public int CartId { get; }

    public DeleteCartCommand(int cartId)
    {
        CartId = cartId;
    }
}
