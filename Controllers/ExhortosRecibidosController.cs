using AplicacionExhortos.Data.Repositories;
using AplicacionExhortos.Models;
using Microsoft.AspNetCore.Mvc;

namespace AplicacionExhortos.Controllers
{
    public class ExhortosRecibidosController : Controller
    {
        private readonly ConsultaExhortoRepository _consultaExhortoRepository;
        private readonly DiligenciasRepository _diligenciasRepository;
        private readonly ExhortosRepository _exhortosRepository;
        private readonly DocumentosRepository _documentosRepository;

        public ExhortosRecibidosController(
            ConsultaExhortoRepository consultaExhortoRepository,
            DiligenciasRepository diligenciasRepository,
            ExhortosRepository exhortosRepository,
            DocumentosRepository documentosRepository)
        {
            _consultaExhortoRepository = consultaExhortoRepository;
            _diligenciasRepository = diligenciasRepository;
            _exhortosRepository = exhortosRepository;
            _documentosRepository = documentosRepository;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
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

            return View(listaExhortos);
        }

        [HttpGet]
        public IActionResult DarTramite(int id)
        {
            int? tuaIdSession = HttpContext.Session.GetInt32("TuaId");
            string? usuarioId = HttpContext.Session.GetString("UsuarioId");

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

            if (string.IsNullOrWhiteSpace(usuarioId))
            {
                TempData["Error"] = "La sesión expiró. Inicie sesión nuevamente.";
                return RedirectToAction("Login", "Login");
            }

            ConsultaExhortos? exhorto =
                _consultaExhortoRepository.ObtenerDetalleExhortoRecibido(id);

            if (exhorto == null)
            {
                TempData["Error"] = "No se encontró el detalle del exhorto.";
                return RedirectToAction(nameof(ExhortosRecibidos));
            }

            string estatusActual = (exhorto.Estatus ?? string.Empty).Trim().ToUpperInvariant();

            if (estatusActual == "PENDIENTE")
            {
                string? noExhortoRecibido =
                    _consultaExhortoRepository.AsignarExhortoRecibido(id, usuarioId);

                if (string.IsNullOrWhiteSpace(noExhortoRecibido))
                {
                    TempData["Error"] = "No fue posible asignar el exhorto recibido.";
                    return RedirectToAction(nameof(ExhortosRecibidos));
                }

                TempData["Exito"] = $"El exhorto pasó de PENDIENTE a TRAMITE. No. Exhorto recibido: {noExhortoRecibido}";
            }
            else if (estatusActual == "TRAMITE")
            {
                TempData["Exito"] = "El exhorto ya se encuentra en TRAMITE.";
            }
            else
            {
                TempData["Error"] = $"El exhorto no se puede tramitar porque su estatus actual es: {exhorto.Estatus}";
                return RedirectToAction(nameof(ExhortosRecibidos));
            }

            return RedirectToAction(nameof(DetalleExhorto), new { id });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
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
                Diligencias = _diligenciasRepository.ObtenerDiligencias(id),
                DocumentosAdjuntos = _documentosRepository.ObtenerDocumentosAdjuntos(exhorto.ExhortoId)
            };

            return View("RelacionExhorto", model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet]
        public IActionResult RelacionExhorto(int exhortoId)
        {
            int? tuaIdSession = HttpContext.Session.GetInt32("TuaId");

            if (exhortoId <= 0)
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
                _consultaExhortoRepository.ObtenerDetalleExhortoRecibido(exhortoId);

            if (exhorto == null)
            {
                TempData["Error"] = "No se encontró el detalle del exhorto.";
                return RedirectToAction(nameof(ExhortosRecibidos));
            }

            DetalleExhortoModel model = new()
            {
                Exhorto = exhorto,
                Diligencias = _diligenciasRepository.ObtenerDiligencias(exhortoId),
                DocumentosAdjuntos = _documentosRepository.ObtenerDocumentosAdjuntos(exhorto.ExhortoId)
            };

            return View("RelacionExhorto", model);
        }
    }
}