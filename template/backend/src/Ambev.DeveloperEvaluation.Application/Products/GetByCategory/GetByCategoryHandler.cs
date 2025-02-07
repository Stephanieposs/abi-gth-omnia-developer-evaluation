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

namespace Ambev.DeveloperEvaluation.Application.Products.GetByCategory;

public class GetByCategoryHandler : IRequestHandler<GetByCategoryCommand, IEnumerable<Product>>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<GetByCategoryHandler> _logger;

    public GetByCategoryHandler(IProductRepository productRepository, ILogger<GetByCategoryHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<Product>> Handle(GetByCategoryCommand request, CancellationToken cancellationToken)
    {
        var categories = await _productRepository.GetProductsByCategoryAsync(request.Category);

        if (categories == null)
        {
            _logger.LogWarning("Category {categories} not found", request.Category);
            return null;
        }

        return (IEnumerable<Product>)categories;
    }
}
