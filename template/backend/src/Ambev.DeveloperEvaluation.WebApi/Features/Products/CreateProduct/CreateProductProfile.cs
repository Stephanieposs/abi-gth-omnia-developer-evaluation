using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.CreateProduct;

public class CreateProductProfile : Profile
{
    public CreateProductProfile()
    {
        CreateMap<CreateProductRequest, Product>(); 
        CreateMap<CreateProductResponse, Product>().ReverseMap();
        CreateMap<CreateProductRatingRequest, Rating>().ReverseMap();
    }
}
