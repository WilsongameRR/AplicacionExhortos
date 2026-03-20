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

            // 1. Guardar diligencias
            ResponseBd respuestaGuardar = _diligenciasRepository.GuardarDiligencias(model);

            if (respuestaGuardar.NoError != 0)
            {
                TempData["ErrorDiligencias"] = string.IsNullOrWhiteSpace(respuestaGuardar.Mensaje)
                    ? "No fue posible guardar las diligencias."
                    : respuestaGuardar.Mensaje;

                return RedirectToAction("AltaDiligencia", new { noExhorto = model.NoExhorto });
            }

            // 2. Enviar exhorto automáticamente
            ResponseBd respuestaEnviar = _diligenciasRepository.ActualizarEstatusExhorto(model.NoExhorto);

            if (respuestaEnviar.NoError != 0)
            {
                TempData["ErrorDiligencias"] = string.IsNullOrWhiteSpace(respuestaEnviar.Mensaje)
                    ? "Las diligencias se guardaron, pero no fue posible enviar el exhorto."
                    : respuestaEnviar.Mensaje;

                TempData["ExitoDiligencias"] = string.IsNullOrWhiteSpace(respuestaGuardar.Mensaje)
                    ? "Diligencias guardadas correctamente."
                    : respuestaGuardar.Mensaje;

                return RedirectToAction("AltaDiligencia", new { noExhorto = model.NoExhorto });
            }

            TempData["ExitoDiligencias"] = string.IsNullOrWhiteSpace(respuestaGuardar.Mensaje)
                ? "Diligencias guardadas correctamente."
                : respuestaGuardar.Mensaje;

            TempData["ExitoEnvio"] = string.IsNullOrWhiteSpace(respuestaEnviar.Mensaje)
                ? "El exhorto fue enviado correctamente."
                : respuestaEnviar.Mensaje;

            return RedirectToAction("AltaDiligencia", new { noExhorto = model.NoExhorto });
        }
    }
}