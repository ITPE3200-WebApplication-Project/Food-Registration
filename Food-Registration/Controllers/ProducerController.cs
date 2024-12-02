using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Food_Registration.Models;
using Food_Registration.DAL;
using System.Security.Claims;
using Food_Registration.DTOs;


namespace Food_Registration.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
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
    public async Task<ActionResult<IEnumerable<Producer>>> GetMyProducers()
    {
      try
      {
        // Get user email
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (email == null)
        {
          return Unauthorized();
        }

        // Get all producers
        var producers = await _producerRepository.GetAllProducersAsync();

        // Filter producers by current user
        var filteredProducers = producers.Where(p => p.OwnerId == email).ToList();

        if (filteredProducers == null)
        {
          return NotFound("No producers found for current user");
        }

        return Ok(filteredProducers);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error retrieving producers");
        return StatusCode(500, "An error occurred while retrieving the producers");
      }
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<Producer>> GetProducerById(int id)
    {
      try
      {
        // Get user email
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (email == null)
        {
          return Unauthorized();
        }

        // Get producer
        var producer = await _producerRepository.GetProducerByIdAsync(id);
        if (producer == null)
        {
          return NotFound($"Producer with ID {id} not found");
        }

        // Make sure producer belongs to current user
        if (producer.OwnerId != email)
        {
          return Unauthorized("You can only access your own producers");
        }

        return Ok(producer);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error retrieving producer");
        return StatusCode(500, "An error occurred while retrieving the producer");
      }
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProducerDTO producerDTO)
    {
      try
      {
        // Get user email
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (email == null)
        {
          return Unauthorized("Unauthorized");
        }

        // Handle image upload
        string? imagePath = null;
        if (!string.IsNullOrEmpty(producerDTO.ImageBase64))
        {
          // Validate file extension
          var extension = producerDTO.ImageFileExtension?.ToLowerInvariant();
          var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

          if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
          {
            return BadRequest("Invalid file extension. Only .jpg, .jpeg, .png, and .gif are allowed.");
          }

          try
          {
            // Convert base64 to bytes
            byte[] imageBytes = Convert.FromBase64String(producerDTO.ImageBase64);

            // Generate unique filename
            string uniqueFileName = $"{Guid.NewGuid()}{extension}";
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "producers");

            // Create directory if it doesn't exist
            if (!Directory.Exists(uploadsFolder))
              Directory.CreateDirectory(uploadsFolder);

            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Save the file
            await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

            imagePath = $"/images/producers/{uniqueFileName}";
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

        // Create producer
        var producer = new Producer
        {
          OwnerId = email,
          Name = producerDTO.Name,
          Description = producerDTO.Description,
          ImageUrl = imagePath
        };

        await _producerRepository.CreateProducerAsync(producer);

        return Ok(producer);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error creating producer");
        return StatusCode(500, "An error occurred while creating the producer");
      }
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProducerDTO producerDTO)
    {
      if (id != producerDTO.ProducerId)
      {
        return BadRequest();
      }

      // Get user email
      var email = User.FindFirst(ClaimTypes.Email)?.Value;
      if (email == null)
      {
        return Unauthorized();
      }

      // Get original producer
      var originalProducer = await _producerRepository.GetProducerByIdAsync(id);
      if (originalProducer == null)
      {
        return NotFound();
      }

      // Check if user is owner of producer
      if (originalProducer.OwnerId != email)
      {
        return Unauthorized();
      }

      try
      {
        // Handle image upload
        string? imagePath = originalProducer.ImageUrl;
        if (!string.IsNullOrEmpty(producerDTO.ImageBase64))
        {
          // Validate file extension
          var extension = producerDTO.ImageFileExtension?.ToLowerInvariant();
          var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

          if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
          {
            return BadRequest("Invalid file extension. Only .jpg, .jpeg, .png, and .gif are allowed.");
          }

          try
          {
            // Delete old image if it exists
            if (!string.IsNullOrEmpty(originalProducer.ImageUrl))
            {
              string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, originalProducer.ImageUrl.TrimStart('/'));
              if (System.IO.File.Exists(oldFilePath))
              {
                System.IO.File.Delete(oldFilePath);
              }
            }

            // Convert base64 to bytes
            byte[] imageBytes = Convert.FromBase64String(producerDTO.ImageBase64);

            // Generate unique filename
            string uniqueFileName = $"{Guid.NewGuid()}{extension}";
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "producers");

            if (!Directory.Exists(uploadsFolder))
              Directory.CreateDirectory(uploadsFolder);

            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Save the file
            await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

            imagePath = $"/images/producers/{uniqueFileName}";
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

        // Update producer
        originalProducer.Name = producerDTO.Name;
        originalProducer.Description = producerDTO.Description;
        originalProducer.ImageUrl = imagePath;

        await _producerRepository.UpdateProducerAsync(originalProducer);
        return Ok(originalProducer);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error updating producer {ProducerId}", id);
        return StatusCode(500, "An error occurred while updating the producer");
      }
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
      try
      {
        // Get user email
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (email == null)
        {
          return Unauthorized();
        }

        // Make sure producer exists
        var producer = await _producerRepository.GetProducerByIdAsync(id);
        if (producer == null)
        {
          _logger.LogError("[ProducerController] Producer not found while executing GetProducerByIdAsync()");
          return BadRequest("Producer not found for id: " + id);
        }

        // Make sure producer belongs to current user
        if (producer.OwnerId != email)
        {
          _logger.LogError("Producer does not belong to current user");
          return Unauthorized("You can only delete your own producers");
        }

        // Make sure producer has no products
        var products = await _productRepository.GetAllProductsAsync();
        if (products.Any(p => p.ProducerId == id))
        {
          return BadRequest("Producer has products and cannot be deleted");
        }

        // Delete producer
        await _producerRepository.DeleteProducerAsync(id);

        return Ok();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error deleting producer");
        return StatusCode(500, "Error deleting producer");
      }
    }
  }
}