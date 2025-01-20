using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SaleItem
{
    public int Id { get; set; }
    public int SaleId { get; set; }


    [JsonIgnore]
    public Sale Sale { get; set; }

    public int ProductId { get; set; }

    [JsonIgnore]
    public Product ProductItem { get; set; }
    [JsonIgnore]
    public Cart CartItem { get; set; }

    public int CartItemId { get; set; }

    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; } 
    public decimal Total { get; set; }
    public bool IsCancelled { get; set; }

    public void CalculateDiscountAndValidate()
    {
        if (CartItem == null || CartItem.CartProductsList == null)
        {
            throw new InvalidOperationException("CartItem or CartProductsList is null.");
        }


        foreach (var item in CartItem.CartProductsList)
        {
            if (item.Quantity > 20)
                throw new InvalidOperationException("Cannot sell more than 20 items of the same product.");

            if (item.Quantity >= 10)
            {
                Discount = 0.2m;
            }
            else if (item.Quantity >= 4)
            {
                Discount = 0.1m;
            }
            else
            {
                Discount = 0m;
            }

            if (item.Product == null)
            {
                throw new InvalidOperationException("Product is null in CartProductsList.");
            }

            Total = item.Quantity * item.Product.Price * (1 - Discount);
        }
    }
}
