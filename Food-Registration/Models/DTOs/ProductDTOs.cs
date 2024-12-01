using System.ComponentModel.DataAnnotations;

namespace Food_Registration.Models.DTOs;


public class CreateProductDTO
{
  [Required]
  public string Name { get; set; } = string.Empty;

  public string Description { get; set; } = string.Empty;

  [Required]
  public string NutritionScore { get; set; } = string.Empty;

  [Required]
  public string Category { get; set; } = string.Empty;

  [Required]
  public int ProducerId { get; set; }

  public int Carbohydrates { get; set; }

  public int Fat { get; set; }

  public int Protein { get; set; }

  public int Calories { get; set; }

  [Required]
  public IFormFile? ImageFile { get; set; }
}

public class UpdateProductDTO
{
  [Required]
  public int ProductId { get; set; }

  [Required]
  public string Name { get; set; } = string.Empty;

  [Required]
  public string Description { get; set; } = string.Empty;

  [Required]
  public string NutritionScore { get; set; } = string.Empty;

  [Required]
  public string Category { get; set; } = string.Empty;

  [Required]
  public int ProducerId { get; set; }

  [Required]
  public int Carbohydrates { get; set; } = 0;

  [Required]
  public int Fat { get; set; } = 0;

  [Required]
  public int Protein { get; set; } = 0;

  [Required]
  public int Calories { get; set; } = 0;

  public IFormFile? ImageFile { get; set; }
}