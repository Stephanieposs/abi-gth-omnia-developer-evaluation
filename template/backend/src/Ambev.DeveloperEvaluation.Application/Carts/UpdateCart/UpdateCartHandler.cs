using Ambev.DeveloperEvaluation.Application.Carts.CreateCart;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;

public class UpdateCartHandler : IRequestHandler<UpdateCartCommand, UpdateCartResult>
{
    private readonly ICartRepository _repo;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateCartHandler> _logger;

    public UpdateCartHandler(ICartRepository repo, IMapper mapper, ILogger<UpdateCartHandler> logger)
    {
        _repo = repo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UpdateCartResult> Handle(UpdateCartCommand request, CancellationToken cancellationToken)
    {
        
        var validator = new UpdateCartValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return null;
        //throw new ValidationException(validationResult.Errors);

       

        var existingCart = await _repo.GetCartByIdAsync(request.CartId);
        if (existingCart == null)
        {
            _logger.LogWarning("Carrinho {CartId} não encontrado.", request.CartId);
            return null;
        }

        // Atualiza os dados do carrinho
        existingCart.Date = request.Date;
        existingCart.UserId = request.UserId;

        // Atualiza os produtos do carrinho
        var updatedProductIds = request.CartProductsList.Select(p => p.ProductId).ToHashSet();

        // Remove produtos que não estão na nova versão
        existingCart.CartProductsList.RemoveAll(p => !updatedProductIds.Contains(p.ProductId));

        // Adiciona ou atualiza produtos
        foreach (var productDto in request.CartProductsList)
        {
            var product = existingCart.CartProductsList
                .FirstOrDefault(p => p.ProductId == productDto.ProductId);

            if (product == null)
            {
                // Adiciona novo produto se não existir
                existingCart.CartProductsList.Add(new CartProduct
                {
                    ProductId = productDto.ProductId,
                    Quantity = productDto.Quantity,
                    CartId = existingCart.Id
                });
            }
            else
            {
                // Atualiza quantidade do produto existente
                product.Quantity = productDto.Quantity;
            }
        }

        await _repo.UpdateCartAsync(existingCart);
        _logger.LogInformation("Carrinho {CartId} atualizado com sucesso.", request.CartId);



        return new UpdateCartResult(
            existingCart.Id,
            existingCart.UserId,
            existingCart.Date,
            existingCart.CartProductsList.Select(p => new UpdateCartProductResult
            {
                ProductId = p.ProductId,
                Quantity = p.Quantity
            }).ToList()
        );
    }
}
   