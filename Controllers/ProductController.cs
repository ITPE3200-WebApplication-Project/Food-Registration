using Microsoft.AspNetCore.Mvc;
using Food_Registration.Models;
using Food_Registration.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Food_Registration.Controllers;


public class ProductController : Controller
{
  private readonly ProductDbContext _ProductDbContext;

  public ProductController(ProductDbContext ProductDbContext)
  {
    _ProductDbContext = ProductDbContext;
  }

  async public Task<IActionResult> AllProducts(string searching, string category)
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

  public IActionResult ReadMore(int id)
  {
    if (_ProductDbContext?.Products == null)
    {
      return Problem("Entity set 'ProductDbContext.Products' is null.");
    }

    var product = _ProductDbContext.Products
      .Include(p => p.Producer)
      .FirstOrDefault(p => p.ProductId == id);

    if (product == null)
    {
      return NotFound();
    }

    return View(product);
  }

  [Authorize]
  public IActionResult Table()
  {
    var currentUserId = User.Identity?.Name;

    if (string.IsNullOrEmpty(currentUserId))
    {
      return RedirectWithMessage("Producer", "Index", "Please create a producer account first", "warning");
    }

    // Get all producers owned by current user
    var userProducerIds = _ProductDbContext.Producers?
        .Where(p => p.OwnerId == currentUserId)
        .Select(p => p.ProducerId)
        .ToList();

    if (userProducerIds == null || !userProducerIds.Any())
    {
      return RedirectWithMessage("Producer", "Index", "Please create a producer account first", "warning");
    }

    // Get all products that belong to the user's producers
    var products = _ProductDbContext.Products?
        .Where(p => userProducerIds.Contains(p.ProducerId))
        .ToList();

    var viewModel = new ProductsViewModel(
        products ?? new List<Product>(),
        "Table"
    );

    return View(viewModel);
  }

  [Authorize]
  public IActionResult NewProduct()
  {
    var currentUserId = User.Identity?.Name;

    if (string.IsNullOrEmpty(currentUserId))
    {
      return Redirect($"/Producer/Index?error={Uri.EscapeDataString("Please create a producer account first")}");
    }

    // Get producers owned by current user
    var userProducers = _ProductDbContext.Producers?
        .Where(p => p.OwnerId == currentUserId)
        .ToList();

    if (userProducers == null || !userProducers.Any())
    {
      return Redirect($"/Producer/Index?error={Uri.EscapeDataString("Please create a producer account first")}");
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
  public IActionResult NewProduct(Product Products)
  {
    if (ModelState.IsValid && _ProductDbContext != null)
    {
      _ProductDbContext?.Products?.Update(Products);
      _ProductDbContext?.SaveChanges();
      return RedirectToAction(nameof(Index));
    }
    return View(Products);
  }


  [HttpGet]
  [Authorize]
  public IActionResult Edit(int id)
  {
    var product = _ProductDbContext.Products?.FirstOrDefault(p => p.ProductId == id);
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
  public IActionResult Edit(Product product)
  {
    if (ModelState.IsValid)
    {
      if (_ProductDbContext.Products == null)
      {
        return Problem("Entity set 'ProductDbContext.Products' is null.");
      }

      var existingProduct = _ProductDbContext.Products.Find(product.ProductId);
      if (existingProduct == null)
      {
        return NotFound();
      }

      // Update the existing product's properties
      _ProductDbContext.Entry(existingProduct).CurrentValues.SetValues(product);
      _ProductDbContext.SaveChanges();

      return RedirectWithMessage("Product", nameof(Table), "Product updated", "info");

    }
    return View(product);
  }

  [HttpGet]
  [Authorize]
  public IActionResult Delete(int id)
  {
    var item = _ProductDbContext.Products?.Find(id);
    if (item == null)
    {
      return NotFound();
    }
    return View(item);
  }

  [HttpPost]
  [Authorize]
  public IActionResult DeleteConfirmed(int id)
  {
    if (_ProductDbContext.Products == null)
    {
      return Problem("Entity set 'ProductDbContext.Products' is null.");
    }

    var item = _ProductDbContext.Products.Find(id);
    if (item == null)
    {
      return NotFound();
    }
    _ProductDbContext.Products.Remove(item);
    _ProductDbContext.SaveChanges();

    return RedirectWithMessage("Product", "Table", "Product successfully deleted", "success");
  }

  private IActionResult RedirectWithMessage(string controller, string action, string message, string messageType = "info")
  {
    return Redirect($"/{controller}/{action}?message={Uri.EscapeDataString(message)}&messageType={messageType}");
  }
}
