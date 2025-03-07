﻿using Ambev.DeveloperEvaluation.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Carts.GetCart;

public class GetCartQuery : IRequest<Cart>
{
    public int Id { get; set; }

    public GetCartQuery(int id)
    {
        Id = id;
    }
}
