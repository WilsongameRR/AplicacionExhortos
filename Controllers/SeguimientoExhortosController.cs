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

            string? usuario = HttpContext.Session.GetString("UsuarioId");

            if (string.IsNullOrWhiteSpace(usuario))
            {
                TempData["Error"] = "La sesión expiró. Inicie sesión nuevamente.";
                return RedirectToAction("Login", "Login");
            }

            // Primero obtenemos el detalle actual
            ConsultaExhortos? exhorto =
                _consultaExhortoRepository.ObtenerDetalleExhortoRecibido(id);

            if (exhorto == null)
            {
                TempData["Error"] = "No se encontró información del exhorto.";
                return RedirectToAction(nameof(SeguimientoExhortos));
            }

            // Si sigue pendiente, asigna número de exhorto recibido
            // y con eso debe pasar a trámite según tu SP
            if (!string.IsNullOrWhiteSpace(exhorto.Estatus) &&
                exhorto.Estatus.Trim().ToUpper() == "PENDIENTE")
            {
                _consultaExhortoRepository.AsignarExhortoRecibido(id, usuario);

                // Volver a consultar ya actualizado
                exhorto = _consultaExhortoRepository.ObtenerDetalleExhortoRecibido(id);

                if (exhorto == null)
                {
                    TempData["Error"] = "No se encontró información del exhorto después de actualizar.";
                    return RedirectToAction(nameof(SeguimientoExhortos));
                }
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