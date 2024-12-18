using Microsoft.EntityFrameworkCore;
using Food_Registration.Models;
using Microsoft.AspNetCore.Identity;
using Food_Registration.DAL;
using Serilog;
using Serilog.Events;

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

// Logging configuration.
var loggerConfiguration = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File($"Logs/app_{DateTime.Now:yyyyMMdd_HHmmss}.log");

// Creates logger.
var logger = loggerConfiguration.CreateLogger();
builder.Logging.AddSerilog(logger);

// Makes logging less verbos.
loggerConfiguration.Filter.ByExcluding(e => e.Properties.TryGetValue("SourceContext", out var value) &&
e.Level == LogEventLevel.Information &&
e.MessageTemplate.Text.Contains("Executed DbCommand"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Product/Error");
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
    pattern: "{controller=Product}/{action=Index}/{id?}"
);

app.Run();
