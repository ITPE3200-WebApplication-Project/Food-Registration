using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Food_Registration.Models;

namespace Food_Registration.DAL;

public class ItemDbContext : IdentityDbContext
{
  public ItemDbContext(DbContextOptions<ItemDbContext> options) : base(options)
  {
    // 

  }
  public DbSet<Product>? Products { get; set; } // Nullable
  public DbSet<Producer>? Producers { get; set; } // Nullable
}