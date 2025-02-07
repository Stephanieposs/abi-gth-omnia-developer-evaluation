namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
public class UpdateSaleItemResponse
{
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
}
