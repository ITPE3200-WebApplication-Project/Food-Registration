using Microsoft.EntityFrameworkCore;
using Food_Registration.Models;
using Microsoft.AspNetCore.Identity;
using Food_Registration.DAL;
using Serilog;
using Serilog.Events;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

// Add CORS support
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowReactApp",
      builder => builder
          .WithOrigins("http://localhost:5174", "http://localhost:4173") // React dev server
          .AllowAnyMethod()
          .AllowAnyHeader());
});

// JWT Configuration
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
  throw new InvalidOperationException("JWT key is not configured in appsettings.json");
}

builder.Services.AddAuthentication(options =>
{
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
  options.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = builder.Configuration["Jwt:Issuer"],
    ValidAudience = builder.Configuration["Jwt:Audience"],
    IssuerSigningKey = new SymmetricSecurityKey(
          Encoding.UTF8.GetBytes(jwtKey))
  };
});

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

// Enable CORS before routing and authorization
app.UseCors("AllowReactApp");

app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Index}/{id?}"
);

app.Run();
