using Ambev.DeveloperEvaluation.Application.Products.DeleteProduct;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Carts.DeleteCart;

public class DeleteCartHandler : IRequestHandler<DeleteCartCommand, bool>
{
    private readonly ICartRepository _repo;
    private readonly ILogger<DeleteCartHandler> _logger;
    private readonly IMapper _mapper;

    public DeleteCartHandler(ICartRepository repo, ILogger<DeleteCartHandler> logger, IMapper mapper)
    {
        _repo = repo;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<bool> Handle(DeleteCartCommand request, CancellationToken cancellationToken)
    {
        var cart = await _repo.GetCartByIdAsync(request.CartId);

        if (cart == null)
        {
            _logger.LogWarning("Cart {CartIs} not found", request.CartId);
            return false;
        }

        await _repo.DeleteCartAsync(request.CartId);

        _logger.LogInformation("Cart {CartId} deleted successfully", request.CartId);
        return true;
    }
}
