using Microsoft.AspNetCore.Mvc;
using Food_Registration.Models;
using Food_Registration.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Food_Registration.DAL;

namespace Food_Registration.Controllers;


public class ProductController : Controller
{
  private readonly ProductDbContext _ProductDbContext;

  public ProductController(ProductDbContext ProductDbContext)
  {
    _ProductDbContext = ProductDbContext;
  }

  public async Task<IActionResult> AllProducts(string searching, string category)
  {
    if (_ProductDbContext?.Products == null)
    {
      return Problem("Entity set 'ProductDbContext.Products' is null.");
    }

    var productQuery = _ProductDbContext.Products.AsQueryable();

    if (!string.IsNullOrEmpty(searching))
    {
      // Use ToLower() to make the search case-insensitive
      productQuery = productQuery.Where(x => x.Name.ToLower().Contains(searching.ToLower()) || x.ProductId.ToString().ToLower().Contains(searching.ToLower()));
    }

    // Legg til kategorifiltrering hvis en kategori er spesifisert
    if (!string.IsNullOrEmpty(category))
    {
      productQuery = productQuery.Where(x => x.Category != null && x.Category.ToLower() == category.ToLower());
    }

    var products = await productQuery.AsNoTracking().ToListAsync();

    return View("~/Views/Product/AllProducts.cshtml", products);
  }

  public async Task <IActionResult> ReadMore(int id)
  {
    if (_ProductDbContext?.Products == null)
    {
      return Problem("Entity set 'ProductDbContext.Products' is null.");
    }

    var product = await _ProductDbContext.Products
      .Include(p => p.Producer)
      .FirstOrDefaultAsync(p => p.ProductId == id);

    if (product == null)
    {
      return NotFound();
    }

    return View(product);
  }

  [Authorize]
  public async Task <IActionResult> Table()
  {
    var currentUserId = User.Identity?.Name;

    if (string.IsNullOrEmpty(currentUserId))
    {
      return RedirectWithMessage("Producer", "Table", "Please create a producer account first", "warning");
    }

    // Sjekk om Producers er null
    if (_ProductDbContext.Producers == null)
    {
        return Problem("Entity set 'ProductDbContext.Producers' is null.");
    }

    // Get all producers owned by current user
    var userProducerIds = await _ProductDbContext.Producers
        .Where(p => p.OwnerId == currentUserId)
        .Select(p => p.ProducerId)
        .ToListAsync();

    if (userProducerIds == null || !userProducerIds.Any())
    {
      return RedirectWithMessage("Producer", "Table", "Please create a producer account first", "warning");
    }

    // Sjekk om Products er null
    if (_ProductDbContext.Products == null)
    {
        return Problem("Entity set 'ProductDbContext.Products' is null.");
    }

    // Get all products that belong to the user's producers
    var products = await _ProductDbContext.Products
        .Include(p => p.Producer)
        .Where(p => userProducerIds.Contains(p.ProducerId))
        .ToListAsync();

    
    var viewModel = new ProductsViewModel(
        products, // ?? new List<Product>(), Tror ikke denne er nødvendig etter å ha sjekket om null tidligere, derfor er new list kommentert ut
        "Table"
    );

    return View(viewModel);
  }

  [Authorize]
  public async Task <IActionResult> NewProduct()
  {
    var currentUserId = User.Identity?.Name;

    if (string.IsNullOrEmpty(currentUserId))
    {
      return Redirect($"/Producer/Table?error={Uri.EscapeDataString("Please create a producer account first")}");
    }

    // Get producers owned by current user
    var userProducers = await _ProductDbContext.Producers?
        .Where(p => p.OwnerId == currentUserId)
        .ToListAsync();

    if (userProducers == null || !userProducers.Any())
    {
      return Redirect($"/Producer/Table?error={Uri.EscapeDataString("Please create a producer account first")}");
    }

    // Create SelectList for producers dropdown
    ViewBag.Producers = new SelectList(userProducers, "ProducerId", "Name");

    // Create SelectList for categories dropdown
    var categories = new List<string>
        {
            "Fruits",
            "Vegetables",
            "Meat",
            "Fish",
            "Dairy",
            "Grains",
            "Beverages",
            "Snacks",
            "Other"
        };
    ViewBag.Categories = new SelectList(categories);

    return View();
  }

  [Authorize]
  [HttpPost]
  public async Task <IActionResult> NewProduct(Product Products)
  {
    if (ModelState.IsValid && _ProductDbContext != null)
    {
      _ProductDbContext?.Products?.Update(Products);
      await _ProductDbContext?.SaveChangesAsync();
      return RedirectToAction(nameof(Table));
    }
    return View(Products);
  }


  [HttpGet]
  [Authorize]
  public async Task <IActionResult> Edit(int id)
  {
    var product = await _ProductDbContext.Products?.FirstOrDefaultAsync(p => p.ProductId == id);
    if (product == null)
    {
      return NotFound();
    }

    // Create SelectList for categories dropdown
    var categories = new List<string>
        {
            "Fruits",
            "Vegetables",
            "Meat",
            "Fish",
            "Dairy",
            "Grains",
            "Beverages",
            "Snacks",
            "Other"
        };
    ViewBag.Categories = new SelectList(categories);

    var currentUserId = User.Identity?.Name;

    if (_ProductDbContext.Producers == null)
    {
      return Problem("Entity set 'ProductDbContext.Products' is null.");
    }
    ViewBag.ProducerList = new SelectList(
        _ProductDbContext.Producers.Where(p => p.OwnerId == currentUserId),
        "ProducerId",
        "Name",
        product.ProducerId
    );

    return View(product); // View Only product taht is choiced
  }

  [HttpPost]
  [Authorize]
  public async Task <IActionResult> Edit(Product product)
  {
    if (ModelState.IsValid)
    {
      if (_ProductDbContext.Products == null)
      {
        return Problem("Entity set 'ProductDbContext.Products' is null.");
      }

      var existingProduct = await _ProductDbContext.Products.FindAsync(product.ProductId);
      if (existingProduct == null)
      {
        return NotFound();
      }

      // Update the existing product's properties
      _ProductDbContext.Entry(existingProduct).CurrentValues.SetValues(product);
      await _ProductDbContext.SaveChangesAsync();

      return RedirectWithMessage("Product", nameof(Table), "Product updated", "info");

    }
    return View(product);
  }

  [HttpPost]
  [Authorize]
  public async Task <IActionResult> DeleteConfirmed(int id)
  {
    if (_ProductDbContext.Products == null)
    {
      return Problem("Entity set 'ProductDbContext.Products' is null.");
    }

    var item = await _ProductDbContext.Products.FindAsync(id);
    if (item == null)
    {
      return NotFound();
    }
    _ProductDbContext.Products.Remove(item);
    await _ProductDbContext.SaveChangesAsync();

    return RedirectWithMessage("Product", "Table", "Product successfully deleted", "success");
  }

  private IActionResult RedirectWithMessage(string controller, string action, string message, string messageType = "info")
  {
    return Redirect($"/{controller}/{action}?message={Uri.EscapeDataString(message)}&messageType={messageType}");
  }
}
