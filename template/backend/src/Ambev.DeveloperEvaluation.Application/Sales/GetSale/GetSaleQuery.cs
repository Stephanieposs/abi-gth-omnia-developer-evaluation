using Ambev.DeveloperEvaluation.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public class GetSaleQuery : IRequest<Sale>
{
    public int SaleId { get; set; }

    public GetSaleQuery(int saleId)
    {
        SaleId = saleId;
    }
}
