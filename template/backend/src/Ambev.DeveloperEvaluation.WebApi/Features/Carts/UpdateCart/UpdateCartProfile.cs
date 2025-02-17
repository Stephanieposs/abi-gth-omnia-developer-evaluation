using Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.WebApi.Features.Carts.CreateCart;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.UpdateCart;

public class UpdateCartProfile : Profile
{
    public UpdateCartProfile()
    {
        CreateMap<UpdateCartRequest, Cart>();
        CreateMap<UpdateCartProductRequest, CartProduct>();

        CreateMap<CartProduct, UpdateCartProductRequest>()
           .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product.Id));

        CreateMap<Cart, UpdateCartRequest>()
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.CartProductsList.Select(cp => new CreateCartProductRequest
            {
                ProductId = cp.ProductId,
                Quantity = cp.Quantity
            })));

        CreateMap<UpdateCartRequest, Cart>()
            .ForMember(dest => dest.CartProductsList, opt => opt.MapFrom(src => src.Products.Select(p => new CartProduct
            {
                ProductId = p.ProductId,
                Quantity = p.Quantity
            })));
        CreateMap<UpdateCartResult, Cart>().ReverseMap();
    }
}
