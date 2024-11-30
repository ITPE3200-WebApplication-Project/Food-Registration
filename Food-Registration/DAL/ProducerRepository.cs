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

    // Get all producers from database
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
        }
    }

    // Find a specific producer by ID
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

    // Add a new producer to database
    public async Task<bool> CreateProducerAsync(Producer producer)
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
            _logger.LogError("[ProducerRepository] items Add() failed when CreateProducerAsync() is called, error message: {e}", e.Message);
            return false;
        }
    }

    // Update existing producer in database
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

    // Remove producer from database
    public async Task<bool> DeleteProducerAsync(int id)
    {
        try
        {
            if (_db.Producers != null)
            {
                // Find the producer in the database
                var producer = await _db.Producers.FirstOrDefaultAsync(p => p.ProducerId == id);
                if (producer == null)
                {
                    _logger.LogError("[ProducerRepository] item not found for ProducerId {id}", id);
                    return false;
                }
                // Remove the producer and save changes
                _db.Producers.Remove(producer);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }
        catch (Exception e)
        {
            _logger.LogError("[ProducerRepository] deletion failed for ProducerId {id}: {e}", id, e.Message);
            return false;
        }
    }

    // Check if producer has any associated products
    public async Task<bool> HasAssociatedProductsAsync(int producerId)
    {
        try
        {
            if (_db.Products != null)
            {
                return await _db.Products.AnyAsync(p => p.ProducerId == producerId);
            }
            return false;
        }
        catch (Exception e)
        {
            _logger.LogError("[ProducerRepository] HasAssociatedProductsAsync failed: {e}", e.Message);
            return false;
        }
    }

    // Save producer image to file system and return path
    public async Task<string?> SaveProducerImageAsync(IFormFile file, string wwwRootPath)
    {
        try
        {
            // Check if file exists and has content
            if (file == null || file.Length == 0) return null;

            // Generate unique filename and create directory path
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string producerPath = Path.Combine(wwwRootPath, "images", "producer");
            Directory.CreateDirectory(producerPath);
            
            // Save file to disk
            string filePath = Path.Combine(producerPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            
            // Return relative path
            return "/images/producer/" + fileName;
        }
        catch (Exception e)
        {
            _logger.LogError("[ProducerRepository] SaveProducerImageAsync failed: {e}", e.Message);
            return null;
        }
    }


    // Delete producer image
    public async Task<bool> DeleteProducerImageAsync(string? imagePath, string wwwRootPath)
    {
        try
        {
            // Return true if no image path provide
            if (string.IsNullOrEmpty(imagePath)) return true;

            // Construct full path to the image
            string fullPath = Path.Combine(wwwRootPath, imagePath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
            {
                await Task.Run(() => System.IO.File.Delete(fullPath));
            }
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("[ProducerRepository] DeleteProducerImageAsync failed: {e}", e.Message);
            return false;
        }
    }
}