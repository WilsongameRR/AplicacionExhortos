using AplicacionExhortos.Data.Repositories;
using AplicacionExhortos.Models;
using Microsoft.AspNetCore.Mvc;

namespace AplicacionExhortos.Controllers
{
    public class SeguimientoExhortosController : Controller
    {
        private readonly ConsultaExhortoRepository _consultaExhortoRepository;
        private readonly DiligenciasRepository _diligenciasRepository;

        public SeguimientoExhortosController(
            ConsultaExhortoRepository consultaExhortoRepository,
            DiligenciasRepository diligenciasRepository)
        {
            _consultaExhortoRepository = consultaExhortoRepository;
            _diligenciasRepository = diligenciasRepository;
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

        [HttpGet]
        public IActionResult DetalleSeguimiento(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "El identificador del exhorto no es válido.";
                return RedirectToAction(nameof(SeguimientoExhortos));
            }

            ConsultaExhortos? exhorto =
                _consultaExhortoRepository.ObtenerDetalleExhortoRecibido(id);

            if (exhorto == null)
            {
                TempData["Error"] = "No se encontró información del exhorto.";
                return RedirectToAction(nameof(SeguimientoExhortos));
            }

            DetalleExhortoModel model = new()
            {
                Exhorto = exhorto,
                Diligencias = _diligenciasRepository.ObtenerDiligencias(id)
            };

            return View(model);
        }
    }
}