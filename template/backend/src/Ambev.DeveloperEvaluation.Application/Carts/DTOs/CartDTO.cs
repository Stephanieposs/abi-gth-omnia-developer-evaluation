

using System.ComponentModel.DataAnnotations;

namespace Ambev.DeveloperEvaluation.Application.Carts.DTOs;

public class CartDTO
{
    public int UserId { get; set; }
    public DateTime Date { get; set; }
    public List<CartProductDTO> Products { get; set; } = new List<CartProductDTO>();
}
