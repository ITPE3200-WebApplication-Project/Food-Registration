using Microsoft.AspNetCore.Mvc;
using Food_Registration.Models;
using Food_Registration.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Food_Registration.DAL;

namespace Food_Registration.Controllers;


public class ProductController : Controller
{
  private readonly IProductRepository _productRepository;
  private readonly IProducerRepository _producerRepository;
  public ProductController(IProductRepository productRepository, IProducerRepository producerRepository)
  {
    _productRepository = productRepository;
    _producerRepository = producerRepository;
  }

  public async Task<IActionResult> AllProducts(string searching, string category)
  {
    var products = await _productRepository.GetAllProductsAsync();

    if (!string.IsNullOrEmpty(searching))
    {
        products = products.Where(x => 
            x.Name.ToLower().Contains(searching.ToLower()) || 
            x.ProductId.ToString().Contains(searching.ToLower()));
    }

    if (!string.IsNullOrEmpty(category))
    {
        products = products.Where(x => 
            x.Category != null && x.Category.ToLower() == category.ToLower());
    }

    // Materialiser som en liste
    var productList = products.ToList();

    return View("~/Views/Product/AllProducts.cshtml", productList);
  }

  public async Task<IActionResult> ReadMore(int id)
  {
    var product = await _productRepository.GetProductByIdAsync(id);

    if (product == null)
    {
        return NotFound();
    }

    return View(product);
  }

  [Authorize]
  public async Task<IActionResult> Table()
  {
    var currentUserId = User.Identity?.Name;
Console.WriteLine($"currentUserId: {currentUserId}");

    if (string.IsNullOrEmpty(currentUserId))
    {
        Console.WriteLine("Nr 1 blir kalt");
        return RedirectWithMessage("Producer", "Table", "Please create a producer account first", "warning");
    }

    // Hent alle produkter inkludert produsenter
    var allProducts = await _productRepository.GetAllProductsAsync();

    // Filtrer produkter basert på OwnerId
    var products = allProducts.Where(p => p.Producer?.OwnerId == currentUserId).ToList();

    var viewModel = new ProductsViewModel(products, "Table");

    return View(viewModel);
  }

  [Authorize]
  public async Task <IActionResult> NewProduct()
  {
    var currentUserId = User.Identity?.Name;
    Console.WriteLine($"currentUserId: {currentUserId}");

    if (string.IsNullOrEmpty(currentUserId))
    {
      Console.WriteLine("Havner i nr2");
      return Redirect($"/Producer/Table?error={Uri.EscapeDataString("Please create a producer account first")}");
    }

    // Get producers owned by current user
    var userProducers = (await _producerRepository.GetAllProducersAsync())
        .Where(p => p.OwnerId == currentUserId)
        .ToList();

    if (userProducers == null || !userProducers.Any())
    {
      Console.WriteLine("Havner i nr3");
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
  public async Task<IActionResult> NewProduct(Product product)
  {
    if (ModelState.IsValid)
    {
        await _productRepository.AddProductAsync(product);
        return RedirectToAction(nameof(Table));
    }

    return View(product);
  }


  [Authorize]
  [HttpGet]
  public async Task<IActionResult> Edit(int id)
  {
    var product = await _productRepository.GetProductByIdAsync(id);

    if (product == null)
    {
        return NotFound();
    }

    var producers = await _producerRepository.GetAllProducersAsync(); // Hent produsenter fra databasen

    ViewBag.Categories = new SelectList(new List<string>
    {
        "Fruits", "Vegetables", "Meat", "Fish", "Dairy", "Grains", "Beverages", "Snacks", "Other"
    });

    ViewBag.ProducerList = new SelectList(producers, "ProducerId", "Name", product.ProducerId);

    return View(product);
  }

  [Authorize]
  [HttpPost]
  public async Task<IActionResult> Edit(Product product)
  {
    if (ModelState.IsValid)
    {
        await _productRepository.UpdateProductAsync(product);
        return RedirectWithMessage("Product", nameof(Table), "Product updated", "info");
    }

  return View(product);
  }

  [HttpPost]
  [Authorize]
  public async Task<IActionResult> DeleteConfirmed(int id)
  {
    await _productRepository.DeleteProductAsync(id);
    return RedirectWithMessage("Product", "Table", "Product successfully deleted", "success");
  }

  private IActionResult RedirectWithMessage(string controller, string action, string message, string messageType = "info")
  {
    return Redirect($"/{controller}/{action}?message={Uri.EscapeDataString(message)}&messageType={messageType}");
  }
}