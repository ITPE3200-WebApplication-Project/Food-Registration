using Food_Registration.Models;

namespace Food_Registration.DAL;

public interface IProducerRepository
{
    // Retrieves all producers from the database asynchronously.
    Task<IEnumerable<Producer>> GetAllProducersAsync();
    Task<Producer?> GetProducerByIdAsync(int id);
    Task<bool> CreateProducerAsync(Producer producer);
    Task<bool> UpdateProducerAsync(Producer producer);
    Task<bool> DeleteProducerAsync(int id);
    // Checks if a producer has any associated products asynchronously.
    Task<bool> HasAssociatedProductsAsync(int producerId);
    Task<string?> SaveProducerImageAsync(IFormFile file, string wwwRootPath);
    Task<bool> DeleteProducerImageAsync(string? imagePath, string wwwRootPath);
}