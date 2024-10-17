using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Alphatech.Services.OrderAPI.DBOrderContext;
using Alphatech.Services.OrderAPI.Models.Dto;
using Alphatech.Services.OrderAPI.Repository;
using Alphatech.Services.OrderAPI.RabbitMQ;
using Alphatech.Services.OrderAPI.Models;
using Alphatech.Services.OrderAPI.OrderServices;

namespace Alphatech.Services.OrderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : BaseController
    {
        protected ResponseDto _response = new();
        private IOrderRepository _iOrderRepository;
        //private readonly OrderService _orderService;
        private readonly GetProductOrderService _service;
        public OrderController(IOrderRepository iOrderRepository, GetProductOrderService service)
        {
            _iOrderRepository = iOrderRepository;
            _service = service;
        }

        [Route("GetOrders")]
        [HttpGet]
        public async Task<object> GetOrders()
        {
            try
            {
                IEnumerable<OrderDto> returnModel = await _iOrderRepository.GetOrders();
                _response.Result = returnModel;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };  
            }
            return _response;
        }

        [Route("GetDynamicOrders")]
        [HttpGet]
        public async Task<object> GetDynamicOrders()
        {
            try
            {
                IEnumerable<object> returnModel = await _iOrderRepository.GetDynamicOrders();
                _response.Result = returnModel;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }

        [Route("GetOrderById/{orderId:int}")]
        [HttpPut]
        public async Task<object> GetOrderById(int orderId)
        {
            try
            {
                var returnModel = await _iOrderRepository.GetOrderById(orderId);
                _response.Result = returnModel;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }

        [Route("CreateUpdateOrder")]
        [HttpPost]
        public async Task<object> CreateUpdateOrder(OrderDto orderDto)
        {
            try
            {
                if (orderDto == null)
                {
                    return BadRequest("Order cannot be null");
                }

                //var returnModel = await _iOrderRepository.CreateUpdateOrder(orderDto);
                //_response.Result = returnModel;


                // Here you would typically save the order to a database
                // For simplicity, we will just publish it to RabbitMQ

                DeclareQueue("order_queue");
                DeclareExchange("product_exchange", "direct");
                BindQueueToExchange("order_queue", "product_exchange", "product");

                _service.StartConsuming();

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            //  return _response;
            return CreatedAtAction(nameof(CreateUpdateOrder), new { ResponseMessage = "Order service is now consuming messages." }, orderDto);
        }

        [Route("DeleteOrder")]
        [HttpDelete]
        public async Task<object> DeleteOrder(int orderId)
        {
            try
            {
                if (orderId <= 0)
                {
                    return BadRequest();
                }

                var returnModel = await _iOrderRepository.DeleteOrder(orderId);
                _response.Result = returnModel;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }

        [Route("TestConnection")]
        [HttpGet]
        public async Task<object> TestConnection()
        {
            try
            {
                int returnModel = await _iOrderRepository.TestConnectionAsync();
                _response.Result = returnModel;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }
    }
}
