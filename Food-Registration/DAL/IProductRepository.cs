using Food_Registration.Models;

namespace Food_Registration.DAL;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<Product?> GetProductByIdAsync(int id);
    Task<bool> CreateProductAsync(Product product);
    Task<bool> UpdateProductAsync(Product product);
    Task<bool> DeleteProductAsync(int id);
    Task<IEnumerable<Product>> GetFilteredProductsAsync(string? search, string? category);
    Task<string?> SaveProductImageAsync(IFormFile file, string wwwRootPath);
    Task<bool> DeleteProductImageAsync(string? imagePath, string wwwRootPath);
    Task<IEnumerable<Product>> GetProductsByProducerIdAsync(int producerId);
}