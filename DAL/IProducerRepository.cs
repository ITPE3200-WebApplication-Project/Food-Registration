using Food_Registration.Models;

namespace Food_Registration.DAL;

public interface IProducerRepository
{
    Task<IEnumerable<Producer>> GetAllProducersAsync();
    Task<Producer?> GetProducerByIdAsync(int id);
    Task AddProducerAsync(Producer producer);
    Task UpdateProducerAsync(Producer producer);
    Task DeleteProducerAsync(int id);
}