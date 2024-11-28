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

    if (product.Producer.ImageUrl == null || product.Producer.ImageUrl == "")
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

    if (string.IsNullOrEmpty(currentUserId))
    {
      _logger.LogWarning("User does not have a producer account.");
      return RedirectWithMessage("Producer", "Table", "Please create a producer account first", "warning");
    }

   try{
    _logger.LogInformation("Getting all products for the user");

     // Get all products including producers
    var allProducts = await _productRepository.GetAllProductsAsync();

    // Only get products owned by current user
    var products = allProducts.Where(p => p.Producer?.OwnerId == currentUserId).ToList();

    var viewModel = new ProductsViewModel(products, "Table");

    return View(viewModel);
    
   }
    catch{
      _logger.LogError("[ProductController] Product not found for the user.");
      return RedirectWithMessage("Error", "Table", "An error occurred while processing your request.", "danger");
    }

    
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
            "Bakery",
            "Dairy",
            "Drinks",
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
  public async Task<IActionResult> NewProduct(Product product, IFormFile file)
  {
      if (!ModelState.IsValid)
      {
          await PopulateDropdowns();
          return View(product);
      }

      var producer = await _producerRepository.GetProducerByIdAsync(product.ProducerId);
      if (producer == null || producer.OwnerId != User.Identity?.Name)
      {
          await PopulateDropdowns();
          return RedirectWithMessage("Product", "NewProduct", "Invalid producer selection", "warning");
      }

      try
      {
          if (file != null)
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

          await _productRepository.AddProductAsync(product);
          return RedirectToAction(nameof(Table));
      }
      catch (Exception ex)
      {
          _logger.LogError(ex, "Error creating product");
          await PopulateDropdowns();
          return RedirectWithMessage("Product", "NewProduct", "Error uploading image", "danger");
      }
  }

  private async Task PopulateDropdowns()
  {
      var currentUserId = User.Identity?.Name;
      var userProducers = (await _producerRepository.GetAllProducersAsync())
          .Where(p => p.OwnerId == currentUserId)
          .ToList();
          
      ViewBag.Producers = new SelectList(userProducers, "ProducerId", "Name");
      ViewBag.Categories = new SelectList(new[] { "Fruits", "Vegetables", "Meat", "Fish", "Dairy", "Grains", "Beverages", "Snacks", "Other" });
      ViewBag.NutritionScores = new SelectList(new[] { "A", "B", "C", "D", "E" });
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
public async Task<IActionResult> Edit(Product product, IFormFile file)
{
    if (!ModelState.IsValid)
    {
        await PopulateDropdowns();
        return View(product);
    }

    var originalProduct = await _productRepository.GetProductByIdAsync(product.ProductId);
    if (originalProduct == null)
    {
        return NotFound();
    }

    try
    {
        // If a new image file is uploaded
        if (file != null)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string productPath = Path.Combine(wwwRootPath, "images", "product");

            // Ensure the directory exists
            Directory.CreateDirectory(productPath);

            string filePath = Path.Combine(productPath, fileName);

            // Log image saving
            _logger.LogInformation($"Saving image to: {filePath}");

            // Save the uploaded image
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Update the ImageUrl to the new image
            product.ImageUrl = "/images/product/" + fileName;
            _logger.LogInformation($"Saving product with ImageUrl: {product.ImageUrl}");
        }
        else
        {
            // If no new file is uploaded, retain the original ImageUrl
            product.ImageUrl = originalProduct.ImageUrl;
        }

        // Update the product in the database
        await _productRepository.UpdateProductAsync(product);

        return RedirectToAction("Edit", new { id = product.ProductId, message = "Product updated", messageType = "info" });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error updating product");
        await PopulateDropdowns();
        return RedirectWithMessage("Product", "Edit", "Error updating product", "danger");
    }
}



  private IActionResult RedirectWithMessage(string controller, string action, string message, string messageType = "info")
  {
    return Redirect($"/{controller}/{action}?message={Uri.EscapeDataString(message)}&messageType={messageType}");
  }
}