using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Food_Registration.Models;
using Food_Registration.DAL;
using Microsoft.Extensions.Logging; 
using Microsoft.AspNetCore.Http;
using System.IO;

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

    [Authorize]
    [HttpGet]
    public IActionResult Create()
    {
      return View();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(Producer producer, IFormFile file)
    {
        if (!ModelState.IsValid)
        {
            return View(producer);
        }

        try
        {
            // Handle image upload
            if (file != null)
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
            await _producerRepository.AddProducerAsync(producer);
            return RedirectToAction(nameof(Table));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating producer");
            return RedirectWithMessage("Producer", "Create", "Error creating producer", "danger");
        }
    }

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

/*
      // Fetch the producer
      var producer = await _ItemDbContext.Producers.FirstOrDefaultAsync(p => p.ProducerId == id);

      if (producer == null)
      {
        return NotFound();
      }
*/
      // Check if the current user owns this producer
      var currentUserId = User.Identity?.Name;
      if (producer.OwnerId != currentUserId)
      {
        return RedirectWithMessage("Producer", "Table", "You can only Update your own producers", "danger");
      }

      return View(producer);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Update(Producer producer, IFormFile? file)
    {
        if (!ModelState.IsValid)
        {
            return View(producer);
        }

        try
        {
            // Get existing producer to check ownership and current image
            var existingProducer = await _producerRepository.GetProducerByIdAsync(producer.ProducerId);
            if (existingProducer == null)
            {
                return NotFound();
            }

            // Check ownership
            var currentUserId = User.Identity?.Name;
            if (existingProducer.OwnerId != currentUserId)
            {
                return RedirectWithMessage("Producer", "Table", "You can only update your own producers", "danger");
            }

            // Copy over the existing image URL unless a new file is being uploaded
            if (file == null || file.Length == 0)
            {
                producer.ImageUrl = existingProducer.ImageUrl;
            }

            // Handle image upload if there's a new file
            if (file != null && file.Length > 0)
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(existingProducer.ImageUrl))
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string oldImagePath = Path.Combine(wwwRootPath, existingProducer.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Save new image
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string producerPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "producer");
                
                Directory.CreateDirectory(producerPath);
                string filePath = Path.Combine(producerPath, fileName);
                
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                
                producer.ImageUrl = "/images/producer/" + fileName;
            }

            var success = await _producerRepository.UpdateProducerAsync(producer);
            if (success)
            {
                return RedirectWithMessage("Producer", "Table", "Producer successfully updated", "success");
            }
            else
            {
                return RedirectWithMessage("Producer", "Table", "Failed to update producer", "error");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating producer: {ex.Message}");
            return RedirectWithMessage("Producer", "Table", "Error updating producer", "error");
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
      var producer = await _producerRepository.GetProducerByIdAsync(id);
      if (producer == null)
      {
        _logger.LogError("[ProducerController] Producer not found while executing GetProducerByIdAsync()");
        return BadRequest("Producer not found for id: " + id);
      }
      /*
      // Find the producer
      var producer = await _ItemDbContext.Producers
        .FirstOrDefaultAsync(p => p.ProducerId == id);
      if (producer == null)
      {
        return NotFound();
      }
      */

      // Verify ownership before allowing deletion
      var currentUserId = User.Identity?.Name;
      if (producer.OwnerId != currentUserId)
      {
        return RedirectWithMessage("Producer", "Table", "You can only delete your own producers", "danger");
      }

      var products = await _productRepository.GetAllProductsAsync();
      /*
      // Check if producer has any associated products
      if (_ItemDbContext.Products == null)
      {
        return Problem("Entity set 'ItemDbContext.Products' is null.");
      }
      */
      var hasProducts = products.Any(p => p.ProducerId == id);
      if (hasProducts)
      {
        _logger.LogWarning("[ProducerController] Cannot delete producer that has associated products");
        return RedirectWithMessage("Producer", "Table",
          "Cannot delete producer that has associated products. Please delete all products first.",
          "warning");
      }
      
      bool returnOk = await _producerRepository.DeleteProducerAsync(id);
      if (!returnOk)
      {
        _logger.LogError("[ProducerController] Producer deletion failed while executing Delete()");
        return BadRequest("Producer deletion failed");
      }
      // Delete the producer
      //await _producerRepository.DeleteProducerAsync(producer);
      //_ItemDbContext.Producers.Remove(producer);
      //await _ItemDbContext.SaveChangesAsync();

      return RedirectWithMessage("Producer", "Table", "Producer successfully deleted", "success");
    }

    private IActionResult RedirectWithMessage(string controller, string action, string message, string messageType = "info")
    {
      return Redirect($"/{controller}/{action}?message={Uri.EscapeDataString(message)}&messageType={messageType}");
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> ReadMore(int id)
    {
        var producer = await _producerRepository.GetProducerByIdAsync(id);
        
        if (producer == null)
        {
            return NotFound();
        }

        // Legg til default bilde hvis ingen finnes
        if (string.IsNullOrEmpty(producer.ImageUrl))
        {
            producer.ImageUrl = "https://mtek3d.com/wp-content/uploads/2018/01/image-placeholder-500x500.jpg";
        }

        return View(producer);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> RemoveImage(int id)
    {
        var producer = await _producerRepository.GetProducerByIdAsync(id);
        if (producer == null)
        {
            return NotFound();
        }

        // Check if the current user owns this producer
        var currentUserId = User.Identity?.Name;
        if (producer.OwnerId != currentUserId)
        {
            return RedirectWithMessage("Producer", "Table", "You can only update your own producers", "danger");
        }

        if (!string.IsNullOrEmpty(producer.ImageUrl))
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string imagePath = Path.Combine(wwwRootPath, producer.ImageUrl.TrimStart('/'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            producer.ImageUrl = null;
            await _producerRepository.UpdateProducerAsync(producer);
        }

        return RedirectToAction(nameof(Update), new { id });
    }
  }
}