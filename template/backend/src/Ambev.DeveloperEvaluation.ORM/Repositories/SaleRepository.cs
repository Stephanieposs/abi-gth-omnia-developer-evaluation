using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _yourContext;

    public SaleRepository(DefaultContext yourContext)
    {
        _yourContext = yourContext;
    }

    public async Task<Sale> CreateSale(Sale sale)
    {
        _yourContext.Sales.Add(sale);
        await _yourContext.SaveChangesAsync();
        return sale;
    }

    public async Task<Sale> GetSaleById(int id)
    {
        var sale =  await _yourContext.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.SaleNumber == id);
        if (sale == null)
        {
            return null;
        }
        else
        {
            return sale;
        }
    }

    public async Task<IEnumerable<Sale>> GetAllSales()
    {
        return await _yourContext.Sales
            .Include(s => s.Items)
            .ToListAsync();
    }

    public async Task<Sale> UpdateSale(Sale sale)
    {
        if (sale != null)
        {
            _yourContext.Sales.Update(sale);
            await _yourContext.SaveChangesAsync();
        }


        return sale;
    }

    public async Task<Sale> DeleteSale(int id)
    {
        var sale = await GetSaleById(id);
        if (sale != null)
        {
            _yourContext.Sales.Remove(sale);
            await _yourContext.SaveChangesAsync();
        }
        return sale;
    }

    
}
