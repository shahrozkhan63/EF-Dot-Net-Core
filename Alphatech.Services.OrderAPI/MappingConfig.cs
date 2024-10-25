using Alphatech.Services.OrderAPI.Models;
using Alphatech.Services.OrderAPI.Models.Dto;
using AutoMapper;

namespace Alphatech.Services.OrderAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                // Map Order to OrderDto
                config.CreateMap<Order, OrderDto>()
                    .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems ?? new List<OrderItem>()));

                // Map OrderDto to Order
                config.CreateMap<OrderDto, Order>();

                // Map OrderItem to OrderItemDto
                config.CreateMap<OrderItem, OrderItemDto>();

                // Map OrderItemDto to OrderItem
                config.CreateMap<OrderItemDto, OrderItem>();
            });

            return mappingConfig;
        }
    }

}
