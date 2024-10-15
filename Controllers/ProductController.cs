using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using webapplikasjon.Models;

namespace webapplikasjon.Controllers;

public class ProductController : Controller
{
    private readonly ProductDB _db;

    public ProductController(ProductDB db)
    {
        _db = db;
    }


[HttpGet]
     public async Task<IActionResult> index (string ProductSearch)
    {
      // ViewData["Getproductlist"] = ProductSearch; // dersom du bruker value i html da kan du bruker @ViewData["Getproductlist"]
       var Products = await _repository.GetAllAsync(); // dersom du ikke bruker ViewData["Getproductlist"] i html index kan du denne
        var productQuery = from x in _db.ProductTable select x;
        if (!string.IsNullOrEmpty(ProductSearch))
        {
            Products = Products.where (x=>x.ProductName.comtains(ProductSearch) || x.ProductId.contains(ProductSearch));
           

        }
        return View(await Products.AsnoTracking().TolistAsync());
    }

}