using Food_Registration.Models;

namespace Food_Registration.DAL;

public interface IProducerRepository
{
    Task<IEnumerable<Producer>> GetAllProducersAsync();
    Task<Producer?> GetProducerByIdAsync(int id);
    Task<bool> CreateProducerAsync(Producer producer);
    Task<bool> UpdateProducerAsync(Producer producer);
    Task<bool> DeleteProducerAsync(int id);
}