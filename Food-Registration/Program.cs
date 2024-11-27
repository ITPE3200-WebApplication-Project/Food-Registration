using Microsoft.EntityFrameworkCore;
using Food_Registration.Models;
using Microsoft.AspNetCore.Identity;
using Food_Registration.DAL;


var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ItemDbContextConnection") ?? throw new InvalidOperationException("Connection string 'ItemDbContextConnection' not found.");

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ItemDbContext>(options =>
{
  options.UseSqlite(
      builder.Configuration["ConnectionStrings:ItemDbContextConnection"]);
});

// Registers services for DAL
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProducerRepository, ProducerRepository>();

builder.Services.AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<ItemDbContext>();

builder.Services.AddRazorPages();
builder.Services.AddSession();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Home/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}
if (app.Environment.IsDevelopment())
{
  DBInit.Seed(app);
}

app.UseAuthentication();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=AllProducts}/{id?}"
);

app.Run();
