using Ambev.DeveloperEvaluation.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleCommand : IRequest<Sale>
{
    public int SaleNumber { get; set; }
    public DateTime Date { get; set; }
    public int CartId { get; set; }
    public string? CustomerName { get; set; }
    public int BranchId { get; set; }
    public string? BranchName { get; set; }
    public List<SaleItem> Items { get; set; } = new List<SaleItem>();
}
