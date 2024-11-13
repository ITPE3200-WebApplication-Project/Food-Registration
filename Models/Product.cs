namespace Food_Registration.Models

{
  public class Product
  {
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Category { get; set; }
    public decimal? Calories {get; set; }
    public decimal? Carbohydrates {get; set; }
    public decimal? Fat {get; set; }
    public decimal? Protein {get; set; } 
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    public string ProducerId { get; set;} = string.Empty;

    // public virtual Producer? Producer { get; set; } // Virtual gir lazy loading, default unngår null-warnings
  }
}