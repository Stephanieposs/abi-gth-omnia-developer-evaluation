using Ambev.DeveloperEvaluation.Application.Carts.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Carts.GetAllCarts;

public class GetAllCartsQuery : IRequest<GetAllCartsPagedResponse<GetAllCartsResponse>>
{
    //public int UserId { get; set; }
    //public DateTime Date { get; set; }
    //public List<GetAllCartsProductResponse> Products { get; set; } = new List<GetAllCartsProductResponse>();


    public int Page { get; set; }
    public int Size { get; set; }
    public string Order { get; set; }
    public Dictionary<string, string> Filters { get; set; }

    public GetAllCartsQuery(int page, int size, string order, Dictionary<string, string> filters)
    {
        Page = page;
        Size = size;
        Order = order;
        Filters = filters ?? new Dictionary<string, string>();
    }
}
