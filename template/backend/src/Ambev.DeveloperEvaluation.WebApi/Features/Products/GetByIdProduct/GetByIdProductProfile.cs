using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.GetByIdProduct;

public class GetByIdProductProfile : Profile
{

    public GetByIdProductProfile()
    {
        CreateMap<GetByIdProductResponse, Product>(); // Map from request to domain entity
        CreateMap<GetByIdProductRatingResponse, Rating>();
    }
}
