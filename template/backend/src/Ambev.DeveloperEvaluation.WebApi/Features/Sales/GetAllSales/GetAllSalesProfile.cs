using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetAllSales;

public class GetAllSalesProfile : Profile
{
    public GetAllSalesProfile() 
    {
        CreateMap<GetAllSalesResponse, Sale>().ReverseMap();
        CreateMap<GetAllSaleItemResponse, SaleItem>().ReverseMap();
    }
}
