using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales;

public class SaleService : ISaleService
{
    private readonly ISaleRepository _repo;

    public SaleService(ISaleRepository repo)
    {
        _repo = repo;
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
        
        var newSale = new Sale
        {
            Date = DateTime.UtcNow,
            CustomerId = sale.CustomerId,
            CustomerName = sale.CustomerName,
            BranchId = sale.BranchId,
            BranchName = sale.BranchName,
            Items = new List<SaleItem>(),
            
        };

        var originalItems = sale.Items.ToList();

        decimal totalAmount = 0;
        foreach (var saleItem in originalItems)
        {
            var newSaleItem = new SaleItem
            {
                ProductId = saleItem.ProductId,
                ProductName = saleItem.ProductName,
                Quantity = saleItem.Quantity,
                UnitPrice = saleItem.UnitPrice,
            };

            newSaleItem.CalculateDiscountAndValidate();
            totalAmount += newSaleItem.Total;

            newSale.Items.Add(newSaleItem);
        }

        newSale.TotalAmount = totalAmount;

        return await _repo.CreateSale(newSale);
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
