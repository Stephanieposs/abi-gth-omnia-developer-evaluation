using Ambev.DeveloperEvaluation.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Products.GetByCategory;

public class GetByCategoryCommand : IRequest<IEnumerable<Product>>
{
    public string Category { get; }

    public GetByCategoryCommand(string category)
    {
        Category = category;
    }
}
