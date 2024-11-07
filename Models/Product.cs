namespace Food_Registration.Models

{
  public class Product
  {
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Category { get; set; }
    public decimal? Calorie {get; set; }
    public decimal? Carbohydrate {get; set; }
    public decimal? Fat {get; set; }
    public decimal? Protein {get; set; } 
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
  }
}