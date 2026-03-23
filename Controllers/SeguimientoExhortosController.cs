using AplicacionExhortos.Data.Repositories;
using AplicacionExhortos.Models;
using Microsoft.AspNetCore.Mvc;

namespace AplicacionExhortos.Controllers
{
    public class SeguimientoExhortosController : Controller
    {
        private readonly ConsultaExhortoRepository _consultaExhortoRepository;

        public SeguimientoExhortosController(ConsultaExhortoRepository consultaExhortoRepository)
        {
            _consultaExhortoRepository = consultaExhortoRepository;
        }

        [HttpGet]
        public IActionResult SeguimientoExhortos()
        {
            int? tuaIdSession = HttpContext.Session.GetInt32("TuaId");

            if (!tuaIdSession.HasValue || tuaIdSession.Value <= 0)
            {
                TempData["Error"] = "La sesión expiró o no contiene el TUA del usuario.";
                return RedirectToAction("Login", "Login");
            }

            List<ConsultaExhortos> lista =
                _consultaExhortoRepository.ConsultaSeguimientoExhortos(tuaIdSession.Value);

            return View(lista);
        }
    }
}