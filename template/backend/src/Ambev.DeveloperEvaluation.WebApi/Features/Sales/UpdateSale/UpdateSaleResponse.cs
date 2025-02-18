using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

public class UpdateSaleResponse
{
    public int SaleNumber { get; set; }
    public DateTime Date { get; set; }
    public int CartId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; }
    public int BranchId { get; set; }
    public string BranchName { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsCancelled { get; set; }


    public List<UpdateSaleItemResponse> Items { get; set; } = new List<UpdateSaleItemResponse>();
}
