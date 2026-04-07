using AplicacionExhortos.Data.Repositories;
using AplicacionExhortos.Models.Exhortos;
using Microsoft.AspNetCore.Mvc;

namespace AplicacionExhortos.Controllers
{
    public class ExhortosController : Controller
    {
        private readonly TuaRepository _tuaRepo;
        private readonly TipoDiligenciaRepository _tipoRepo;
        private readonly ExhortosRepository _exhortosRepo;

        public ExhortosController(
            TuaRepository tuaRepo,
            TipoDiligenciaRepository tipoRepo,
            ExhortosRepository exhortosRepo)
        {
            _tuaRepo = tuaRepo;
            _tipoRepo = tipoRepo;
            _exhortosRepo = exhortosRepo;
        }

        [HttpGet]
        public IActionResult AltaDeExhortos()
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("UsuarioId")))
                {
                    TempData["Error"] = "La sesión expiró. Inicie sesión nuevamente.";
                    return RedirectToAction("Login", "Login");
                }

                CargarCatalogos();
                return View("~/Views/AltaDeExhortos/AltaDeExhortos.cshtml");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar los catálogos: " + ex.Message;
                CargarCatalogos();
                return View("~/Views/AltaDeExhortos/AltaDeExhortos.cshtml");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Guardar(AltaExhortoModel model)
        {
            try
            {
                int? tuaOrigen = HttpContext.Session.GetInt32("TuaId");
                string? usuarioOrigen = HttpContext.Session.GetString("UsuarioId");

                if (tuaOrigen == null || string.IsNullOrWhiteSpace(usuarioOrigen))
                {
                    TempData["Error"] = "La sesión expiró. Inicie sesión nuevamente.";
                    return RedirectToAction("Login", "Login");
                }

                ValidarFechaAcuerdo(model.FechaGeneral);
                ValidarFechaAudiencia(model.FechaAudiencia);

                if (!ModelState.IsValid)
                {
                    CargarCatalogos();
                    return View("~/Views/AltaDeExhortos/AltaDeExhortos.cshtml", model);
                }

                var respuesta = _exhortosRepo.GuardarExhorto(model, tuaOrigen.Value, usuarioOrigen);

                if (respuesta.NoError != 0)
                {
                    TempData["Error"] = respuesta.Mensaje ?? "No fue posible guardar el exhorto.";
                    CargarCatalogos();
                    return View("~/Views/AltaDeExhortos/AltaDeExhortos.cshtml", model);
                }

                TempData["MensajeExito"] = "El exhorto se guardó correctamente.";
                TempData["NumeroExhorto"] = respuesta.Valor ?? string.Empty;
                TempData["IdExhorto"] = respuesta.IdGenerado;

                return RedirectToAction("AltaDiligencia", "Diligencias", new
                {
                    noExhorto = respuesta.Valor
                });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al guardar: " + ex.Message;
                CargarCatalogos();
                return View("~/Views/AltaDeExhortos/AltaDeExhortos.cshtml", model);
            }
        }

        [HttpGet]
        public IActionResult AltaDocumentos(string noExhorto)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UsuarioId")))
            {
                TempData["Error"] = "La sesión expiró. Inicie sesión nuevamente.";
                return RedirectToAction("Login", "Login");
            }

            ViewBag.NoExhorto = noExhorto ?? string.Empty;

            return View("~/Views/AltaDeExhortos/AltaDocumentos.cshtml");
        }

        private void ValidarFechaAcuerdo(DateTime? fechaAcuerdo)
        {
            DateTime fechaActual = DateTime.Today;
            DateTime fechaMinima = fechaActual.AddYears(-1);

            if (!fechaAcuerdo.HasValue)
            {
                ModelState.AddModelError("FechaGeneral", "La Fecha de Acuerdo es obligatoria.");
                return;
            }

            DateTime fechaCapturada = fechaAcuerdo.Value.Date;

            if (fechaCapturada < fechaMinima || fechaCapturada > fechaActual)
            {
                ModelState.AddModelError(
                    "FechaGeneral",
                    $"La Fecha de Acuerdo debe estar entre {fechaMinima:dd/MM/yyyy} y {fechaActual:dd/MM/yyyy}."
                );
            }
        }

        private void ValidarFechaAudiencia(DateTime? fechaAudiencia)
        {
            if (!fechaAudiencia.HasValue)
            {
                ModelState.AddModelError("FechaAudiencia", "La Fecha de Audiencia es obligatoria.");
                return;
            }

            DateTime fechaCapturada = fechaAudiencia.Value.Date;
            DateTime fechaMinima = DateTime.Today.AddYears(-1);
            DateTime fechaMaxima = DateTime.Today.AddYears(5);

            if (fechaCapturada < fechaMinima || fechaCapturada > fechaMaxima)
            {
                ModelState.AddModelError(
                    "FechaAudiencia",
                    $"La Fecha de Audiencia debe estar entre {fechaMinima:dd/MM/yyyy} y {fechaMaxima:dd/MM/yyyy}."
                );
            }
        }

        private void CargarCatalogos()
        {
            ViewBag.TUAs = _tuaRepo.ObtenerTUAs();
            ViewBag.TiposDiligencia = _tipoRepo.ObtenerTiposDiligencia();
        }
    }
}