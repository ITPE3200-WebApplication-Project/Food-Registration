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
  }
}