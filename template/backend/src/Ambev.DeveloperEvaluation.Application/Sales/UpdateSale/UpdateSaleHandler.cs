using Ambev.DeveloperEvaluation.Application.Carts.GetCart;
using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
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

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, Sale>
{
    private readonly ISaleRepository _repo;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateSaleHandler> _logger;
    private readonly IMediator _mediator;

    public UpdateSaleHandler(ISaleRepository repo, IMapper mapper, ILogger<UpdateSaleHandler> logger, IMediator mediator)
    {
        _repo = repo;
        _mapper = mapper;
        _logger = logger;
        _mediator = mediator;
    }

    public async Task<Sale> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var existingSale = await _repo.GetSaleById(command.SaleNumber);
        if (existingSale == null)
        {
            _logger.LogWarning("Sale {saleNumber} wasn't found", command.SaleNumber);
            return null;
        }

        if (command.CartId != existingSale.CartId)
        {
            // Buscar o Cart pelo ID
            var cartQuery = new GetCartQuery(command.CartId);
            var cart = await _mediator.Send(cartQuery);

            if (cart == null || cart.CartProductsList == null || !cart.CartProductsList.Any())
            {
                _logger.LogError("Cart is empty or invalid: {CartId}", command.CartId);
                return null;
            }

            var saleItems = new List<SaleItem>();

            // 3️⃣ Buscar os produtos e criar os itens da venda
            foreach (var cartProduct in cart.CartProductsList)
            {
                var productQuery = new GetProductCommand(cartProduct.ProductId);
                var product = await _mediator.Send(productQuery);

                if (product == null)
                {
                    _logger.LogError("Product {ProductId} not found", cartProduct.ProductId);
                    return null; // Ignorar este item se o produto não existir
                }

                var cartCart = _mapper.Map<Cart>(cart);

                var saleItem = new SaleItem
                {
                    ProductId = product.Id,
                    ProductItem = product,
                    ProductName = product.Title,
                    CartItem = cartCart,
                    CartItemId = command.CartId,
                    UnitPrice = product.Price,
                    Quantity = cartProduct.Quantity,
                    IsCancelled = false
                };

                // 4️⃣ Calcular descontos e totais
                saleItem.CalculateDiscountAndValidate();
                saleItem.Total = saleItem.UnitPrice * saleItem.Quantity * (1 - saleItem.Discount);

                saleItems.Add(saleItem);
            }

            // 5️⃣ Associar os itens à venda e calcular o total
            existingSale.Items.Clear();
            existingSale.Items.AddRange(saleItems);

            existingSale.TotalAmount = existingSale.Items.Sum(item => item.Total);
            existingSale.CustomerId = cart.UserId;
        }



        var sale = _mapper.Map<Sale>(command);


        existingSale.BranchName = command.BranchName;
        existingSale.BranchId = command.BranchId;
        existingSale.CustomerName = command.CustomerName;
        existingSale.Date = command.Date;

        var updatedSale = await _repo.UpdateSale(existingSale);
        _logger.LogInformation("Updated Cart {Cart}", updatedSale);

        return existingSale;
    }
}


