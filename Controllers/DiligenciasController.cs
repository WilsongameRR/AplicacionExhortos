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
            ViewBag.ExitoEnvio = TempData["ExitoEnvio"];
            ViewBag.ErrorDiligencias = TempData["ErrorDiligencias"];
            ViewBag.ExitoDiligencias = TempData["ExitoDiligencias"];

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GuardarDiligencias(AltaDiligenciasModel model)
        {
            if (string.IsNullOrEmpty(model.NoExhorto))
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

            if (respuesta.NoError == 1)
            {
                TempData["ErrorDiligencias"] = "El número de exhorto no existe.";
                return View("AltaDiligencia", model);
            }

            if (respuesta.NoError == 99)
            {
                TempData["ErrorDiligencias"] = "Error inesperado en la base de datos.";
                return View("AltaDiligencia", model);
            }

            TempData["ExitoDiligencias"] = "Diligencias guardadas correctamente.";
            TempData["MostrarConfirmacionFinal"] = true;

            return RedirectToAction("AltaDiligencia", new { noExhorto = model.NoExhorto });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EnviarExhorto(string noExhorto)
        {
            if (string.IsNullOrWhiteSpace(noExhorto))
            {
                TempData["ErrorDiligencias"] = "Debe proporcionar el número de exhorto.";
                return RedirectToAction("AltaDiligencia");
            }

            ResponseBd respuesta = _diligenciasRepository.ActualizarEstatusExhorto(noExhorto);

            if (respuesta.NoError != 0)
            {
                TempData["ErrorDiligencias"] = "No fue posible enviar el exhorto.";
                TempData["MostrarPanelEnvio"] = true;
                return RedirectToAction("AltaDiligencia", new { noExhorto });
            }

            TempData["ExitoEnvio"] = "El exhorto fue enviado correctamente.";
            return RedirectToAction("AltaDiligencia");
        }
    }
}