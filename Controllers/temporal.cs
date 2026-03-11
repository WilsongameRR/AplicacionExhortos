using AplicacionExhortos.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace AplicacionExhortos.Controllers
{
    public class TemporalController : Controller
    {
        public IActionResult GenerarPassword()
        {
            string password = "54321";
            string hash = PasswordGenerator.GenerarHash(password);

            return Content(hash);
        }
    }
}