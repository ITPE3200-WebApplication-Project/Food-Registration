using Microsoft.AspNetCore.Mvc;
using Food_Registration.Models;
using Food_Registration.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Food_Registration.Controllers;


public class SearchController : Controller
{
    private readonly ProductDbContext _ProductDbContext;

    public SearchController(ProductDbContext ProductDbContext)
    {
        _ProductDbContext = ProductDbContext;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string searching)
    {
        var productQuery = _ProductDbContext.Products.AsQueryable();

        if (!string.IsNullOrEmpty(searching))
        {
            productQuery = productQuery.Where(x => x.Name.Contains(searching) || x.ProductId.ToString().Contains(searching));
        }

        var products = await productQuery.AsNoTracking().ToListAsync();

        return View("~/Views/Home/Search.cshtml", products);
    }
}