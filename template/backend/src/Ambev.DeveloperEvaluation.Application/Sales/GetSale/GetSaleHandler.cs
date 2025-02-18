using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public class GetSaleHandler : IRequestHandler<GetSaleQuery, Sale>
{
    private readonly ISaleRepository _repo;
    private readonly ILogger<GetSaleHandler> _logger;

    public GetSaleHandler(ISaleRepository repo, ILogger<GetSaleHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<Sale> Handle(GetSaleQuery request, CancellationToken cancellationToken)
    {
        var sale = await _repo.GetSaleById(request.SaleId);

        if (sale == null)
        {
            _logger.LogWarning("Sale {SaleIs} not found", request.SaleId);
            return null;
        }

        return sale;
    }
}
