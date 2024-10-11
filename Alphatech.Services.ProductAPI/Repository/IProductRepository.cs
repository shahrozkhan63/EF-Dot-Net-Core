using Alphatech.Services.ProductAPI.Models.Dto;

namespace Alphatech.Services.ProductAPI.Repository
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductDto>> GetProducts();
        Task<IEnumerable<object>> GetDynamicProducts();
        Task<ProductDto> GetProductById(int productId);
        Task<ProductDto> CreateUpdateProduct(ProductDto product);
        Task<bool> DeleteProduct(int productId);
        Task<int> TestConnectionAsync();

    }
}
