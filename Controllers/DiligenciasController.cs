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

            ViewBag.MostrarConfirmacionFinal = TempData["MostrarConfirmacionFinal"] != null;
            ViewBag.MostrarPanelEnvio = TempData["MostrarPanelEnvio"] != null;
            ViewBag.ExitoEnvio = TempData["ExitoEnvio"]?.ToString();
            ViewBag.ErrorDiligencias = TempData["ErrorDiligencias"]?.ToString();
            ViewBag.ExitoDiligencias = TempData["ExitoDiligencias"]?.ToString();

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

            ResponseBd respuesta = _diligenciasRepository.GuardarDiligencias(model);

            if (respuesta.NoError != 0)
            {
                TempData["ErrorDiligencias"] = string.IsNullOrWhiteSpace(respuesta.Mensaje)
                    ? "No fue posible guardar las diligencias."
                    : respuesta.Mensaje;

                return RedirectToAction("AltaDiligencia", new { noExhorto = model.NoExhorto });
            }

            TempData["ExitoDiligencias"] = string.IsNullOrWhiteSpace(respuesta.Mensaje)
                ? "Diligencias guardadas correctamente."
                : respuesta.Mensaje;

            TempData["MostrarConfirmacionFinal"] = true;
            TempData["MostrarPanelEnvio"] = true;

            return RedirectToAction("AltaDiligencia", new { noExhorto = model.NoExhorto });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EnviarExhorto(string noExhorto)
        {
            noExhorto = noExhorto?.Trim();

            if (string.IsNullOrWhiteSpace(noExhorto))
            {
                TempData["ErrorDiligencias"] = "Debe proporcionar el número de exhorto.";
                return RedirectToAction("AltaDiligencia");
            }

            ResponseBd respuesta = _diligenciasRepository.ActualizarEstatusExhorto(noExhorto);

            if (respuesta.NoError != 0)
            {
                TempData["ErrorDiligencias"] = string.IsNullOrWhiteSpace(respuesta.Mensaje)
                    ? "No fue posible enviar el exhorto."
                    : respuesta.Mensaje;

                TempData["MostrarPanelEnvio"] = true;
                TempData["MostrarConfirmacionFinal"] = true;

                return RedirectToAction("AltaDiligencia", new { noExhorto });
            }

            TempData["ExitoEnvio"] = string.IsNullOrWhiteSpace(respuesta.Mensaje)
                ? "El exhorto fue enviado correctamente."
                : respuesta.Mensaje;

            TempData["MostrarPanelEnvio"] = true;
            TempData["MostrarConfirmacionFinal"] = true;

            return RedirectToAction("AltaDiligencia", new { noExhorto });
        }
    }
}