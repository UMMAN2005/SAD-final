using API.Dtos;
using AutoMapper;
using Core.Entities;

namespace API.Helpers.Profiles;

public class MappingProfile : Profile {
  public MappingProfile(IHttpContextAccessor accessor, IConfiguration configuration) {
    var context = accessor.HttpContext;

    var uriBuilder = new UriBuilder(context!.Request.Scheme, context.Request.Host.Host, context.Request.Host.Port ?? -1);
    if (uriBuilder.Uri.IsDefaultPort) uriBuilder.Port = -1;
    var baseUrl = uriBuilder.Uri.AbsoluteUri;

    CreateMap<OrderPostDto, Order>().ReverseMap();
    CreateMap<OrderGetDto, Order>().ReverseMap();

    CreateMap<ProductPostDto, Product>().ReverseMap();
    CreateMap<ProductGetDto, Product>().ReverseMap();

    CreateMap<CardPostDto, Card>().ReverseMap();
    CreateMap<CardGetDto, Card>().ReverseMap();

    CreateMap<PaymentPostDto, Payment>().ReverseMap();
    CreateMap<PaymentGetDto, Payment>().ReverseMap();

    CreateMap<ReviewPostDto, Review>().ReverseMap();
    CreateMap<ReviewGetDto, Review>().ReverseMap();

    CreateMap<CategoryPostDto, Category>().ReverseMap();
    CreateMap<CategoryGetDto, Category>().ReverseMap();

    CreateMap<OrderItemGetDto, OrderItem>().ReverseMap();

    CreateMap<AppUserGetDto, AppUser>().ReverseMap();
  }
}