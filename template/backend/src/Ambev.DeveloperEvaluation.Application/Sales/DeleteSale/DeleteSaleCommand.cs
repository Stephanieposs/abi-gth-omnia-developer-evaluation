using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

public class DeleteSaleCommand : IRequest<bool>
{
    public int SaleId { get; }

    public DeleteSaleCommand(int saleId)
    {
        SaleId = saleId;
    }
}
