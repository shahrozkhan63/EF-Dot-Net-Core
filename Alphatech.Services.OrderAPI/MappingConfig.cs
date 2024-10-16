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
                config.CreateMap<OrderDto, Order>();
                config.CreateMap<Order, OrderDto>();
            });
            return mappingConfig;
        }
    }
}
