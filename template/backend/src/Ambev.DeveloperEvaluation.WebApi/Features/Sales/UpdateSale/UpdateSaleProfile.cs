using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

public class UpdateSaleProfile : Profile
{
    public UpdateSaleProfile()
    {
        CreateMap<UpdateSaleRequest, Sale>().ReverseMap();
        CreateMap<UpdateSaleResponse, Sale>().ReverseMap();
        CreateMap<UpdateSaleItemResponse, SaleItem>().ReverseMap();
        CreateMap<UpdateSaleRequest, UpdateSaleCommand>().ReverseMap();
        CreateMap<UpdateSaleCommand, Sale>().ReverseMap();
    }
}
