using Microsoft.AspNetCore.Mvc;
using Food_Registration.Models;
using Food_Registration.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Food_Registration.Controllers;

public class ProductController : Controller
{
  private readonly ProductDbContext _ProductDbContext;

  public ProductController(ProductDbContext ProductDbContext)
  {
    _ProductDbContext = ProductDbContext;
  }


/*
[HttpGet]
public async Task<IActionResult> Index(string ProductSearch)
{
    var productQuery = _ProductDbContext.Products.AsQueryable();

    if (!string.IsNullOrEmpty(ProductSearch))
    {
        productQuery = productQuery.Where(x => x.Name.Contains(ProductSearch) || x.ProductId.ToString().Contains(ProductSearch));
    }

    var products = await productQuery.AsNoTracking().ToListAsync();
    return View(products);
}*/




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