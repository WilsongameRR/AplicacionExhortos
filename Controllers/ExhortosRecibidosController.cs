using AplicacionExhortos.Data.Repositories;
using AplicacionExhortos.Models;
using Microsoft.AspNetCore.Mvc;

namespace AplicacionExhortos.Controllers
{
    public class ExhortosRecibidosController : SessionControllerBase
    {
        private readonly ConsultaExhortoRepository _consultaExhortoRepository;
        private readonly DiligenciasRepository _diligenciasRepository;
        private readonly DocumentosRepository _documentosRepository;

        public ExhortosRecibidosController(
            ConsultaExhortoRepository consultaExhortoRepository,
            DiligenciasRepository diligenciasRepository,
            DocumentosRepository documentosRepository)
        {
            _consultaExhortoRepository = consultaExhortoRepository;
            _diligenciasRepository = diligenciasRepository;
            _documentosRepository = documentosRepository;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet]
        public IActionResult ExhortosRecibidos()
        {
            if (!TryObtenerUsuarioIdSesion(out _))
            {
                return RedirigirALoginPorSesionExpirada();
            }

            if (!TryObtenerTuaIdSesion(out int tuaIdSession))
            {
                TempData["Error"] = "No se encontró el TUA del usuario en sesión.";
                return View(new List<ConsultaExhortos>());
            }

            List<ConsultaExhortos> listaExhortos =
                _consultaExhortoRepository.ConsultaExhortosRecibidos(tuaIdSession);

            return View(listaExhortos);
        }

        [HttpGet]
        public IActionResult DarTramite(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "El identificador del exhorto no es válido.";
                return RedirectToAction(nameof(ExhortosRecibidos));
            }

            if (!TryObtenerTuaIdSesion(out _))
            {
                return RedirigirALoginPorSesionExpirada("La sesión expiró o no contiene el TUA del usuario.");
            }

            if (!TryObtenerUsuarioIdSesion(out string usuarioId))
            {
                return RedirigirALoginPorSesionExpirada();
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
            if (id <= 0)
            {
                TempData["Error"] = "El identificador del exhorto no es válido.";
                return RedirectToAction(nameof(ExhortosRecibidos));
            }

            if (!TryObtenerTuaIdSesion(out _))
            {
                return RedirigirALoginPorSesionExpirada("La sesión expiró o no contiene el TUA del usuario.");
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
            if (exhortoId <= 0)
            {
                TempData["Error"] = "El identificador del exhorto no es válido.";
                return RedirectToAction(nameof(ExhortosRecibidos));
            }

            if (!TryObtenerTuaIdSesion(out _))
            {
                return RedirigirALoginPorSesionExpirada("La sesión expiró o no contiene el TUA del usuario.");
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
