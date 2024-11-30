using Food_Registration.Models;

namespace Food_Registration.DAL;

public interface IProducerRepository
{
    Task<IEnumerable<Producer>> GetAllProducersAsync();
    Task<Producer?> GetProducerByIdAsync(int id);
    Task<bool> CreateProducerAsync(Producer producer);
    Task<bool> UpdateProducerAsync(Producer producer);
    Task<bool> DeleteProducerAsync(int id);
    Task<bool> HasAssociatedProductsAsync(int producerId);
    Task<string?> SaveProducerImageAsync(IFormFile file, string wwwRootPath);
    Task<bool> DeleteProducerImageAsync(string? imagePath, string wwwRootPath);
}