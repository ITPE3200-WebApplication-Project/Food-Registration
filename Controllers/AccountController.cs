
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using webapplikasjon.Models;

namespace webapplikasjon.Controllers;

public class AccountController : Controller
{
    public IActionResult Login()
    {
        return View();
    }


    public IActionResult SignUp()
    {
        return View();
    }
}
