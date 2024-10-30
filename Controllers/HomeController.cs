using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using webapplikasjon.Models;

namespace webapplikasjon.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public ActionResult ReadMore()
    {
        return View();
    }
    
    public ActionResult Category()
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
