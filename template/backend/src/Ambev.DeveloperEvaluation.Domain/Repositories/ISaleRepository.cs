using Ambev.DeveloperEvaluation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

public interface ISaleRepository
{
    Task<IEnumerable<Sale>> GetAllSales();
    Task<Sale> GetSaleById(int id);
    Task<Sale> CreateSale(Sale sale);
    Task<Sale> UpdateSale(Sale sale);
    Task<Sale> DeleteSale(int id);
}
