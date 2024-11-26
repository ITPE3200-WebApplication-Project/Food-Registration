using Microsoft.AspNetCore.Mvc;
using Food_Registration.Models;
using Food_Registration.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Food_Registration.Controllers;

[Authorize]
public class ProductController : Controller
{
  private readonly ProductDbContext _ProductDbContext;

  public ProductController(ProductDbContext ProductDbContext)
  {
    _ProductDbContext = ProductDbContext;
  }

  [Authorize]
  public IActionResult Table()
  {
    var currentUserId = User.Identity?.Name;

    if (string.IsNullOrEmpty(currentUserId))
    {
      return Redirect($"/Producer/Index?error={Uri.EscapeDataString("Please create a producer account first")}");
    }

    // Get all producers owned by current user
    var userProducerIds = _ProductDbContext.Producers?
        .Where(p => p.OwnerId == currentUserId)
        .Select(p => p.ProducerId)
        .ToList();

    if (userProducerIds == null || !userProducerIds.Any())
    {
      return Redirect($"/Producer/Index?error={Uri.EscapeDataString("Please create a producer account first")}");
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
  public IActionResult Grid()
  {
    List<Product> products = _ProductDbContext.Products?.ToList() ?? new List<Product>();
    var ProductsViewModel = new ProductsViewModel(products, "Grid");
    return View(ProductsViewModel);
  }

  public IActionResult Details(int id)
  {
    List<Product> Products = _ProductDbContext.Products?.ToList() ?? new List<Product>();
    var product = Products?.FirstOrDefault(i => i.ProductId == id);
    if (product == null)
      return NotFound();
    return View(product);
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

    return View(product); // View Only product taht is choiced
  }

  [HttpPost]
  public IActionResult Edit(Product Products)
  {
    if (ModelState.IsValid)
    {
      if (_ProductDbContext.Products == null)
      {
        return Problem("Entity set 'ProductDbContext.Products' is null.");
      }
      _ProductDbContext.Products.Update(Products);
      _ProductDbContext.SaveChanges();
      return RedirectToAction(nameof(Table));
    }
    return View(Products);//show if there is feil.
  }

  [HttpGet]
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

    //Set ViewBag.DeletionSuccess to true when the product is successfully deleted.
    ViewBag.DeletionSuccess = true;

    return RedirectToAction(nameof(Table));
  }
}
