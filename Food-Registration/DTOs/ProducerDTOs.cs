using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Food_Registration.DTOs;

public class CreateProducerDTO
{
  [Required]
  public string Name { get; set; } = string.Empty;

  [Required]
  public string Description { get; set; } = string.Empty;

  [Required]
  [JsonPropertyName("imageBase64")]
  public string? ImageBase64 { get; set; }

  [Required]
  [JsonPropertyName("imageFileExtension")]
  public string? ImageFileExtension { get; set; }
}

public class UpdateProducerDTO
{
  [Required]
  public int ProducerId { get; set; }

  [Required]
  public string Name { get; set; } = string.Empty;

  [Required]
  public string Description { get; set; } = string.Empty;

  [JsonPropertyName("imageBase64")]
  public string? ImageBase64 { get; set; }

  [JsonPropertyName("imageFileExtension")]
  public string? ImageFileExtension { get; set; }
}

