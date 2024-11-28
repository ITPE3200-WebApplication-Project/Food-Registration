using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Food_Registration.Models;
using Microsoft.EntityFrameworkCore;
using Food_Registration.DAL;
using Microsoft.Extensions.Logging; 


namespace Food_Registration.Controllers
{
  public class ProducerController : Controller
  {
    private readonly ItemDbContext _ItemDbContext;

     private readonly ILogger<ProducerController> _logger; 

      public ProducerController(ItemDbContext ItemDbContext, ILogger<ProducerController> logger)
        {
            _ItemDbContext = ItemDbContext;
            _logger = logger; 
        }


    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Table()
    {
      if (_ItemDbContext?.Producers == null)
      {
        return Problem("Entity set 'ItemDbContext.Producers' is null.");
      }

      var currentUserId = User.Identity?.Name;

      var producers = await _ItemDbContext.Producers
        .Where(p => p.OwnerId == currentUserId)
        .ToListAsync();

      return View(producers);
    }

    [Authorize]
    [HttpGet]
    public IActionResult NewProducer()
    {
      return View();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> NewProducer(Producer producer)
    {
      if (!ModelState.IsValid)
      {
        return View(producer);
      }

      if (_ItemDbContext?.Producers == null)
      {
        return Problem("Entity set 'ItemDbContext.Producers' is null.");
      }

      // Make sure the user is logged in
      if (User.Identity == null || User.Identity.Name == null)
      {
        return RedirectWithMessage("Producer", "NewProducer", "You must be logged in to create a producer", "danger");
      }

      // Set the owner to the current user
      producer.OwnerId = User.Identity.Name;
      _ItemDbContext.Producers.Add(producer);

      await _ItemDbContext.SaveChangesAsync();
      return RedirectToAction(nameof(Table));
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
      if (_ItemDbContext?.Producers == null)
      {
        return Problem("Entity set 'ItemDbContext.Producers' is null.");
      }

      // Fetch the producer
      var producer = await _ItemDbContext.Producers.FirstOrDefaultAsync(p => p.ProducerId == id);

      if (producer == null)
      {
        return NotFound();
      }

      // Check if the current user owns this producer
      var currentUserId = User.Identity?.Name;
      if (producer.OwnerId != currentUserId)
      {
        return RedirectWithMessage("Producer", "Table", "You can only edit your own producers", "danger");
      }

      return View(producer);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Producer producer)
    {
      if (!ModelState.IsValid)
      {
        return View(producer);
      }

      // Fetch the existing producer
      if (_ItemDbContext.Producers == null)
      {
        return NotFound();
      }

      var existingProducer = await _ItemDbContext.Producers.FirstOrDefaultAsync(p => p.ProducerId == producer.ProducerId);
      if (existingProducer == null)
      {
        return NotFound();
      }

      // Verify ownership before allowing edit
      var currentUserId = User.Identity?.Name;
      if (existingProducer.OwnerId != currentUserId)
      {
        return RedirectWithMessage("Producer", "Table", "You can only edit your own producers", "danger");
      }

      // Update all fields except OwnerId (which should remain unchanged)
      existingProducer.Name = producer.Name;
      existingProducer.Description = producer.Description;
      existingProducer.ImageUrl = producer.ImageUrl;

      await _ItemDbContext.SaveChangesAsync();
      return RedirectToAction(nameof(Table));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      if (_ItemDbContext?.Producers == null)
      {
        return Problem("Entity set 'ItemDbContext.Producers' is null.");
      }
      // Find the producer
      var producer = await _ItemDbContext.Producers
        .FirstOrDefaultAsync(p => p.ProducerId == id);
      if (producer == null)
      {
        return NotFound();
      }

      // Verify ownership before allowing deletion
      var currentUserId = User.Identity?.Name;
      if (producer.OwnerId != currentUserId)
      {
        return RedirectWithMessage("Producer", "Table", "You can only delete your own producers", "danger");
      }

      // Check if producer has any associated products
      if (_ItemDbContext.Products == null)
      {
        return Problem("Entity set 'ItemDbContext.Products' is null.");
      }
      var hasProducts = await _ItemDbContext.Products.AnyAsync(p => p.ProducerId == id);
      if (hasProducts)
      {
        return RedirectWithMessage("Producer", "Table",
          "Cannot delete producer that has associated products. Please delete all products first.",
          "warning");
      }

      // Delete the producer
      _ItemDbContext.Producers.Remove(producer);
      await _ItemDbContext.SaveChangesAsync();

      return RedirectWithMessage("Producer", "Table", "Producer successfully deleted", "success");
    }

    private IActionResult RedirectWithMessage(string controller, string action, string message, string messageType = "info")
    {
      return Redirect($"/{controller}/{action}?message={Uri.EscapeDataString(message)}&messageType={messageType}");
    }
  }
}