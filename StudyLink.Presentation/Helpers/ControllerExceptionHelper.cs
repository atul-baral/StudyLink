using Azure.Core;
using Microsoft.AspNetCore.Mvc;

namespace StudyLink.Presentation.Helpers
{
    public static class ControllerExceptionHelper
    {
        public static IActionResult Handle(this Controller controller, string errorMessage)
        {
            Console.WriteLine($"An error occurred: {errorMessage}");
            controller.TempData["Error"] = "An error occurred.";
            var refererUrl = controller.Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(refererUrl))
            {
                return controller.Redirect(refererUrl);
            }

            return controller.RedirectToAction("Index", "Home");
        }
    }

}
