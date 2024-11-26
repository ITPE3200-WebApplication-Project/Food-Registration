using Microsoft.AspNetCore.Mvc;
using Food_Registration.Models;
using Food_Registration.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Food_Registration.Controllers;

[Authorize]
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




  [Authorize]
  public IActionResult Table()
  {
    List<Product> products = _ProductDbContext.Products?.ToList() ?? new List<Product>();
    var ProductsViewModel = new ProductsViewModel(products, "Table");

    return View(ProductsViewModel);
  }

  [Authorize]
  public IActionResult Grid()
  {
    List<Product> products = _ProductDbContext.Products?.ToList() ?? new List<Product>();
    var ProductsViewModel = new ProductsViewModel(products, "Grid");
    return View(ProductsViewModel);
  }

  public IActionResult Details(int id)
  {
    List<Product> Products = _ProductDbContext.Products?.ToList() ?? new List<Product>();
    var product = Products?.FirstOrDefault(i => i.ProductId == id);
    if (product == null)
      return NotFound();
    return View(product);
  }


  [HttpGet]
  public IActionResult Update(int id)
  {
    var product = _ProductDbContext.Products?.FirstOrDefault(p => p.ProductId == id);
    if (product == null)
    {
      return NotFound();
    }
    return View(product); // View Only product taht is choiced
  }

  [HttpPost]
  public IActionResult Update(Product Products)
  {
    if (ModelState.IsValid)
    {
      if (_ProductDbContext.Products == null)
      {
        return Problem("Entity set 'ProductDbContext.Products' is null.");
      }
      _ProductDbContext.Products.Update(Products);
      _ProductDbContext.SaveChanges();
      return RedirectToAction(nameof(Table));
    }
    return View(Products);//show if there is feil.
  }

  [HttpGet]
  public IActionResult Delete(int id)
  {
    var item = _ProductDbContext.Products?.Find(id);
    if (item == null)
    {
      return NotFound();
    }
    return View(item);
  }

  [HttpPost]
  public IActionResult DeleteConfirmed(int id)
  {
    if (_ProductDbContext.Products == null)
    {
      return Problem("Entity set 'ProductDbContext.Products' is null.");
    }

    var item = _ProductDbContext.Products.Find(id);
    if (item == null)
    {
      return NotFound();
    }
    _ProductDbContext.Products.Remove(item);
    _ProductDbContext.SaveChanges();

    //Set ViewBag.DeletionSuccess to true when the product is successfully deleted.
    ViewBag.DeletionSuccess = true;

    return RedirectToAction(nameof(Table));
  }
}
