
namespace Food_Registration.Models


{
  public class Producer
  {
    public int ProducerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
  }
}