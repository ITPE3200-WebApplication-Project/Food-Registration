using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Food_Registration.Models;
using Food_Registration.DAL;
using System.Security.Claims;


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
    public async Task<IActionResult> Create([FromBody] Producer producer)
    {
      try
      {
        // Get user email
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (email == null)
        {
          return Unauthorized();
        }

        // Set owner id
        producer.OwnerId = email;


        // Validate imageUrl
        if (string.IsNullOrEmpty(producer.ImageUrl) || !producer.ImageUrl.StartsWith("/images/producer/"))
        {
          _logger.LogError("Invalid imageUrl: {ImageUrl}", producer.ImageUrl);
          return BadRequest("Invalid imageUrl");
        }

        // Create producer
        await _producerRepository.CreateProducerAsync(producer);

        return Ok(producer);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error creating producer");
        return StatusCode(500, "Error creating producer");
      }
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Producer producer)
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
        var originalProducer = await _producerRepository.GetProducerByIdAsync(id);
        if (originalProducer == null)
        {
          _logger.LogError("[ProducerController] Producer not found while executing GetProducerByIdAsync()");
          return BadRequest("Producer not found for id: " + id);
        }


        // Make sure producer belongs to current user
        if (originalProducer.OwnerId != email)
        {
          _logger.LogError("Producer does not belong to current user");
          return Unauthorized("You can only update your own producers");
        }


        if (producer.OwnerId != originalProducer.OwnerId)
        {
          _logger.LogError("You cannot change the owner of a producer");
          return Unauthorized("You cannot change the owner of a producer");
        }

        // Update producer
        await _producerRepository.UpdateProducerAsync(producer);

        return Ok(producer);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error updating producer");
        return StatusCode(500, "Error updating producer");
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