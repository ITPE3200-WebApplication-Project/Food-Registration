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

  [HttpGet]
  public async Task<IActionResult> AllProducts(string searching, string category)
  {
    var products = await _productRepository.GetAllProductsAsync();

    // Default image
    foreach (var product in products)
    {
      if (product.ImageUrl == null || product.ImageUrl == "")
      {
        product.ImageUrl = "https://mtek3d.com/wp-content/uploads/2018/01/image-placeholder-500x500.jpg";
      }
    }

    // Filter products based on search and category
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

    // Convert the filtered products to a list
    var productList = products.ToList();

    return View("~/Views/Product/AllProducts.cshtml", productList);
  }

  [HttpGet]
  public async Task<IActionResult> ReadMore(int id)
  {
    var product = await _productRepository.GetProductByIdAsync(id);

    if (product == null)
    {
      return RedirectWithMessage("Product", "AllProducts", "Product not found", "warning");
    }

    if (product.ImageUrl == null || product.ImageUrl == "")
    {
      product.ImageUrl = "https://mtek3d.com/wp-content/uploads/2018/01/image-placeholder-500x500.jpg";
    }

    return View(product);
  }

  [Authorize]
  [HttpGet]
  public async Task<IActionResult> Table()
  {
    var currentUserId = User.Identity?.Name;

    if (string.IsNullOrEmpty(currentUserId))
    {
      return RedirectWithMessage("Producer", "Table", "Please create a producer account first", "warning");
    }

    // Get all products including producers
    var allProducts = await _productRepository.GetAllProductsAsync();

    // Only get products owned by current user
    var products = allProducts.Where(p => p.Producer?.OwnerId == currentUserId).ToList();

    var viewModel = new ProductsViewModel(products, "Table");

    return View(viewModel);
  }

  [Authorize]
  [HttpGet]
  public async Task<IActionResult> NewProduct()
  {
    var currentUserId = User.Identity?.Name;

    if (string.IsNullOrEmpty(currentUserId))
    {
      return RedirectWithMessage("Producer", "Table", "Please create a producer account first", "warning");
    }

    // Get producers owned by current user
    var userProducers = (await _producerRepository.GetAllProducersAsync())
        .Where(p => p.OwnerId == currentUserId)
        .ToList();
    if (userProducers == null || !userProducers.Any())
    {
      return RedirectWithMessage("Producer", "Table", "Please create a producer account first", "warning");
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

    // Create SelectList for nutrition score
    var nutritionScores = new List<string>
        {
            "A",
            "B",
            "C",
            "D",
            "E"
        };
    ViewBag.NutritionScores = new SelectList(nutritionScores);

    return View();
  }

  [Authorize]
  [HttpPost]
  public async Task<IActionResult> NewProduct(Product product)
  {
    if (!ModelState.IsValid)
    {
      return View(product);
    }

    var producer = await _producerRepository.GetProducerByIdAsync(product.ProducerId);
    if (producer == null)
    {
      return RedirectWithMessage("Product", "NewProduct", "Please select a producer", "warning");
    }

    // Check if user owns the producer
    var currentUserId = User.Identity?.Name;
    if (producer.OwnerId != currentUserId)
    {
      return RedirectWithMessage("Product", "NewProduct", "You are not the owner of this producer", "warning");
    }

    await _productRepository.AddProductAsync(product);
    return RedirectToAction(nameof(Table));
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

    // Get all producers
    var producers = await _producerRepository.GetAllProducersAsync();

    ViewBag.Categories = new SelectList(new List<string>
    {
        "Fruits", "Vegetables", "Meat", "Fish", "Dairy", "Grains", "Beverages", "Snacks", "Other"
    });

    ViewBag.ProducerList = new SelectList(producers, "ProducerId", "Name", product.ProducerId);
    ViewBag.NutritionScores = new SelectList(new List<string>
    {
      "A", "B", "C", "D", "E"
    });

    return View(product);
  }

  [Authorize]
  [HttpPost]
  public async Task<IActionResult> Edit(Product product)
  {
    if (!ModelState.IsValid)
    {
      return View(product);
    }

    // Get not-edited product
    var originalProduct = await _productRepository.GetProductByIdAsync(product.ProductId);
    if (originalProduct == null)
    {
      return NotFound();
    }

    // Check if user owns the producer (originalProduct)
    var producer = await _producerRepository.GetProducerByIdAsync(originalProduct.ProducerId);
    if (producer == null)
    {
      return RedirectWithMessage("Product", "Edit", "Please select a producer", "warning");
    }

    // Update product
    await _productRepository.UpdateProductAsync(product);
    return RedirectWithMessage("Product", nameof(Table), "Product updated", "info");
  }

  [HttpPost]
  [Authorize]
  public async Task<IActionResult> DeleteConfirmed(int id)
  {
    // Get the product
    var product = await _productRepository.GetProductByIdAsync(id);
    if (product == null)
    {
      return NotFound();
    }

    // Get the producer that owns the product
    var producer = await _producerRepository.GetProducerByIdAsync(product.ProducerId);
    if (producer == null)
    {
      return NotFound();
    }

    // Check if user owns the producer
    var currentUserId = User.Identity?.Name;
    if (producer.OwnerId != currentUserId)
    {
      return RedirectWithMessage("Product", "Table", "You are not the owner of this producer", "warning");
    }

    await _productRepository.DeleteProductAsync(id);
    return RedirectWithMessage("Product", "Table", "Product successfully deleted", "success");
  }

  private IActionResult RedirectWithMessage(string controller, string action, string message, string messageType = "info")
  {
    return Redirect($"/{controller}/{action}?message={Uri.EscapeDataString(message)}&messageType={messageType}");
  }
}