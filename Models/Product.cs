using System;
namespace Food_Registration.Models

{
  public class Product
  {
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
  }
}