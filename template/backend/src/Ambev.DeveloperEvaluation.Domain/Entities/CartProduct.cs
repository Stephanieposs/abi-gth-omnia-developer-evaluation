using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class CartProduct
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonIgnore]
    public int Id { get; set; }
    

    public int CartId { get; set; }

    [JsonIgnore]
    public Cart? Cart { get; set; }

    [JsonIgnore]
    public Product Product { get; set; } 

    public int ProductId { get; set; }

    

    public int Quantity { get; set; } 
    

}
