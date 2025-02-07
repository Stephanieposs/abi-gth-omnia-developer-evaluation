using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

public class CreateSaleProfile : Profile
{
    public CreateSaleProfile() 
    {
        CreateMap<CreateSaleRequest, Sale>();
        CreateMap<CreateSaleResponse, Sale>().ReverseMap();
        CreateMap<CreateSaleItemResponse, SaleItem>().ReverseMap();
    }
}
