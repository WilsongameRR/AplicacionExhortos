using AplicacionExhortos.Data.Repositories;
using AplicacionExhortos.Models;
using Microsoft.AspNetCore.Mvc;

namespace AplicacionExhortos.Controllers
{
    public class DiligenciasController : Controller
    {
        private readonly DiligenciasRepository _diligenciasRepository;

        public DiligenciasController(DiligenciasRepository diligenciasRepository)
        {
            _diligenciasRepository = diligenciasRepository;
        }

        [HttpGet]
        public IActionResult AltaDiligencia(string? noExhorto = null)
        {
            AltaDiligenciasModel model = new AltaDiligenciasModel
            {
                NoExhorto = noExhorto ?? string.Empty
            };

            ViewBag.ExitoEnvio = TempData["ExitoEnvio"]?.ToString();
            ViewBag.ErrorDiligencias = TempData["ErrorDiligencias"]?.ToString();
            ViewBag.ExitoDiligencias = TempData["ExitoDiligencias"]?.ToString();
            ViewBag.MensajeExito = TempData["MensajeExito"]?.ToString();
            ViewBag.NumeroExhorto = TempData["NumeroExhorto"]?.ToString();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GuardarDiligencias(AltaDiligenciasModel model)
        {
            if (string.IsNullOrWhiteSpace(model.NoExhorto))
            {
                TempData["ErrorDiligencias"] = "Debe capturar el número de exhorto.";
                return View("AltaDiligencia", model);
            }

            if (model.Diligencias == null || !model.Diligencias.Any())
            {
                TempData["ErrorDiligencias"] = "Debe agregar al menos una diligencia.";
                return View("AltaDiligencia", model);
            }

            ResponseBd respuestaGuardar = _diligenciasRepository.GuardarDiligencias(model);

            if (respuestaGuardar.NoError != 0)
            {
                TempData["ErrorDiligencias"] = string.IsNullOrWhiteSpace(respuestaGuardar.Mensaje)
                    ? "No fue posible guardar las diligencias."
                    : respuestaGuardar.Mensaje;

                return RedirectToAction("AltaDiligencia", new { noExhorto = model.NoExhorto });
            }

            ResponseBd respuestaEnviar = _diligenciasRepository.ActualizarEstatusExhorto(model.NoExhorto);

            if (respuestaEnviar.NoError != 0)
            {
                TempData["ErrorDiligencias"] = string.IsNullOrWhiteSpace(respuestaEnviar.Mensaje)
                    ? "Las diligencias se guardaron, pero no fue posible enviar el exhorto."
                    : respuestaEnviar.Mensaje;

                TempData["ExitoDiligencias"] = null;
                TempData["ExitoEnvio"] = null;
                TempData["BloquearDiligencias"] = null;

                return RedirectToAction("AltaDiligencia", new { noExhorto = model.NoExhorto });
            }

            TempData["ExitoDiligencias"] = null;
            TempData["ExitoEnvio"] = "El exhorto ha sido enviado al TUA Exhortado";
            TempData["BloquearDiligencias"] = true;

            return RedirectToAction("AltaDocumentos", "Exhortos", new
            {
                noExhorto = model.NoExhorto
            });
        }

        [HttpGet]
        public IActionResult SeguimientoDiligencia(int exhortoId, int diligenciaId)
        {
            if (exhortoId <= 0)
            {
                TempData["Error"] = "El identificador del exhorto no es válido.";
                return RedirectToAction("SeguimientoExhortos", "SeguimientoExhortos");
            }

            if (diligenciaId <= 0)
            {
                TempData["Error"] = "El identificador de la diligencia no es válido.";
                return RedirectToAction("DetalleSeguimiento", "SeguimientoExhortos", new { exhortoId });
            }

            DiligenciaModel? model = _diligenciasRepository.ObtenerDiligenciaPorId(exhortoId, diligenciaId);

            if (model == null)
            {
                TempData["Error"] = "No se encontró la diligencia.";
                return RedirectToAction("DetalleSeguimiento", "SeguimientoExhortos", new { exhortoId });
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GuardarSeguimientoDiligencia(DiligenciaModel model)
        {
            if (model.ExhortoId <= 0 || model.DiligenciaId <= 0 || model.DiligenciaNoEnvio <= 0)
            {
                TempData["Error"] = "No se identificó correctamente la diligencia.";
                return RedirectToAction("SeguimientoDiligencia", new
                {
                    exhortoId = model.ExhortoId,
                    diligenciaId = model.DiligenciaId
                });
            }

            if (!model.FechaDiligencia.HasValue)
            {
                TempData["Error"] = "Debe capturar la fecha de diligencia.";
                return RedirectToAction("SeguimientoDiligencia", new
                {
                    exhortoId = model.ExhortoId,
                    diligenciaId = model.DiligenciaId
                });
            }

            DateTime hoy = DateTime.Today;
            DateTime fechaMinima = hoy.AddYears(-1);

            if (model.FechaDiligencia.Value.Date > hoy)
            {
                TempData["Error"] = "La fecha de diligencia no puede ser mayor a la fecha actual.";
                return RedirectToAction("SeguimientoDiligencia", new
                {
                    exhortoId = model.ExhortoId,
                    diligenciaId = model.DiligenciaId
                });
            }

            if (model.FechaDiligencia.Value.Date < fechaMinima)
            {
                TempData["Error"] = "La fecha de diligencia no puede ser mayor a un año anterior.";
                return RedirectToAction("SeguimientoDiligencia", new
                {
                    exhortoId = model.ExhortoId,
                    diligenciaId = model.DiligenciaId
                });
            }

            if (string.IsNullOrWhiteSpace(model.EstatusDiligencia))
            {
                TempData["Error"] = "Debe seleccionar un estatus.";
                return RedirectToAction("SeguimientoDiligencia", new
                {
                    exhortoId = model.ExhortoId,
                    diligenciaId = model.DiligenciaId
                });
            }

            ResponseBd respuesta = _diligenciasRepository.ActualizarDiligencia(model);

            if (respuesta.NoError != 0)
            {
                TempData["Error"] = string.IsNullOrWhiteSpace(respuesta.Mensaje)
                    ? "No fue posible actualizar la diligencia."
                    : respuesta.Mensaje;

                return RedirectToAction("SeguimientoDiligencia", new
                {
                    exhortoId = model.ExhortoId,
                    diligenciaId = model.DiligenciaId
                });
            }

            TempData["Exito"] = string.IsNullOrWhiteSpace(respuesta.Mensaje)
                ? "Diligencia actualizada correctamente."
                : respuesta.Mensaje;

            return RedirectToAction("SeguimientoDiligencia", new
            {
                exhortoId = model.ExhortoId,
                diligenciaId = model.DiligenciaId
            });
        }
    }
}