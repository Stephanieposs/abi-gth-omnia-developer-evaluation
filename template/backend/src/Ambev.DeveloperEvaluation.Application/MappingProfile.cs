using Ambev.DeveloperEvaluation.Application.Carts.DTOs;
using Ambev.DeveloperEvaluation.Application.Sales.DTOs;
using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Cart, CartDTO>()
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.CartProductsList.Select(cp => new CartProductDTO
            {
                ProductId = cp.ProductId,
                Quantity = cp.Quantity
            })));

        // Mapeamento de CartDTO para Cart
        CreateMap<CartDTO, Cart>()
            .ForMember(dest => dest.CartProductsList, opt => opt.MapFrom(src => src.Products.Select(p => new CartProduct
            {
                ProductId = p.ProductId,
                Quantity = p.Quantity
            })));

        // Map CartProduct to CartProductDTO
        CreateMap<CartProduct, CartProductDTO>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product.Id));//.ReverseMap();


        CreateMap<Sale, SaleDTO>().ReverseMap();
        CreateMap<SaleItem, SaleItemDTO>().ReverseMap();
    }
}
