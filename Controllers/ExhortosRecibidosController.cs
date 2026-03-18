using Microsoft.AspNetCore.Mvc;

namespace AplicacionExhortos.Controllers
{
    public class ExhortosRecibidosController : Controller
    {
        public IActionResult ExhortosRecibidos()
        {
            return View();
        }
    }
}