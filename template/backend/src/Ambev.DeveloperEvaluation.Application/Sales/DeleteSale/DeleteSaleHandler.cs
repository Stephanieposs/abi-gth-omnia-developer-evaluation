using Ambev.DeveloperEvaluation.Application.Carts.DeleteCart;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

public class DeleteSaleHandler : IRequestHandler<DeleteSaleCommand, bool>
{
    private readonly ISaleRepository _repo;
    private readonly ILogger<DeleteSaleHandler> _logger;
    private readonly IMapper _mapper;

    public DeleteSaleHandler(ISaleRepository repo, ILogger<DeleteSaleHandler> logger, IMapper mapper)
    {
        _repo = repo;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<bool> Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = await _repo.GetSaleById(request.SaleId);

        if (sale == null)
        {
            _logger.LogWarning("Sale {SaleId} not found", request.SaleId);
            return false;
        }

        await _repo.DeleteSale(request.SaleId);

        _logger.LogInformation("Sale {SaleId} deleted successfully", request.SaleId);
        return true;
    }
}
