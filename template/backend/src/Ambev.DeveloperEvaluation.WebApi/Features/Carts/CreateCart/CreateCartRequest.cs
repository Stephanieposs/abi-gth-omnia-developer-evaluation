﻿using Ambev.DeveloperEvaluation.Application.Carts.DTOs;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.CreateCart;

public class CreateCartRequest
{
    public int UserId { get; set; }
    public DateTime Date { get; set; }
    public List<CreateCartProductRequest> Products { get; set; } = new List<CreateCartProductRequest>();
}
