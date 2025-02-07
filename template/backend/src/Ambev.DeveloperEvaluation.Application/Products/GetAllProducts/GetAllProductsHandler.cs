using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Products.GetAllProducts;

public class GetAllProductsHandler : IRequestHandler<GetAllProductsQuery, GetAllProductsResponse>
{
    private readonly IProductRepository _repo;
    private readonly ILogger<GetAllProductsHandler> _logger;

    public GetAllProductsHandler(ILogger<GetAllProductsHandler> logger, IProductRepository repo)
    {
        _logger = logger;
        _repo = repo;
    }

    public async Task<GetAllProductsResponse> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var (items, totalItems) = await _repo.GetFilteredAndOrderedProductsAsync(
                request.Page, request.Size, request.Order, request.Filters
            );

            return new GetAllProductsResponse
            {
                Data = items,
                TotalItems = totalItems,
                CurrentPage = request.Page,
                TotalPages = (int)Math.Ceiling(totalItems / (double)request.Size)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching products.");
            throw;
        }
    }
}
