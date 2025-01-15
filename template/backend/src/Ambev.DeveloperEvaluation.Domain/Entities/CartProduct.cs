using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class CartProduct
{

    //[JsonIgnore]
    public int CartId { get; set; }

    //[JsonIgnore]
    public Cart? Cart { get; set; } 
    
    //[JsonIgnore]
    public Product Product { get; set; } 

    public int ProductId { get; set; }

    

    public int Quantity { get; set; } 
    

}
