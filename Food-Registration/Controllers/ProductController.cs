using Microsoft.AspNetCore.Mvc;
using Food_Registration.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Food_Registration.DAL;

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

  // Display products
  [HttpGet]
  public async Task<IActionResult> Index(string? searching, string? category)
  {
    try
    {
      var products = await _productRepository.GetFilteredProductsAsync(searching, category);
      
      if (products == null)
      {
        _logger.LogError("[ProductController] products not found while executing GetFilteredProductsAsync()");
        return NotFound("Products not found");
      }

      // Set default image if none exists
      foreach (var product in products)
      {
        if (string.IsNullOrEmpty(product.ImageUrl))
        {
          product.ImageUrl = "https://mtek3d.com/wp-content/uploads/2018/01/image-placeholder-500x500.jpg";
        }
      }

      ViewData["Getproductlist"] = searching;
      return View("~/Views/Product/Index.cshtml", products.ToList());
    }
    catch (Exception e)
    {
      _logger.LogError("[ProductController] Index action failed: {e}", e.Message);
      return RedirectToAction("Error");
    }
  }

  // Display detailed view of a specific product
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
  
  // Display list of products for logged-in user
  [Authorize]
  [HttpGet]
  public async Task<IActionResult> Table()
  {
    var currentUserId = User.Identity?.Name;

    // Check if user is logged in
    if (string.IsNullOrEmpty(currentUserId))
    {
        _logger.LogWarning("User does not have a producer account.");
        return RedirectWithMessage("Producer", "Table", "Please create a producer account first", "warning");
    }

    try
    {
        // Get all products from database
        var Index = await _productRepository.GetAllProductsAsync();

        // Filter products to show only those belonging to the user's producers
        var filteredProducts = Index?
            .Where(p => p.Producer?.OwnerId == currentUserId)
            .ToList();

        return View(filteredProducts);
    }
    catch (Exception e)
    {
        _logger.LogError(e, "Error retrieving products");
        return RedirectWithMessage("Product", "Table", "Error retrieving products", "error");
    }
  }



  // Display create product page
  [Authorize]
  [HttpGet]
  public async Task<IActionResult> Create()
  {
    var currentUserId = User.Identity?.Name;

    // Check if user is logged in
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

    // Create list for producer dropdown
    ViewBag.Producers = new SelectList(userProducers, "ProducerId", "Name");

    // Create list for category dropdown
    var categories = new List<string>
        {
            "Fruits",
            "Vegetables",
            "Meat",
            "Bakery",
            "Dairy",
            "Drinks",
            "Other"
        };
    ViewBag.Categories = new SelectList(categories);

    // Create list for nutrition score
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

  // Create product
  [Authorize]
  [HttpPost]
  public async Task<IActionResult> Create(Product product, IFormFile? file)
  {

    // Check for negative values
    if (product.Calories < 0 || product.Protein < 0 || product.Carbohydrates < 0 || product.Fat < 0)
    {
        ModelState.AddModelError(string.Empty, "Calories, Protein, Carbohydrates, and Fat values cannot be negative.");

        // Reload dropdown lists
        await PopulateDropdowns();
        return View(product); // Return the form
    }

    if (!ModelState.IsValid)
    {
        // Reload dropdown lists
        await PopulateDropdowns();
        return View(product);
    }

    // Get and filter producers for current user
    var currentUserId = User.Identity?.Name;
    var userProducers = (await _producerRepository.GetAllProducersAsync())
        .Where(p => p.OwnerId == currentUserId)
        .ToList();

    // Populate dropdown lists for the view
    ViewBag.Producers = new SelectList(userProducers, "ProducerId", "Name");
    ViewBag.Categories = new SelectList(new List<string>
    {
        "Fruits", "Vegetables", "Meat", "Bakery", "Dairy", "Drinks", "Other"
    });
    ViewBag.NutritionScores = new SelectList(new List<string> { "A", "B", "C", "D", "E" });

    // Validate required fields
    if (!ModelState.IsValid || product.ProducerId == 0 || string.IsNullOrEmpty(product.NutritionScore))
    {
        if (product.ProducerId == 0)
        {
            ModelState.AddModelError("ProducerId", "Please select a producer");
        }
        if (string.IsNullOrEmpty(product.NutritionScore))
        {
            ModelState.AddModelError("NutritionScore", "Please select a nutrition score");
        }
        return View(product);
    }

    // Handle image upload if provided
    if (file != null && file.Length > 0)
    {
        string wwwRootPath = _webHostEnvironment.WebRootPath;
        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        string productPath = Path.Combine(wwwRootPath, "images", "product");
        
        Directory.CreateDirectory(productPath);
        string filePath = Path.Combine(productPath, fileName);
        
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
        
        product.ImageUrl = "/images/product/" + fileName;
    }

    if (!ModelState.IsValid)
    {
        await PopulateDropdowns();
        return View(product);
    }

    // Check if producer exists and belongs to current user
    var producer = await _producerRepository.GetProducerByIdAsync(product.ProducerId);
    if (producer == null || producer.OwnerId != User.Identity?.Name)
    {
        _logger.LogError("[ProductController] producer not found while executing _producerRepository.GetProducerByIdAsync()");   
        await PopulateDropdowns();
        return RedirectWithMessage("Product", "Create", "Invalid producer selection", "warning");
    }
    
    // Save product to database
    try
      {
          await _productRepository.CreateProductAsync(product);
          return RedirectToAction(nameof(Table));
      }
      catch (Exception e)
      {
          _logger.LogError(e, "Error creating product");
          await PopulateDropdowns();
          return RedirectWithMessage("Product", "Create", "Error uploading image", "danger");
      }
    }
    

  // Populate dropdown lists
  private async Task PopulateDropdowns()
  {
      var currentUserId = User.Identity?.Name;
      var userProducers = (await _producerRepository.GetAllProducersAsync())
          .Where(p => p.OwnerId == currentUserId)
          .ToList();
          
      ViewBag.Producers = new SelectList(userProducers, "ProducerId", "Name");
      ViewBag.Categories = new SelectList(new[] { "Fruits", "Vegetables", "Meat", "Bakery", "Dairy", "Drinks", "Other" });
      ViewBag.NutritionScores = new SelectList(new[] { "A", "B", "C", "D", "E" });
  }

 
  // Show update form for specific product
  [Authorize]
  [HttpGet]
  public async Task<IActionResult> Update(int id)
  {
    var product = await _productRepository.GetProductByIdAsync(id);

    if (product == null)
    {
      _logger.LogError("[ProductController] product not found while executing _productRepository.GetProductByIdAsync()");
      return BadRequest("Product not found");
    }

    // Add default image if none exists
    if (string.IsNullOrEmpty(product.ImageUrl))
    {
        product.ImageUrl = "https://mtek3d.com/wp-content/uploads/2018/01/image-placeholder-500x500.jpg";
    }

    // Get only producers owned by current user
    var currentUserId = User.Identity?.Name;
    var userProducers = (await _producerRepository.GetAllProducersAsync())
        .Where(p => p.OwnerId == currentUserId)
        .ToList();

    ViewBag.Categories = new SelectList(new List<string>
    {
        "Fruits", "Vegetables", "Meat", "Bakery", "Dairy", "Drinks", "Other"
    });

    // Update producer list to only show user's producers
    ViewBag.ProducerList = new SelectList(userProducers, "ProducerId", "Name", product.ProducerId);
    ViewBag.NutritionScores = new SelectList(new List<string>
    {
      "A", "B", "C", "D", "E"
    });

    return View(product);
  }

  // Update product
  [Authorize]
  [HttpPost]
  public async Task<IActionResult> Update(Product product, IFormFile? file, bool removeImage = false)
  {
    try
    {
        var existingProduct = await _productRepository.GetProductByIdAsync(product.ProductId);
        if (existingProduct == null)
        {
            return NotFound();
        }

        // Preserve the existing image URL before validation
        product.ImageUrl = existingProduct.ImageUrl;

        // Validate nutrition score
        if (string.IsNullOrEmpty(product.NutritionScore))
        {
            ModelState.AddModelError("NutritionScore", "Please select a nutrition score");
            // Reload dropdown lists
            var currentUserId = User.Identity?.Name;
            var userProducers = (await _producerRepository.GetAllProducersAsync())
                .Where(p => p.OwnerId == currentUserId)
                .ToList();
            ViewBag.ProducerList = new SelectList(userProducers, "ProducerId", "Name", product.ProducerId);
            ViewBag.Categories = new SelectList(new[] { "Fruits", "Vegetables", "Meat", "Bakery", "Dairy", "Drinks", "Other" });
            ViewBag.NutritionScores = new SelectList(new[] { "A", "B", "C", "D", "E" });
            return View(product);
        }

        // Verify ownership
        var producer = await _producerRepository.GetProducerByIdAsync(existingProduct.ProducerId);
        if (producer?.OwnerId != User.Identity?.Name)
        {
            return RedirectWithMessage("Product", "Table", "You can only Update your own products", "error");
        }

        // Remove image if requested
        if (removeImage && !string.IsNullOrEmpty(existingProduct.ImageUrl))
        {
            await _productRepository.DeleteProductImageAsync(existingProduct.ImageUrl, _webHostEnvironment.WebRootPath);
            existingProduct.ImageUrl = null;
        }
        // Upload new image if provided
        else if (file != null && file.Length > 0)
        {
            var newImagePath = await _productRepository.SaveProductImageAsync(file, _webHostEnvironment.WebRootPath);
            existingProduct.ImageUrl = newImagePath;
        }

        // Update the other properties
        existingProduct.Name = product.Name;
        existingProduct.Description = product.Description;
        existingProduct.Category = product.Category;
        existingProduct.NutritionScore = product.NutritionScore;
        existingProduct.Calories = product.Calories;
        existingProduct.Carbohydrates = product.Carbohydrates;
        existingProduct.Fat = product.Fat;
        existingProduct.Protein = product.Protein;

        await _productRepository.UpdateProductAsync(existingProduct);
        return RedirectWithMessage("Product", "Table", "Product successfully updated", "success");
    }
    catch (Exception e)
    {
        _logger.LogError($"Update failed: {e.Message}");
        return RedirectWithMessage("Product", "Table", "Failed to update product", "error");
    }
  }

  // Delete product
  [HttpPost]
  [Authorize]
  public async Task<IActionResult> Delete(int id)
  {
    try
    {
        var product = await _productRepository.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        var producer = await _producerRepository.GetProducerByIdAsync(product.ProducerId);
        if (producer?.OwnerId != User.Identity?.Name)
        {
            return RedirectWithMessage("Product", "Table", "You can only delete your own products", "error");
        }

        var success = await _productRepository.DeleteProductAsync(id);
        if (!success)
        {
            return RedirectWithMessage("Product", "Table", "Failed to delete product", "error");
        }

        //Feedback to the user
        return RedirectWithMessage("Product", "Table", "Product successfully deleted", "success");
    }
    catch (Exception e)
    {
        _logger.LogError($"Delete failed: {e.Message}");
        return RedirectWithMessage("Product", "Table", "Failed to delete product", "error");
    }
  }



  // Redirect with status messages
  private IActionResult RedirectWithMessage(string controller, string action, string message, string messageType = "info")
  {
    return Redirect($"/{controller}/{action}?message={Uri.EscapeDataString(message)}&messageType={messageType}");
  }
}