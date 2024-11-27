using Microsoft.EntityFrameworkCore;
using Food_Registration.Models;

namespace Food_Registration.DAL;

public class ProductRepository : IProductRepository
{
    private readonly ProductDbContext _db;

    public ProductRepository(ProductDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
{
    var products = await _db.Products
        .Include(p => p.Producer)
        .ToListAsync();
    return products;
}

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _db.Products.FirstOrDefaultAsync(p => p.ProductId == id);
    }

    public async Task AddProductAsync(Product product)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateProductAsync(Product product)
    {
        _db.Products.Update(product);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.ProductId == id);
        if (product != null)
        {
            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
        }
    }
}