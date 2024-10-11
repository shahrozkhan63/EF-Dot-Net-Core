using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Alphatech.Services.ProductAPI.DBProductContext;
using Alphatech.Services.ProductAPI.Models.Dto;
using Alphatech.Services.ProductAPI.Repository;

namespace Alphatech.Services.ProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        protected ResponseDto _response = new();
        private IProductRepository _iProductRepository;

        public ProductController(IProductRepository iProductRepository)
        {
            _iProductRepository = iProductRepository;
        }

        [Route("GetProducts")]
        [HttpGet]
        public async Task<object> GetProducts()
        {
            try
            {
                IEnumerable<ProductDto> returnModel = await _iProductRepository.GetProducts();
                _response.Result = returnModel;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };  
            }
            return _response;
        }

        [Route("GetDynamicProducts")]
        [HttpGet]
        public async Task<object> GetDynamicProducts()
        {
            try
            {
                IEnumerable<object> returnModel = await _iProductRepository.GetDynamicProducts();
                _response.Result = returnModel;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }

        [Route("GetProductById/{productId:int}")]
        [HttpPut]
        public async Task<object> GetProductById(int productId)
        {
            try
            {
                var returnModel = await _iProductRepository.GetProductById(productId);
                _response.Result = returnModel;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }

        [Route("CreateUpdateProduct")]
        [HttpPost]
        public async Task<object> CreateUpdateProduct(ProductDto productDto)
        {
            try
            {
                var returnModel = await _iProductRepository.CreateUpdateProduct(productDto);
                _response.Result = returnModel;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }

        [Route("DeleteProduct")]
        [HttpDelete]
        public async Task<object> DeleteProduct(int productId)
        {
            try
            {
                if (productId <= 0)
                {
                    return BadRequest();
                }

                var returnModel = await _iProductRepository.DeleteProduct(productId);
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
                int returnModel = await _iProductRepository.TestConnectionAsync();
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
