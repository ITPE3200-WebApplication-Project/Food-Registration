using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Food_Registration.Models;
using Microsoft.EntityFrameworkCore;

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

    async public Task<IActionResult> Index(string searching)
    {
        var productQuery = _ProductDbContext.Products.AsQueryable();

        if (!string.IsNullOrEmpty(searching))
        {
            // Use ToLower() to make the search case-insensitive
            productQuery = productQuery.Where(x => x.Name.ToLower().Contains(searching.ToLower()) || x.ProductId.ToString().ToLower().Contains(searching.ToLower()));
        }

        var products = await productQuery.AsNoTracking().ToListAsync();

        return View("~/Views/Home/Index.cshtml", products);
    }

    public ActionResult ReadMore()
    {
        return View();
    }

    public IActionResult NewProduct()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
