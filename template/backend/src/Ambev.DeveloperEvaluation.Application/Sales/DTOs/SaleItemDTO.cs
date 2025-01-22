using Ambev.DeveloperEvaluation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.DTOs;

public class SaleItemDTO
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
