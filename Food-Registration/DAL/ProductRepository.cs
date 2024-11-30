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
            _logger.LogError("[ProductRepository] Failed to update product: {e}", e.Message);
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
                    _logger.LogError("[ProductRepository] item not found for ProductId {id}", id);  
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
    }

    public async Task<IEnumerable<Product>> GetFilteredProductsAsync(string? search, string? category)
    {
        try
        {
            var products = await GetAllProductsAsync();
            
            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(x =>
                    x.Name.ToLower().Contains(search.ToLower()) ||
                    x.ProductId.ToString().Contains(search.ToLower()));
            }

            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(x =>
                    x.Category != null && x.Category.ToLower() == category.ToLower());
            }

            return products;
        }
        catch (Exception e)
        {
            _logger.LogError("[ProductRepository] GetFilteredProductsAsync failed: {e}", e.Message);
            return new List<Product>();
        }
    }

    public async Task<string?> SaveProductImageAsync(IFormFile file, string wwwRootPath)
    {
        try
        {
            if (file == null || file.Length == 0) return null;

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string productPath = Path.Combine(wwwRootPath, "images", "product");
            Directory.CreateDirectory(productPath);
            
            string filePath = Path.Combine(productPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            
            return "/images/product/" + fileName;
        }
        catch (Exception e)
        {
            _logger.LogError("[ProductRepository] SaveProductImageAsync failed: {e}", e.Message);
            return null;
        }
    }

    public async Task<bool> DeleteProductImageAsync(string? imagePath, string wwwRootPath)
    {
        try
        {
            if (string.IsNullOrEmpty(imagePath)) return true;

            string fullPath = Path.Combine(wwwRootPath, imagePath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
            {
                await Task.Run(() => System.IO.File.Delete(fullPath));
            }
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("[ProductRepository] DeleteProductImageAsync failed: {e}", e.Message);
            return false;
        }
    }

    public async Task<IEnumerable<Product>> GetProductsByProducerIdAsync(int producerId)
    {
        try
        {
            if (_db.Products != null)
            {
                return await _db.Products
                    .Where(p => p.ProducerId == producerId)
                    .ToListAsync();
            }
            return new List<Product>();
        }
        catch (Exception e)
        {
            _logger.LogError("[ProductRepository] GetProductsByProducerIdAsync failed: {e}", e.Message);
            return new List<Product>();
        }
    }
}