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
            var products = await _db.Products
                .Include(p => p.Producer)
                .ToListAsync();

            _logger.LogInformation($"[ProductRepository] Found {products.Count} products in database");
            foreach (var p in products)
            {
                _logger.LogInformation($"Product: {p.Name}, ProducerId: {p.ProducerId}, Producer: {p.Producer?.Name}, OwnerId: {p.Producer?.OwnerId}");
            }

            return products;
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
            return await _db.Products
                .Include(p => p.Producer)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }
        catch (Exception e)
        {
            _logger.LogError($"[ProductRepository] Error getting product by id {id}: {e.Message}");
            return null;
        }
    }

    public async Task<bool> AddProductAsync(Product product)
    {
        try
        {
            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("[ProductRepository] items Add() failed when AddProductAsync() is called, error message: {e}", e.Message);
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
            _logger.LogInformation($"[ProductRepository] Updating product {product.ProductId} with NutritionScore: {product.NutritionScore}");
            
            var existingProduct = await _db.Products
                .Include(p => p.Producer)
                .FirstOrDefaultAsync(p => p.ProductId == product.ProductId);

            if (existingProduct == null)
            {
                _logger.LogError($"[ProductRepository] Product {product.ProductId} not found");
                return false;
            }

            // Update all fields
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Category = product.Category;
            existingProduct.NutritionScore = product.NutritionScore;
            existingProduct.Calories = product.Calories;
            existingProduct.Carbohydrates = product.Carbohydrates;
            existingProduct.Fat = product.Fat;
            existingProduct.Protein = product.Protein;
            existingProduct.ProducerId = product.ProducerId;

            await _db.SaveChangesAsync();
            
            _logger.LogInformation($"[ProductRepository] Successfully updated product {product.ProductId}");
            return true;
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