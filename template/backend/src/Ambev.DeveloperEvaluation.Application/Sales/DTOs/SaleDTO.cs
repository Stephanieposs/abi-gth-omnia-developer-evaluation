﻿using Ambev.DeveloperEvaluation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.ComponentModel;

namespace Ambev.DeveloperEvaluation.Application.Sales.DTOs;

public class SaleDTO
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int SaleNumber { get; set; }
    public DateTime Date { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; }
    public int BranchId { get; set; }
    public string BranchName { get; set; }

    [ReadOnly(true)]
    public decimal TotalAmount { get; set; }

    public List<SaleItemDTO> Items { get; set; } = new List<SaleItemDTO>();
}
