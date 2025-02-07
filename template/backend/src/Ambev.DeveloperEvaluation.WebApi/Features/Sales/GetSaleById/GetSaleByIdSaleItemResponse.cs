namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSaleById;

public class GetSaleByIdSaleItemResponse
{
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
}
