using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.CreateProduct;

public class CreateProductProfile : Profile
{
    public CreateProductProfile()
    {
        CreateMap<CreateProductRequest, Product>(); // Map from request to domain entity
        CreateMap<CreateProductResponse, Product>();
        CreateMap<CreateProductRatingRequest, Rating>();
    }
}
