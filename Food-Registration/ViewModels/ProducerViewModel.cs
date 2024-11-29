using Food_Registration.Models;

namespace Food_Registration.ViewModels 
{
  public class ProducerViewModel
{
    public IEnumerable<Producer> Producers { get; set; }
    public string? CurrentViewName { get; set; }

    public ProducerViewModel(IEnumerable<Producer> producers, string? currentViewName)
    {
        Producers = producers;
        CurrentViewName = currentViewName;
    }
}
}