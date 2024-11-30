using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Food_Registration.Models;
using Food_Registration.DAL;
using System.Security.Claims;

namespace Food_Registration.Controllers
{
  public class ProducerController : Controller
  {
    
    private readonly IProducerRepository _producerRepository;

    private readonly IProductRepository _productRepository;

    private readonly ILogger<ProducerController> _logger; 

    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProducerController(IProducerRepository producerRepository, IProductRepository productRepository, ILogger<ProducerController> logger, IWebHostEnvironment webHostEnvironment)
    {
      _producerRepository = producerRepository;
      _productRepository = productRepository;
      _logger = logger; 
      _webHostEnvironment = webHostEnvironment;
    }


    // Display list of producers for logged-in user
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Table()
    {
      var producers = await _producerRepository.GetAllProducersAsync();
      
      if (producers == null)
      {
        _logger.LogError("[ProducerController] Producer list not found while executing _producerRepository.GetAllProducersAsync()");
        return NotFound("Producer list not found");
      }

      // Filter producers by current user
      var currentUserId = User.Identity?.Name;
      var filteredProducers = producers.Where(p => p.OwnerId == currentUserId).ToList();
      
      return View(filteredProducers);
    }

    // Show create producer page
    [Authorize]
    [HttpGet]
    public IActionResult Create()
    {
      return View();
    }

    // Create a new producer
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(Producer producer, IFormFile? file)
    {
        if (!ModelState.IsValid)
        {
            return View(producer);
        }

        try
        {
            // Handle image upload if a file was provided
            if (file != null && file.Length > 0)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string producerPath = Path.Combine(wwwRootPath, "images", "producer");
                
                // Ensure directory exists
                Directory.CreateDirectory(producerPath);
                string filePath = Path.Combine(producerPath, fileName);
                
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                
                producer.ImageUrl = "/images/producer/" + fileName;
            }

            producer.OwnerId = User.Identity?.Name ?? string.Empty;
            await _producerRepository.CreateProducerAsync(producer);
            return RedirectToAction(nameof(Table));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating producer");
            return RedirectWithMessage("Producer", "Create", "Error creating producer", "danger");
        }
    }

    // Update a producer
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
      var producer = await _producerRepository.GetProducerByIdAsync(id);
      if (producer == null)
      {
        _logger.LogError("[ProducerController] Producer not found while executing GetProducerByIdAsync()");
        return BadRequest("Producer not found for id: " + id);
      }


      // Check if the current user owns this producer
      var currentUserId = User.Identity?.Name;
      if (producer.OwnerId != currentUserId)
      {
        return RedirectWithMessage("Producer", "Table", "You can only Update your own producers", "danger");
      }

      return View(producer);
    }

    // Update a producer
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Update(Producer producer, IFormFile? file, bool removeImage = false)
    {
        try
        {
            var existingProducer = await _producerRepository.GetProducerByIdAsync(producer.ProducerId);
            if (existingProducer == null)
            {
                return NotFound();
            }
            // Check if the current user owns this producer
            if (existingProducer.OwnerId != User.Identity?.Name)
            {
                return RedirectWithMessage("Producer", "Table", "You can only update your own producers", "error");
            }

            // Handle image removal
            if (removeImage && !string.IsNullOrEmpty(existingProducer.ImageUrl))
            {
                await _producerRepository.DeleteProducerImageAsync(existingProducer.ImageUrl, _webHostEnvironment.WebRootPath);
                existingProducer.ImageUrl = null;
            }
            // Handle new image upload
            else if (file != null && file.Length > 0)
            {
                var newImagePath = await _producerRepository.SaveProducerImageAsync(file, _webHostEnvironment.WebRootPath);
                existingProducer.ImageUrl = newImagePath;
            }

            // Update other properties
            existingProducer.Name = producer.Name;
            existingProducer.Description = producer.Description;

            await _producerRepository.UpdateProducerAsync(existingProducer);
            return RedirectWithMessage("Producer", "Table", "Producer successfully updated", "success");
        }
        catch (Exception e)
        {
            _logger.LogError($"Update failed: {e.Message}");
            return RedirectWithMessage("Producer", "Table", "Failed to update producer", "error");
        }
    }

    // Delete producer if no associated products exist
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var producer = await _producerRepository.GetProducerByIdAsync(id);
            if (producer == null)
            {
                return NotFound();
            }

            // Check if the current user owns this producer
            if (producer.OwnerId != User.Identity?.Name)
            {
                return RedirectWithMessage("Producer", "Table", "You can only delete your own producers", "error");
            }

            // Check for associated products
            var hasProducts = await _producerRepository.HasAssociatedProductsAsync(id);
            if (hasProducts)
            {
                return RedirectWithMessage("Producer", "Table", 
                    "Cannot delete producer that has associated products. Please delete all products first.", 
                    "warning");
            }

            // Delete producer image if exists
            if (!string.IsNullOrEmpty(producer.ImageUrl))
            {
                await _producerRepository.DeleteProducerImageAsync(producer.ImageUrl, _webHostEnvironment.WebRootPath);
            }

            await _producerRepository.DeleteProducerAsync(id);
            return RedirectWithMessage("Producer", "Table", "Producer successfully deleted", "success");
        }
        catch (Exception e)
        {
            _logger.LogError($"Delete failed: {e.Message}");
            return RedirectWithMessage("Producer", "Table", "Failed to delete producer", "error");
        }
    }

    // Redirecting with status messages
    private IActionResult RedirectWithMessage(string controller, string action, string message, string messageType = "info")
    {
      return Redirect($"/{controller}/{action}?message={Uri.EscapeDataString(message)}&messageType={messageType}");
    }

    // Display detailed view of a producer
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> ReadMore(int id)
    {
        var producer = await _producerRepository.GetProducerByIdAsync(id);
        
        if (producer == null)
        {
            return NotFound();
        }

        // Add default image if none exists
        if (string.IsNullOrEmpty(producer.ImageUrl))
        {
            producer.ImageUrl = "https://mtek3d.com/wp-content/uploads/2018/01/image-placeholder-500x500.jpg";
        }

        return View(producer);
    }

    // Handle removal of producer image
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> RemoveImage(int id)
    {
        try
        {
            var producer = await _producerRepository.GetProducerByIdAsync(id);
            if (producer == null)
            {
                _logger.LogWarning($"Producer not found with ID: {id}");
                return NotFound();
            }

            // Only proceed if the producer has an image
            if (!string.IsNullOrEmpty(producer.ImageUrl))
            {
                // Construct the full path to the image file
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string imagePath = Path.Combine(wwwRootPath, producer.ImageUrl.TrimStart('/'));
                
                // Delete the physical file if it exists
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                else
                {
                    _logger.LogWarning($"File not found at path: {imagePath}");
                }
            }

            // Clear the image URL in the database after successful file deletion
            producer.ImageUrl = null;
            var updateResult = await _producerRepository.UpdateProducerAsync(producer);
            if (!updateResult)
            {
                return RedirectWithMessage("Producer", "Update", "Failed to remove image", "error");
            }

            return RedirectToAction("Update", new { id = producer.ProducerId });
        }
        catch (Exception e)
        {
            _logger.LogError("[ProducerController] RemoveImage action failed: {e}", e.Message);
            return RedirectWithMessage("Producer", "Update", "An error occurred while removing the image", "error");
        }
    }
  }
}