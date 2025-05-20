using MarketAPI.Models;

namespace MarketAPI.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product> CreateProductAsync(ProductInputModel productInput);
        Task<bool> UpdateProductAsync(int id, ProductInputModel productInput);
        Task<bool> DeleteProductAsync(int id);
    }
}
