namespace Food_Registration.Models
using System.ComponentModel.DataAnnotations;

{
  public class Product
  {
    public int ProductId { get; set; }
    public int ProducerId { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public string? Category { get; set; }
    [Range(0, (double)decimal.MaxValue, ErrorMessage = "Calories cannot be negative.")]    
    public decimal? Calories { get; set; }
    
    [Range(0, (double)decimal.MaxValue, ErrorMessage = "Carbohydrates cannot be negative.")]
    public decimal? Carbohydrates { get; set; }
    
    [Range(0, (double)decimal.MaxValue, ErrorMessage = "Fat cannot be negative.")]
    public decimal? Fat { get; set; }
    
    [Range(0, (double)decimal.MaxValue, ErrorMessage = "Protein cannot be negative.")]
    public decimal? Protein { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    public string? NutritionScore { get; set; }
    public Producer? Producer { get; set; }
  }
}