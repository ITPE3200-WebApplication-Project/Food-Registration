using Microsoft.AspNetCore.Mvc;
using Food_Registration.Models;
using Food_Registration.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Food_Registration.DAL;
using Microsoft.Extensions.Logging; 


namespace Food_Registration.Controllers;


public class ProductController : Controller
{
  private readonly IProductRepository _productRepository;

  private readonly IProducerRepository _producerRepository;

  private readonly IWebHostEnvironment _webHostEnvironment;

   private readonly ILogger<ProductController> _logger;
  public ProductController(IProductRepository productRepository, IProducerRepository producerRepository, IWebHostEnvironment webHostEnvironment, ILogger<ProductController> logger)
  {
    _productRepository = productRepository;
    _producerRepository = producerRepository;
    _webHostEnvironment = webHostEnvironment;
     _logger = logger;
  }

  [HttpGet]
  public async Task<IActionResult> AllProducts(string searching, string category)
  {
    var products = await _productRepository.GetAllProductsAsync();

    if (products == null)
    {
      _logger.LogError("[ProductController] products not found while executing _productRepository.GetAllProductsAsync()");
      return NotFound("Products not found");
    }

    // Default image
    foreach (var product in products)
    {
      if (product.ImageUrl == null || product.ImageUrl == "")
      {
        product.ImageUrl = "https://mtek3d.com/wp-content/uploads/2018/01/image-placeholder-500x500.jpg";
      }
    }

    // Filter products based on search
    if (!string.IsNullOrEmpty(searching))
    {
      products = products.Where(x =>
          x.Name.ToLower().Contains(searching.ToLower()) ||
          x.ProductId.ToString().Contains(searching.ToLower()));
    }

    // Filter products based on category
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
        _logger.LogError($"[ProductController] Product with id {id} not found in ReadMore");
        return RedirectWithMessage("Product", "Table", "Product not found", "error");
    }

    // Add default image if none exists
    if (string.IsNullOrEmpty(product.ImageUrl))
    {
        product.ImageUrl = "https://mtek3d.com/wp-content/uploads/2018/01/image-placeholder-500x500.jpg";
    }

    // Ensure Producer is loaded
    if (product.Producer == null)
    {
        _logger.LogError($"[ProductController] Producer not loaded for product {id} in ReadMore");
        return RedirectWithMessage("Product", "Table", "Product details not available", "error");
    }

    // Add default producer image if none exists
    if (string.IsNullOrEmpty(product.Producer.ImageUrl))
    {
        product.Producer.ImageUrl = "https://mtek3d.com/wp-content/uploads/2018/01/image-placeholder-500x500.jpg";
    }

