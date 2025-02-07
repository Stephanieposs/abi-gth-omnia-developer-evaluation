namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

public class UpdateSaleRequest
{
    public DateTime Date { get; set; }
    public int CartId { get; set; }
    public string? CustomerName { get; set; }
    public int BranchId { get; set; }
    public string? BranchName { get; set; }
}
