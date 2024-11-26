using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Food_Registration.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

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
        var productQuery = _ProductDbContext.Products.AsQueryable();

        if (!string.IsNullOrEmpty(searching))
        {
            // Use ToLower() to make the search case-insensitive
            productQuery = productQuery.Where(x => x.Name.ToLower().Contains(searching.ToLower()) || x.ProductId.ToString().ToLower().Contains(searching.ToLower()));
        }

        // Legg til kategorifiltrering hvis en kategori er spesifisert
        if (!string.IsNullOrEmpty(category))
        {
            productQuery = productQuery.Where(x => x.Category.ToLower() == category.ToLower());
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
        return View();
    }

    [Authorize]
    [HttpPost]
    public IActionResult NewProduct(Product Products)
    {
        if (ModelState.IsValid)
        {
            _ProductDbContext.Products.Update(Products);
            _ProductDbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        return View(Products);//show if there is feil.
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
