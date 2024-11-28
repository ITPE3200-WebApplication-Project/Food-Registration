using Food_Registration.Models;

namespace Food_Registration.ViewModels
{
  public class ProductsViewModel
  {
    public IEnumerable<Product> Products { get; set; }
    public string? CurrentViewName { get; set; }

    public ProductsViewModel(IEnumerable<Product> products, string? currentViewName)
    {
      Products = products;
      CurrentViewName = currentViewName;
    }
  }
}