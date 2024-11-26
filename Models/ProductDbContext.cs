using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Food_Registration.Models;

public class ProductDbContext : IdentityDbContext
{
  public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
  {
    // 
    
  }
  public DbSet<Product>? Products { get; set; } // Nullable
  public DbSet<Producer>? Producers { get; set; } // Nullable
}