using Microsoft.AspNetCore.Mvc;
using Food_Registration.Models;
using Microsoft.AspNetCore.Authorization;
using Food_Registration.DAL;
using System.Security.Claims;
using Food_Registration.DTOs;
using System.IO;

namespace Food_Registration.Controllers;


[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
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
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts([FromQuery] string? category, [FromQuery] string? search)
    {
        try
        {
            var products = await _productRepository.GetAllProductsAsync();

            if (!string.IsNullOrEmpty(category))
                products = products.Where(p => p.Category == category);

            if (!string.IsNullOrEmpty(search))
                products = products.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase));

            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products");
            return StatusCode(500, "An error occurred while retrieving the products");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProductById(int id)
    {
        try
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();
            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product {ProductId}", id);
            return StatusCode(500, "An error occurred while retrieving the product");
        }
    }

    [Authorize]
    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<Product>>> GetMyProducts()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null)
            {
                _logger.LogWarning("Unauthorized access attempt - no email claim found");
                return Unauthorized();
            }

            // Get list of producerIds of producers I own
            var producers = await _producerRepository.GetAllProducersAsync();
            var myProducers = producers.Where(p => p.OwnerId == email).Select(p => p.ProducerId).ToList();
            if (myProducers == null)
            {
                _logger.LogWarning("Unauthorized access attempt - no producers found for user {Email}", email);
                return Unauthorized();
            }

            // Get products owned by producers I own        
            var products = await _productRepository.GetAllProductsAsync();
            products = products.Where(p => myProducers.Contains(p.ProducerId)).ToList();

            _logger.LogInformation("Successfully retrieved {Count} products for user {Email}", products.Count(), email);
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products for user");
            return StatusCode(500, "An error occurred while retrieving the products");
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct([FromBody] CreateProductDTO productDTO)
    {
        try
        {
            // Get user email
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null)
            {
                return Unauthorized("Unauthorized");
            }

            // Check if user is owner of producer
            var producers = await _producerRepository.GetAllProducersAsync();
            var isProducerOwner = producers.Any(p => p.OwnerId == email && p.ProducerId == productDTO.ProducerId);
            if (!isProducerOwner)
            {
                return Unauthorized("Unauthorized. Not owner of producer.");
            }

            // Handle image upload
            string? imagePath = null;
            if (!string.IsNullOrEmpty(productDTO.ImageBase64))
            {
                // Validate file extension
                var extension = productDTO.ImageFileExtension?.ToLowerInvariant();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

                if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                {
                    return BadRequest("Invalid file extension. Only .jpg, .jpeg, .png, and .gif are allowed.");
                }

                try
                {
                    // Convert base64 to bytes
                    byte[] imageBytes = Convert.FromBase64String(productDTO.ImageBase64);

                    // Generate unique filename
                    string uniqueFileName = $"{Guid.NewGuid()}{extension}";
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");

                    // Create directory if it doesn't exist
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save the file
                    await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

                    imagePath = $"/images/products/{uniqueFileName}";
                }
                catch (FormatException)
                {
                    return BadRequest("Invalid base64 string");
                }
                catch (Exception)
                {
                    return BadRequest("Invalid image file");
                }
            }

            // Create product
            var product = new Product
            {
                Name = productDTO.Name,
                Description = productDTO.Description,
                NutritionScore = productDTO.NutritionScore,
                Category = productDTO.Category,
                ProducerId = productDTO.ProducerId,
                Calories = productDTO.Calories,
                Carbohydrates = productDTO.Carbohydrates,
                Fat = productDTO.Fat,
                Protein = productDTO.Protein,
                ImageUrl = imagePath
            };

            await _productRepository.CreateProductAsync(product);

            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return StatusCode(500, "An error occurred while creating the product");
        }
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDTO productDTO)
    {
        if (id != productDTO.ProductId)
        {

            return BadRequest();
        }

        // Get user email
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (email == null)
        {
            return Unauthorized();
        }

        // Get original product
        var originalProduct = await _productRepository.GetProductByIdAsync(id);
        if (originalProduct == null)
        {
            return NotFound();
        }

        // Check if user is owner of original product's producer
        var producers = await _producerRepository.GetAllProducersAsync();
        var isOriginalProducerOwner = producers.Any(p => p.OwnerId == email && p.ProducerId == originalProduct.ProducerId);
        if (!isOriginalProducerOwner)
        {
            return Unauthorized();
        }

        // Check if user is owner of producer of product to update
        var isProducerOwner = producers.Any(p => p.OwnerId == email && p.ProducerId == productDTO.ProducerId);
        if (!isProducerOwner)
        {
            return Unauthorized();
        }

        try
        {
            // Handle image upload
            string? imagePath = originalProduct.ImageUrl;
            if (!string.IsNullOrEmpty(productDTO.ImageBase64))
            {
                // Validate file extension
                var extension = productDTO.ImageFileExtension?.ToLowerInvariant();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

                if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                {
                    return BadRequest("Invalid file extension. Only .jpg, .jpeg, .png, and .gif are allowed.");
                }

                try
                {
                    // Delete old image if it exists
                    if (!string.IsNullOrEmpty(originalProduct.ImageUrl))
                    {
                        string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, originalProduct.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    // Convert base64 to bytes
                    byte[] imageBytes = Convert.FromBase64String(productDTO.ImageBase64);

                    // Generate unique filename
                    string uniqueFileName = $"{Guid.NewGuid()}{extension}";
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");

                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save the file
                    await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

                    imagePath = $"/images/products/{uniqueFileName}";
                }
                catch (FormatException)
                {
                    return BadRequest("Invalid base64 string");
                }
                catch (Exception)
                {
                    return BadRequest("Invalid image file");
                }
            }

            // Update product
            originalProduct.Name = productDTO.Name;
            originalProduct.Description = productDTO.Description;
            originalProduct.NutritionScore = productDTO.NutritionScore;
            originalProduct.Category = productDTO.Category;
            originalProduct.ProducerId = productDTO.ProducerId;
            originalProduct.Calories = productDTO.Calories;
            originalProduct.Carbohydrates = productDTO.Carbohydrates;
            originalProduct.Fat = productDTO.Fat;
            originalProduct.Protein = productDTO.Protein;
            originalProduct.ImageUrl = imagePath;

            await _productRepository.UpdateProductAsync(originalProduct);
            return Ok(originalProduct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product {ProductId}", id);
            return StatusCode(500, "An error occurred while updating the product");
        }
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        try
        {
            // Get user email
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null)
            {
                return Unauthorized();
            }

            // Get original product
            var originalProduct = await _productRepository.GetProductByIdAsync(id);
            if (originalProduct == null)
            {
                return NotFound();
            }

            // Check if user is owner of original product's producer
            var producers = await _producerRepository.GetAllProducersAsync();
            var isOriginalProducerOwner = producers.Any(p => p.OwnerId == email && p.ProducerId == originalProduct.ProducerId);
            if (!isOriginalProducerOwner)
            {
                return Unauthorized();
            }

            // Delete product
            await _productRepository.DeleteProductAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product {ProductId}", id);
            return StatusCode(500, "An error occurred while deleting the product");
        }
    }
}