using Microsoft.AspNetCore.Mvc;

namespace Food_Registration.Extensions
{
  public static class ControllerExtensions
  {
    public static IActionResult RedirectWithError(this Controller controller, string action, string error)
    {
      return controller.Redirect($"/{controller.ControllerContext.ActionDescriptor.ControllerName}/{action}?error={Uri.EscapeDataString(error)}");
    }
  }
}