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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
