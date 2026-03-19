using AplicacionExhortos.Data.Repositories;
using AplicacionExhortos.Models;
using Microsoft.AspNetCore.Mvc;

namespace AplicacionExhortos.Controllers
{
    public class ExhortosRecibidosController : Controller
    {
        private readonly ConsultaExhortoRepository _consultaExhortoRepository;
        private readonly DiligenciasRepository _diligenciasRepository;

        public ExhortosRecibidosController(
            ConsultaExhortoRepository consultaExhortoRepository,
            DiligenciasRepository diligenciasRepository)
        {
            _consultaExhortoRepository = consultaExhortoRepository;
            _diligenciasRepository = diligenciasRepository;
        }

        [HttpGet]
        public IActionResult ExhortosRecibidos()
        {
            string? usuarioId = HttpContext.Session.GetString("UsuarioId");
            int? tuaIdSession = HttpContext.Session.GetInt32("TuaId");

            if (string.IsNullOrWhiteSpace(usuarioId))
            {
                TempData["Error"] = "La sesión expiró. Inicie sesión nuevamente.";
                return RedirectToAction("Login", "Login");
            }

            if (!tuaIdSession.HasValue || tuaIdSession.Value <= 0)
            {
                TempData["Error"] = "No se encontró el TUA del usuario en sesión.";
                return View(new List<ConsultaExhortos>());
            }

            List<ConsultaExhortos> listaExhortos =
                _consultaExhortoRepository.ConsultaExhortosRecibidos(tuaIdSession.Value);

            ViewBag.DebugTuaId = tuaIdSession.Value;
            ViewBag.DebugTotal = listaExhortos.Count;

            return View(listaExhortos);
        }

        [HttpGet]
        public IActionResult RelacionExhorto(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "El identificador del exhorto no es válido.";
                return RedirectToAction("ExhortosRecibidos");
            }

            var model = new DetalleExhortoModel
            {
                Exhorto = new ConsultaExhortos(),
                Diligencias = _diligenciasRepository.ObtenerDiligencias(id)
            };

            return View(model);
        }
    }
}