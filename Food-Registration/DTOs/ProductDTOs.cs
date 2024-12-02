using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Food_Registration.DTOs;


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

  [JsonPropertyName("imageBase64")]
  public string? ImageBase64 { get; set; }

  [JsonPropertyName("imageFileExtension")]
  public string? ImageFileExtension { get; set; }
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

  [JsonPropertyName("imageBase64")]
  public string? ImageBase64 { get; set; }

  [JsonPropertyName("imageFileExtension")]
  public string? ImageFileExtension { get; set; }
}