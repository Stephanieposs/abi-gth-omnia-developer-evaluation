using Ambev.DeveloperEvaluation.Application.Carts.GetAllCarts;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetAllSales;

public class GetAllSalesHandler : IRequestHandler<GetAllSalesQuery, IEnumerable<Sale>>
{
    private readonly ISaleRepository _repo;
    private readonly ILogger<GetAllSalesHandler> _logger;

    public GetAllSalesHandler(ILogger<GetAllSalesHandler> logger, ISaleRepository repo)
    {
        _logger = logger;
        _repo = repo;
    }

    public async Task<IEnumerable<Sale>> Handle(GetAllSalesQuery request, CancellationToken cancellationToken)
    {
       
        var sales = await _repo.GetAllSales();


        return sales;
    }

}
