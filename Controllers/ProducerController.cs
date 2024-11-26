using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Food_Registration.Models;

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
    public IActionResult Index()
    {
      if (_ProductDbContext?.Producers == null)
      {
        return Problem("Entity set 'ProductDbContext.Producers' is null.");
      }

      var currentUserId = User.Identity?.Name;
      Console.WriteLine(currentUserId);
      var producers = _ProductDbContext.Producers
        .Where(p => p.OwnerId == currentUserId)
        .ToList();

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
        return RedirectToAction(nameof(Index));
      }
      return View(producer);
    }

    [Authorize]
    public IActionResult Edit(int id)
    {
      if (_ProductDbContext?.Producers == null)
      {
        return Problem("Entity set 'ProductDbContext.Producers' is null.");
      }

      var currentUserId = User.Identity?.Name;
      var producer = _ProductDbContext.Producers.FirstOrDefault(p => p.ProducerId == id);

      if (producer == null)
      {
        return NotFound();
      }

      // Check if the current user owns this producer
      if (producer.OwnerId != currentUserId)
      {
        return Redirect($"/Producer/Index?error={Uri.EscapeDataString("You can only edit your own producers")}");
      }

      return View(producer);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Producer producer)
    {
      if (ModelState.IsValid)
      {
        var currentUserId = User.Identity?.Name;
        var existingProducer = _ProductDbContext.Producers?.FirstOrDefault(p => p.ProducerId == producer.ProducerId);

        if (existingProducer == null)
        {
          return NotFound();
        }

        if (existingProducer.OwnerId != currentUserId)
        {
          return Redirect($"/Producer/Index?error={Uri.EscapeDataString("You can only edit your own producers")}");
        }

        // Update only allowed fields
        existingProducer.Name = producer.Name;
        existingProducer.Description = producer.Description;
        existingProducer.ImageUrl = producer.ImageUrl;

        _ProductDbContext.SaveChanges();
        return RedirectToAction(nameof(Index));
      }
      return View(producer);
    }
  }
}