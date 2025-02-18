using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetAllSales;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSaleById;

public class GetSaleByIdProfile : Profile
{
    public GetSaleByIdProfile()
    {
        CreateMap<GetSaleByIdResponse, Sale>().ReverseMap();
        CreateMap<GetSaleByIdSaleItemResponse, SaleItem>().ReverseMap();
    }
}
