using Ambev.DeveloperEvaluation.Application.Carts.DTOs;
using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.CreateCart;

public class CreateCartProfile : Profile
{
    public CreateCartProfile()
    {
        CreateMap<CreateCartRequest, Cart>(); 
        CreateMap<CreateCartProductRequest, CartProduct>();

        CreateMap<CartProduct, CreateCartProductRequest>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product.Id));

        CreateMap<Cart, CreateCartRequest>()
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.CartProductsList.Select(cp => new CreateCartProductRequest
            {
                ProductId = cp.ProductId,
                Quantity = cp.Quantity
            })));

        CreateMap<CreateCartRequest, Cart>()
            .ForMember(dest => dest.CartProductsList, opt => opt.MapFrom(src => src.Products.Select(p => new CartProduct
            {
                ProductId = p.ProductId,
                Quantity = p.Quantity
            })));

    }
}
