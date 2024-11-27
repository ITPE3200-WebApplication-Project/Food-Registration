using Microsoft.EntityFrameworkCore;
using Food_Registration.Models;

namespace Food_Registration.DAL;

public class ProductRepository : IProductRepository
{
    private readonly ItemDbContext _db;

    public ProductRepository(ItemDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        // Check if the Products entity set is null
        if (_db.Products == null)
        {
            return new List<Product>();
        }

        var products = await _db.Products
            .Include(p => p.Producer)
            .ToListAsync();

        return products;
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        // Check if the Products entity set is null
        if (_db.Products == null)
        {
            return null;
        }
        return await _db.Products.Include(p => p.Producer).FirstOrDefaultAsync(p => p.ProductId == id);
    }

    public async Task AddProductAsync(Product product)
    {
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
    }

    public async Task UpdateProductAsync(Product product)
    {
        // Check if the Products entity set is null
        if (_db.Products == null)
        {
            return;
        }
        _db.Products.Update(product);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(int id)
    {
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
    }
}