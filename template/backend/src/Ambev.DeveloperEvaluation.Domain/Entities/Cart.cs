using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static Ambev.DeveloperEvaluation.Domain.Entities.CartProduct;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Cart
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime Date { get; set; }

    public List<CartProduct> CartProductsList { get; set; } = new List<CartProduct>();

}


  