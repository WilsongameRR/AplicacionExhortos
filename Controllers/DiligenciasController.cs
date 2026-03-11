using AplicacionExhortos.Data.Repositories;
using AplicacionExhortos.Models;
using Microsoft.AspNetCore.Mvc;

namespace AplicacionExhortos.Controllers
{
    public class DiligenciasController : Controller
    {
        private readonly TipoDiligenciaRepository _tipoDiligenciaRepository;
        private readonly DiligenciasRepository _diligenciasRepository;

        public DiligenciasController(
            TipoDiligenciaRepository tipoDiligenciaRepository,
            DiligenciasRepository diligenciasRepository)
        {
            _tipoDiligenciaRepository = tipoDiligenciaRepository;
            _diligenciasRepository = diligenciasRepository;
        }

        [HttpGet]
        public IActionResult AltaDiligencia(int idExhorto)
        {
            ViewBag.TiposDiligencia = _tipoDiligenciaRepository.ObtenerTiposDiligencia();
            ViewBag.IdExhorto = idExhorto;
            return View();
        }

        [HttpPost]
        public IActionResult GuardarDiligencias(AltaDiligenciasModel model)
        {
            try
            {
                if (model.Diligencias == null || model.Diligencias.Any())
                {
                    // DiligenciasRepository repository = new DiligenciasRepository();

                    ReponseBd reponseBd = new ReponseBd();
                    TempData["Error"] = "Debe agregar al menos una diligencia.";
                    ViewBag.TiposDiligencia = _tipoDiligenciaRepository.ObtenerTiposDiligencia();
               
                    reponseBd= _diligenciasRepository.GuardarDiligencias(model);
              

                    ViewBag.IdExhorto = model.NoExhorto;
                    return View("AltaDiligencia", model);
                }

                TempData["Error"] = $"Sí llegaron {model.Diligencias.Count} diligencias. IdExhorto: {model.NoExhorto}";
                ViewBag.IdExhorto = model.NoExhorto;
                return View("AltaDiligencia", model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al guardar las diligencias: " + ex.Message;
                ViewBag.TiposDiligencia = _tipoDiligenciaRepository.ObtenerTiposDiligencia();
                ViewBag.IdExhorto = model.NoExhorto;
                return View("AltaDiligencia", model);
            }
        }
    }
}