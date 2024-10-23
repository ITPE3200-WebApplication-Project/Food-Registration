using Microsoft.AspNetCore.Mvc;
using Food_Registration.Models;
using Food_Registration.ViewModels;

namespace Food_Registration.Controllers;

public class ProductController : Controller
{
  private readonly ProductDbContext _ProductDbContext;

  public ProductController(ProductDbContext ProductDbContext)
  {
    _ProductDbContext = ProductDbContext;
  }

  public IActionResult Table()
  {
    List<Product> products = _ProductDbContext.Products.ToList();
    var ProductsViewModel = new ProductsViewModel(products, "Table");

    return View(ProductsViewModel);
  }

  public IActionResult Grid()
  {
    List<Product> products = _ProductDbContext.Products.ToList();
    var ProductsViewModel = new ProductsViewModel(products, "Grid");
    return View(ProductsViewModel);
  }

  public IActionResult Details(int id)
  {
    List<Product> Products = _ProductDbContext.Products.ToList();
    var product = Products.FirstOrDefault(i => i.ProductId == id);
    if (product == null)
      return NotFound();
    return View(product);
  }
}