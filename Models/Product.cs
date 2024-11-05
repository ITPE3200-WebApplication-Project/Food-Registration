namespace Food_Registration.Models

{
  public class Product
  {
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Category { get; set; }
    public int? Calorie {get; set; }
    public int? Carbonhydrate {get; set; }
    public int? Fat {get; set; }
    public int? Protein {get; set; } 
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
  }
}