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
        public IActionResult AltaDiligencia()
        {
            return View(new AltaDiligenciasModel());
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
            return RedirectToAction("AltaDiligencia");
        }
    }
}