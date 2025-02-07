using Ambev.DeveloperEvaluation.Application.Products.DeleteProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Products.GetProduct;

public class GetProductHandler : IRequestHandler<GetProductCommand, Product>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<GetProductHandler> _logger;

    public GetProductHandler(IProductRepository productRepository, ILogger<GetProductHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<Product> Handle(GetProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetProductById(request.ProductId);

        if (product == null)
        {
            _logger.LogWarning("Product {ProductId} not found", request.ProductId);
            return null;
        }

        return product;
    }
}
