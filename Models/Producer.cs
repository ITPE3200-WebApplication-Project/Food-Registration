using System;
namespace Food_Registration.Models

{
  public class Producer
  {
    public int producerId { get; set; }
    public string name { get; set; } = string.Empty;
    public string? description { get; set; }
    public string? imageUrl { get; set; }
  }
}