    return View(product);
  }

  [Authorize]
  [HttpGet]
  public async Task<IActionResult> Table()
  {
    var currentUserId = User.Identity?.Name;
    _logger.LogInformation($"[ProductController] Current user: {currentUserId}");

    if (string.IsNullOrEmpty(currentUserId))
    {
        return RedirectWithMessage("Producer", "Table", "Please create a producer account first", "warning");
    }

    var allProducts = await _productRepository.GetAllProductsAsync();
    _logger.LogInformation($"[ProductController] Total products found: {allProducts?.Count() ?? 0}");

    var filteredProducts = allProducts?
        .Where(p => p.Producer?.OwnerId == currentUserId)
        .ToList();
    _logger.LogInformation($"[ProductController] Filtered products for user: {filteredProducts?.Count ?? 0}");

    return View(filteredProducts);
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
    var currentUserId = User.Identity?.Name;
    
    if (!ModelState.IsValid)
    {
      // Repopulate all dropdown data before returning the view
      
      // Repopulate producers dropdown
      var userProducers = (await _producerRepository.GetAllProducersAsync())
          .Where(p => p.OwnerId == currentUserId)
          .ToList();
      ViewBag.Producers = new SelectList(userProducers, "ProducerId", "Name");

      // Repopulate categories dropdown
      var categories = new List<string>
      {
          "Fruits", "Vegetables", "Meat", "Fish", "Dairy", 
          "Grains", "Beverages", "Snacks", "Other"
      };
      ViewBag.Categories = new SelectList(categories);

      // Repopulate nutrition scores dropdown
      var nutritionScores = new List<string> { "A", "B", "C", "D", "E" };
      ViewBag.NutritionScores = new SelectList(nutritionScores);

      return View(product);
    }

    var producer = await _producerRepository.GetProducerByIdAsync(product.ProducerId);
    if (producer == null)
    {
      _logger.LogError("[ProductController] producer not found while executing _producerRepository.GetProducerByIdAsync()");
      return RedirectWithMessage("Product", "NewProduct", "Please select a producer", "warning");
    }

    // Check if user owns the producer
    if (producer.OwnerId != currentUserId)
    {
      _logger.LogError("[ProductController] producer does not belong to current user while executing _producerRepository.GetProducerByIdAsync()");
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
      _logger.LogError("[ProductController] product not found while executing _productRepository.GetProductByIdAsync()");
      return BadRequest("Product not found");
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
    _logger.LogInformation($"[ProductController] Editing product {product.ProductId}");

    if (!ModelState.IsValid)
    {
        // Repopulate dropdowns before returning
        await PopulateDropDowns();
        return View(product);
    }

    var existingProduct = await _productRepository.GetProductByIdAsync(product.ProductId);
    if (existingProduct == null)
    {
        return RedirectWithMessage("Product", "Table", "Product not found", "error");
    }

    // Verify ownership
    var currentUserId = User.Identity?.Name;
    var producer = await _producerRepository.GetProducerByIdAsync(existingProduct.ProducerId);
    if (producer?.OwnerId != currentUserId)
    {
        return RedirectWithMessage("Product", "Table", "You can only edit your own products", "error");
    }

    // Update all fields
    existingProduct.Name = product.Name;
    existingProduct.Description = product.Description;
    existingProduct.Category = product.Category;
    existingProduct.NutritionScore = product.NutritionScore;
    existingProduct.Calories = product.Calories;
    existingProduct.Carbohydrates = product.Carbohydrates;
    existingProduct.Fat = product.Fat;
    existingProduct.Protein = product.Protein;
    existingProduct.ProducerId = product.ProducerId;

    var success = await _productRepository.UpdateProductAsync(existingProduct);
    if (!success)
    {
        return RedirectWithMessage("Product", "Table", "Failed to update product", "error");
    }

    return RedirectToAction(nameof(Table));
  }

  private async Task PopulateDropDowns()
  {
    var currentUserId = User.Identity?.Name;
    var producers = (await _producerRepository.GetAllProducersAsync())
        .Where(p => p.OwnerId == currentUserId)
        .ToList();

    ViewBag.ProducerList = new SelectList(producers, "ProducerId", "Name");
    ViewBag.Categories = new SelectList(new List<string>
    {
        "Fruits", "Vegetables", "Meat", "Fish", "Dairy", 
        "Grains", "Beverages", "Snacks", "Other"
    });
    ViewBag.NutritionScores = new SelectList(new List<string> { "A", "B", "C", "D", "E" });
  }

  [HttpPost]
  [Authorize]
  public async Task<IActionResult> DeleteConfirmed(int id)
  {
    // Get the product
    var product = await _productRepository.GetProductByIdAsync(id);
    if (product == null)
    {
      _logger.LogError("[ProductController] product not found while executing _productRepository.GetProductByIdAsync()");
      return NotFound();
    }

    // Get the producer that owns the product
    var producer = await _producerRepository.GetProducerByIdAsync(product.ProducerId);
    if (producer == null)
    {
      _logger.LogError("[ProductController] producer not found while executing _producerRepository.GetProducerByIdAsync()");
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