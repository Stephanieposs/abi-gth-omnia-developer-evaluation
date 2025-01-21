using Ambev.DeveloperEvaluation.Application.Sales.DTOs;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services;
using AutoMapper;
using AutoMapper.Configuration.Annotations;
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
    //private readonly HttpClient _httpClient;
    private readonly IMapper _mapper;
    private readonly ICartService _cartService;
    private readonly IProductService _productService;

    public SaleService(ISaleRepository repo, IMapper mapper, ICartService cartService, IProductService productService)
    {
        _repo = repo;
        //_httpClient = httpClient;
        _mapper = mapper;
        _cartService = cartService;
        _productService = productService;
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
        var cart = await _cartService.GetCartByIdAsync(sale.CartId);

        if (cart == null || cart.CartProductsList == null || !cart.CartProductsList.Any())
        {
            throw new Exception("Cart is empty or invalid.");
        }

        
        if (sale.Items == null)
        {
            sale.Items = new List<SaleItem>();
            Console.WriteLine("console new List");
        }

        var newSaleItems = new List<SaleItem>();

        foreach (var cartProduct in cart.CartProductsList)  // var items in sale.Items
        {
            var product = await _productService.GetByIdAsync(cartProduct.ProductId);
            
            if (product == null)
            {
                throw new Exception($"Product with ID {cartProduct.ProductId} not found.");
            }

            var saleItem = new SaleItem
            {
                ProductId = cartProduct.ProductId,
                ProductItem = cartProduct.Product,
                ProductName = product.Title,
                CartItem = cart,
                CartItemId = cart.Id,
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

        var createdSale = await _repo.CreateSale(sale);
        return sale;
    }



    public async Task<Sale> UpdateSale(Sale sale)
    {
        var existingSale = await _repo.GetSaleById(sale.SaleNumber);
        if (existingSale == null)
        {
            throw new KeyNotFoundException($"Sale with SaleNumber {sale.SaleNumber} not found.");
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
