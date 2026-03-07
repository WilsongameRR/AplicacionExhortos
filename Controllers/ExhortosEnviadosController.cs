using Microsoft.AspNetCore.Mvc;

namespace AplicacionExhortos.Controllers
{
    public class ExhortosEnviadosController : Controller
    {
        public IActionResult ExhortosEnviados()
        {
            return View();
        }
    }
}