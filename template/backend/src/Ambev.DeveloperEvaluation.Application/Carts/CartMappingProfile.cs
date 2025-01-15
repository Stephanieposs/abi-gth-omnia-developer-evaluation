using Ambev.DeveloperEvaluation.Application.Carts.DTOs;
using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;

namespace Ambev.DeveloperEvaluation.Application.Carts;

public class CartMappingProfile : Profile
{
    public readonly IMapper _mapper;
    public CartMappingProfile()
    {
      

        CreateMap<Cart, CartDTO>();
        CreateMap<CartDTO, Cart>();
        CreateMap<CartProduct, CartProductDTO>();
        CreateMap<CartProductDTO, CartProduct>();

        CreateMap<Cart, CartDTO>()
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.CartProductsList.Select(item => _mapper.Map<CartProductDTO>(item.Product))));

    }
}
