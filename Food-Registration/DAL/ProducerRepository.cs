using Microsoft.EntityFrameworkCore;
using Food_Registration.Models;

namespace Food_Registration.DAL;

public class ProducerRepository : IProducerRepository
{
    private readonly ItemDbContext _db;

    public ProducerRepository(ItemDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Producer>> GetAllProducersAsync()
    {
        return await _db.Producers.ToListAsync();
    }

    public async Task<Producer?> GetProducerByIdAsync(int id)
    {
        return await _db.Producers.FirstOrDefaultAsync(p => p.ProducerId == id);
    }

    public async Task AddProducerAsync(Producer producer)
    {
        _db.Producers.Add(producer);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateProducerAsync(Producer producer)
    {
        _db.Producers.Update(producer);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteProducerAsync(int id)
    {
        var producer = await _db.Producers.FirstOrDefaultAsync(p => p.ProducerId == id);
        if (producer != null)
        {
            _db.Producers.Remove(producer);
            await _db.SaveChangesAsync();
        }
    }
}