using Microsoft.AspNetCore.Mvc;

namespace AplicacionExhortos.Controllers
{
    public class ExhortosPendientesController : Controller
    {
        public IActionResult ExhortosPendientes()
        {
            return View();
        }
    }
}