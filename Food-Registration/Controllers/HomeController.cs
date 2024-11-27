using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Food_Registration.Models;
using Microsoft.EntityFrameworkCore;
using Food_Registration.DAL;

namespace Food_Registration.Controllers;

public class HomeController : Controller
{
    private readonly ItemDbContext _ItemDbContext;

    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, ItemDbContext ItemDbContext)
    {
        _logger = logger;
        _ItemDbContext = ItemDbContext;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
