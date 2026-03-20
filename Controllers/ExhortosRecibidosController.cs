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
            string? numTuaSession = HttpContext.Session.GetString("NumTua");

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


            return View(listaExhortos);
        }

        [HttpGet]
        public IActionResult DetalleExhorto(int id)
        {
            int? tuaIdSession = HttpContext.Session.GetInt32("TuaId");

            if (id <= 0)
            {
                TempData["Error"] = "El identificador del exhorto no es válido.";
                return RedirectToAction(nameof(ExhortosRecibidos));
            }

            if (!tuaIdSession.HasValue || tuaIdSession.Value <= 0)
            {
                TempData["Error"] = "La sesión expiró o no contiene el TUA del usuario.";
                return RedirectToAction("Login", "Login");
            }

            ConsultaExhortos? exhorto =
                _consultaExhortoRepository.ObtenerDetalleExhortoRecibido(id);

            if (exhorto == null)
            {
                TempData["Error"] = "No se encontró el detalle del exhorto.";
                return RedirectToAction(nameof(ExhortosRecibidos));
            }

            DetalleExhortoModel model = new()
            {
                Exhorto = exhorto,
                Diligencias = _diligenciasRepository.ObtenerDiligencias(id)
            };

            return View("RelacionExhorto", model);
        }
    }
}