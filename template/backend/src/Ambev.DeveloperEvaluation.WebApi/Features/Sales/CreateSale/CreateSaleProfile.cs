using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

public class CreateSaleProfile : Profile
{
    public CreateSaleProfile() 
    {
        CreateMap<CreateSaleRequest, Sale>().ReverseMap();
        CreateMap<CreateSaleResponse, Sale>().ReverseMap();
        CreateMap<CreateSaleItemResponse, SaleItem>().ReverseMap();
        CreateMap<CreateSaleCommand, Sale>().ReverseMap();
        CreateMap<CreateSaleCommand, CreateSaleRequest>().ReverseMap();
    }
}
