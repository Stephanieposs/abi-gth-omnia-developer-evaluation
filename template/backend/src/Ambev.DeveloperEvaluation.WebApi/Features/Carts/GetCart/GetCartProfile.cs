using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.GetCart;

public class GetCartProfile : Profile
{
    public GetCartProfile()
    {
        //CreateMap<GetCartResponse, Cart>().ReverseMap();
        CreateMap<Cart, GetCartResponse>()
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.CartProductsList));

        CreateMap<CartProduct, GetCartProductResponse>().ReverseMap(); ;
    }
}
