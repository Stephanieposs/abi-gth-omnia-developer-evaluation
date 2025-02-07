using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.UpdateProduct;

public class UpdateProductProfile : Profile
{
    public UpdateProductProfile()
    {
        CreateMap<UpdateProductRequest, Product>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<UpdateProductRatingRequest, Rating>();
        CreateMap<UpdateProductResponse, Product>().ReverseMap();
    }
}
