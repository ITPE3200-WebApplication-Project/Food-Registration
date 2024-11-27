using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Food_Registration.Models;
using Microsoft.EntityFrameworkCore;
using Food_Registration.DAL;

namespace Food_Registration.Controllers
{
  public class ProducerController : Controller
  {
    private readonly ProductDbContext _ProductDbContext;

    public ProducerController(ProductDbContext ProductDbContext)
    {
      _ProductDbContext = ProductDbContext;
    }

    [Authorize]
    public async Task <IActionResult> Table()
    {
      if (_ProductDbContext?.Producers == null)
      {
        return Problem("Entity set 'ProductDbContext.Producers' is null.");
      }

      var currentUserId = User.Identity?.Name;

      var producers = await _ProductDbContext.Producers
        .Where(p => p.OwnerId == currentUserId)
        .ToListAsync();

      return View(producers);
    }

    [Authorize]
    public IActionResult NewProducer()
    {
      return View();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> NewProducer(Producer producer)
    {
      if (ModelState.IsValid)
      {
        if (_ProductDbContext?.Producers == null)
        {
          return Problem("Entity set 'ProductDbContext.Producers' is null.");
        }

        producer.OwnerId = User.Identity?.Name ?? string.Empty;
        _ProductDbContext.Producers.Add(producer);
        await _ProductDbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Table));
      }
      return View(producer);
    }

    [Authorize]
    public async Task <IActionResult> Edit(int id)
    {
      if (_ProductDbContext?.Producers == null)
      {
        return Problem("Entity set 'ProductDbContext.Producers' is null.");
      }

      var currentUserId = User.Identity?.Name;
      var producer = await _ProductDbContext.Producers.FirstOrDefaultAsync(p => p.ProducerId == id);

      if (producer == null)
      {
        return NotFound();
      }

      // Check if the current user owns this producer
      if (producer.OwnerId != currentUserId)
      {
        return Redirect($"/Producer/Table?error={Uri.EscapeDataString("You can only edit your own producers")}");
      }

      return View(producer);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task <IActionResult> Edit(Producer producer)
    {
      if (!ModelState.IsValid)
      {
        return View(producer);
      }

      var currentUserId = User.Identity?.Name;
      var existingProducer = await _ProductDbContext.Producers?.FirstOrDefaultAsync(p => p.ProducerId == producer.ProducerId);

      if (existingProducer == null)
      {
        return NotFound();
      }

      // Verify ownership before allowing edit
      if (existingProducer.OwnerId != currentUserId)
      {
        return Redirect($"/Producer/Table?error={Uri.EscapeDataString("You can only edit your own producers")}");
      }

      // Update only allowed fields
      existingProducer.Name = producer.Name;
      existingProducer.Description = producer.Description;
      existingProducer.ImageUrl = producer.ImageUrl;
      // Do not update OwnerId as it should remain unchanged

      await _ProductDbContext.SaveChangesAsync();
      return RedirectToAction(nameof(Table));
    }

    [HttpPost]
    [Authorize]
    public async Task <IActionResult> DeleteConfirmed(int id)
    {
      if (_ProductDbContext?.Producers == null)
      {
        return Problem("Entity set 'ProductDbContext.Producers' is null.");
      }

      var producer = await _ProductDbContext.Producers
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
      if (_ProductDbContext.Products == null)
      {
        return Problem("Entity set 'ProductDbContext.Products' is null.");
      }

      var hasProducts = await _ProductDbContext.Products.AnyAsync(p => p.ProducerId.Equals(id));
      if (hasProducts)
      {
        return RedirectWithMessage("Producer", "Table",
          "Cannot delete producer that has associated products. Please delete all products first.",
          "warning");
      }

      // Delete the producer
      _ProductDbContext.Producers.Remove(producer);
      await _ProductDbContext.SaveChangesAsync();

      return RedirectWithMessage("Producer", "Table", "Producer successfully deleted", "success");
    }

    private IActionResult RedirectWithMessage(string controller, string action, string message, string messageType = "info")
    {
      return Redirect($"/{controller}/{action}?message={Uri.EscapeDataString(message)}&messageType={messageType}");
    }
  }
}