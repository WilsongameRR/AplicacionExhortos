using Microsoft.AspNetCore.Mvc;

namespace AplicacionExhortos.Controllers
{
    public class AltaDeExhortosController : Controller
    {
        [HttpGet]
        public IActionResult AltaDocumentos()
        {
            return View(); 
        }
    }
}