﻿using Ambev.DeveloperEvaluation.Application.Carts.GetCart;
using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.Application.Sales.DTOs;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services;
using AutoMapper;
using AutoMapper.Configuration.Annotations;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales;

public class SaleService : ISaleService
{
    private readonly ISaleRepository _repo;
    private readonly ILogger<SaleService> _logger;
    private readonly IMediator _mediator;

    public SaleService(ISaleRepository repo, ILogger<SaleService> logger, IMediator mediator) 
    {
        _repo = repo;
        _logger = logger;
        _mediator = mediator;
    }

    public async Task<IEnumerable<Sale>> GetAllSales()
    {
        return await _repo.GetAllSales();
    }

    public async Task<Sale> GetSaleById(int id)
    {
        return await _repo.GetSaleById(id);
    }

    public async Task<Sale> CreateSale(Sale sale)
    {
        //var cart = await _cartService.GetCartByIdAsync(sale.CartId);
        var query = new GetCartQuery(sale.CartId);
        var cart = await _mediator.Send(query);


        if (cart == null || cart.Products == null || !cart.Products.Any())
        {
            //throw new Exception("Cart is empty or invalid.");
            _logger.LogError("Cart is empty or invalid at sale {sale}", sale);
            return null;
        }

        
        if (sale.Items == null)
        {
            sale.Items = new List<SaleItem>();
            _logger.LogWarning("saleItems null, new saleItem list was created at {sale}", sale);
            //Console.WriteLine("console new List");
        }

        var newSaleItems = new List<SaleItem>();

        foreach (var cartProduct in cart.Products)  
        {

            //var product = await _productService.GetByIdAsync(cartProduct.ProductId);

            var command = new GetProductCommand(cartProduct.ProductId);
            var product = await _mediator.Send(command);

            if (product == null)
            {
                _logger.LogError("Product {product} not found", product);
                //throw new Exception($"Product with ID {cartProduct.ProductId} not found.");
                return null;
            }

            var saleItem = new SaleItem
            {
                ProductId = cartProduct.ProductId,
                //ProductItem = cartProduct.Product,
                ProductName = product.Title,
                //CartItem = cart,                          -------------------------------------------------------------------------------------------
                //CartItemId = cart.Id,
                UnitPrice = product.Price,
                Quantity = cartProduct.Quantity,
                IsCancelled = false
            };


            saleItem.CalculateDiscountAndValidate();
            
            saleItem.Total = saleItem.UnitPrice * saleItem.Quantity * (1 - saleItem.Discount);

            newSaleItems.Add(saleItem);

        }

        sale.Items.AddRange(newSaleItems);

        sale.TotalAmount = sale.Items.Sum(item => item.Total);
        sale.CustomerId = cart.UserId;

        var createdSale = await _repo.CreateSale(sale);
        return sale;
    }



    public async Task<Sale> UpdateSale(Sale sale)
    {
        var existingSale = await _repo.GetSaleById(sale.SaleNumber);
        if (existingSale == null)
        {
            _logger.LogError("Sale with SaleNumber {sale.SaleNumber} not found", sale.SaleNumber);
            return null;
            //throw new KeyNotFoundException($"Sale with SaleNumber {sale.SaleNumber} not found.");
        }

        existingSale.BranchName = sale.BranchName;
        existingSale.BranchId = sale.BranchId;
        existingSale.CustomerId = sale.CustomerId;
        existingSale.CustomerName = sale.CustomerName;
        existingSale.Date = sale.Date;
        existingSale.IsCancelled = sale.IsCancelled;
        existingSale.TotalAmount = sale.TotalAmount;
        existingSale.Items = sale.Items;

        await _repo.UpdateSale(existingSale);
        return existingSale;
    }

    public async Task<Sale> DeleteSale(int id)
    {
        return await _repo.DeleteSale(id);
    }
}
