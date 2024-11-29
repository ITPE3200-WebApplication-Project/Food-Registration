using Microsoft.EntityFrameworkCore;
using Food_Registration.Models;

namespace Food_Registration.DAL;

public class ProductRepository : IProductRepository
{
    private readonly ItemDbContext _db;

    private readonly ILogger<ProductRepository> _logger;

    public ProductRepository(ItemDbContext db, ILogger<ProductRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        try
        {
            if (_db.Products != null)
            {
                return await _db.Products.Include(p => p.Producer).ToListAsync();
            }
            return new List<Product>();
        }
        catch (Exception e)
        {
            _logger.LogError("[ProductRepository] GetAllProductsAsync failed: {e}", e.Message);
            return new List<Product>();
        }
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        try
        {
            if (_db.Products == null) return null;
            return await _db.Products
                .Include(p => p.Producer)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }
        catch (Exception e)
        {
            _logger.LogError("[ProductRepository] Error getting product by id {id}: {e}", id, e.Message);
            return null;
        }
    }

    public async Task<bool> CreateProductAsync(Product product)
    {
        try
        {
            if (_db.Products != null)
            {
                _db.Products.Add(product);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }
        catch (Exception e)
        {
            _logger.LogError("[ProductRepository] items Add() failed when CreateProductAsync() is called, error message: {e}", e.Message);
            return false;
        }
        /*
        // Check if the Products entity set is null
        if (_db.Products == null)
        {
            return;
        }

        // Check if all required fields are filled
        if (string.IsNullOrEmpty(product.Name) || product?.ProducerId == null || string.IsNullOrEmpty(product.NutritionScore))
        {
            return;
        }

        // Add the product to the database
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        */
    }

    public async Task<bool> UpdateProductAsync(Product product)
    {
        try
        {
            if (_db.Products != null)
            {
                _db.Products.Update(product);
                return await _db.SaveChangesAsync() > 0;
            }
            return false;
        }
        catch (Exception e)
        {
            _logger.LogError($"[ProductRepository] Failed to update product: {e.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        try
        {
            if (_db.Products != null)
            {
                var product = await _db.Products.FirstOrDefaultAsync(p => p.ProductId == id);
                if (product == null)
                {
                    _logger.LogError("[ProductRepository] item not found for the ProductId {ProductId:0000}", id);  
                    return false;
                }
                _db.Products.Remove(product);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }
        catch (Exception e)
        {
            _logger.LogError("[ProductRepository] items Remove() failed when DeleteProductAsync() is called, error message: {e}", e.Message);
            return false;
        }
        /*
        // Check if the Products entity set is null
        if (_db.Products == null)
        {
            return;
        }
        var product = await _db.Products.FirstOrDefaultAsync(p => p.ProductId == id);
        if (product != null)
        {
            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
        }
        */
    }
}