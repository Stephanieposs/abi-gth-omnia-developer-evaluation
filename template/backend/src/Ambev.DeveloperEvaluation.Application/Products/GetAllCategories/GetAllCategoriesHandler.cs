using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Products.GetAllCategories;

public class GetAllCategoriesHandler : IRequestHandler<GetAllCategoriesQuery, IEnumerable<string>>
{
    private readonly ILogger<GetAllCategoriesHandler> _logger;
    private readonly IProductRepository _repo;

    public GetAllCategoriesHandler(ILogger<GetAllCategoriesHandler> logger, IProductRepository repo)
    {
        _logger = logger;
        _repo = repo;
    }

    public async Task<IEnumerable<string>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            return await _repo.GetAllProductsCategories();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching categories.");
            throw;
        }
    }
}
