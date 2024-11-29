using Microsoft.EntityFrameworkCore;
using Food_Registration.Models;

namespace Food_Registration.DAL;

public class ProducerRepository : IProducerRepository
{
    private readonly ItemDbContext _db;

    private readonly ILogger<ProducerRepository> _logger;

    public ProducerRepository(ItemDbContext db, ILogger<ProducerRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IEnumerable<Producer>> GetAllProducersAsync()
    {
        try
        {
            if (_db.Producers != null)
            {
                return await _db.Producers.ToListAsync();
            }
            return new List<Producer>();
        }
        catch (Exception e)
        {
            _logger.LogError("[ProducerRepository] items ToListAsync() failed when GetAllProducersAsync() is called, error message: {e}", e.Message);
            return new List<Producer>();
            // return null;
        }
    }

    public async Task<Producer?> GetProducerByIdAsync(int id)
    {
        try
        {
            if (_db.Producers != null)
            {
                return await _db.Producers.FirstOrDefaultAsync(p => p.ProducerId == id);
            }
            return null;
        }
        catch (Exception e)
        {
            _logger.LogError("[ProducerRepository] items FirstOrDefaultAsync() failed when GetProducerByIdAsync() is called, error message: {e}", e.Message);
            return null;
        }
    }

    public async Task<bool> AddProducerAsync(Producer producer)
    {
        try
        {
            if (_db.Producers != null)
            {
                _db.Producers.Add(producer);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }
        catch (Exception e)
        {
            _logger.LogError("[ProducerRepository] items Add() failed when AddProducerAsync() is called, error message: {e}", e.Message);
            return false;
        }
    }

    public async Task<bool> UpdateProducerAsync(Producer producer)
    {
        try
        {
            if (_db.Producers != null)
            {
                _db.Producers.Update(producer);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }
        catch (Exception e)
        {
            _logger.LogError("[ProducerRepository] items Update() failed when UpdateProducerAsync() is called, error message: {e}", e.Message);
            return false;
        }
    }

    public async Task<bool> DeleteProducerAsync(int id)
    {
        try
        {
            if (_db.Producers != null)
            {
                var producer = await _db.Producers.FirstOrDefaultAsync(p => p.ProducerId == id);
                if (producer == null)
                {
                    _logger.LogError("[ProducerRepository] item not found for the ProducedId {ProducerId:0000}", id);
                    return false;
                }
                _db.Producers.Remove(producer);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }
        catch (Exception e)
        {
            _logger.LogError("[ProducerRepository] item deletion failed for the ItemId {ItemId:0000}, error message: {e}", id, e.Message);
            return false;
        }
    }
}