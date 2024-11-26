namespace Food_Registration.Models

{
  public class Product
  {
    public int ProductId { get; set; }
    public int ProducerId { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public string? Category { get; set; }
    public decimal? Calories { get; set; }
    public decimal? Carbohydrates { get; set; }
    public decimal? Fat { get; set; }
    public decimal? Protein { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public Producer? Producer { get; set; }
  }
}