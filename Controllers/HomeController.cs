using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Food_Registration.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Food_Registration.Controllers;

public class HomeController : Controller
{
    private readonly ProductDbContext _ProductDbContext;

    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, ProductDbContext ProductDbContext)
    {
        _logger = logger;
        _ProductDbContext = ProductDbContext;
    }

    async public Task<IActionResult> Index(string searching, string category)
    {
        if (_ProductDbContext?.Products == null)
        {
            return Problem("Entity set 'ProductDbContext.Products' is null.");
        }

        var productQuery = _ProductDbContext.Products.AsQueryable();

        if (!string.IsNullOrEmpty(searching))
        {
            // Use ToLower() to make the search case-insensitive
            productQuery = productQuery.Where(x => x.Name.ToLower().Contains(searching.ToLower()) || x.ProductId.ToString().ToLower().Contains(searching.ToLower()));
        }

        // Legg til kategorifiltrering hvis en kategori er spesifisert
        if (!string.IsNullOrEmpty(category))
        {
            productQuery = productQuery.Where(x => x.Category != null && x.Category.ToLower() == category.ToLower());
        }

        var products = await productQuery.AsNoTracking().ToListAsync();

        return View("~/Views/Home/Index.cshtml", products);
    }

    public ActionResult ReadMore()
    {
        return View();
    }

    public ActionResult Category()
    {
        return View();
    }

    [Authorize]
    public IActionResult NewProduct()
    {
        var currentUserId = User.Identity?.Name;

        if (string.IsNullOrEmpty(currentUserId))
        {
            return Redirect($"/Producer/Index?error={Uri.EscapeDataString("Please create a producer account first")}");
        }

        // Get producers owned by current user
        var userProducers = _ProductDbContext.Producers?
            .Where(p => p.OwnerId == currentUserId)
            .ToList();

        if (userProducers == null || !userProducers.Any())
        {
            return Redirect($"/Producer/Index?error={Uri.EscapeDataString("Please create a producer account first")}");
        }

        // Create SelectList for producers dropdown
        ViewBag.Producers = new SelectList(userProducers, "ProducerId", "Name");

        // Create SelectList for categories dropdown
        var categories = new List<string>
        {
            "Fruits",
            "Vegetables",
            "Meat",
            "Fish",
            "Dairy",
            "Grains",
            "Beverages",
            "Snacks",
            "Other"
        };
        ViewBag.Categories = new SelectList(categories);

        return View();
    }

    [Authorize]
    [HttpPost]
    public IActionResult NewProduct(Product Products)
    {
        if (ModelState.IsValid && _ProductDbContext != null)
        {
            _ProductDbContext?.Products?.Update(Products);
            _ProductDbContext?.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        return View(Products);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
