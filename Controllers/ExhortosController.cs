using AplicacionExhortos.Data.Repositories;
using AplicacionExhortos.Models;
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

                ResponseBd respuesta = _exhortosRepo.GuardarExhorto(model, tuaOrigen.Value, usuarioOrigen);

                if (respuesta.NoError != 0)
                {
                    TempData["Error"] = respuesta.Mensaje ?? "No fue posible guardar el exhorto.";
                    CargarCatalogos();
                    return View("~/Views/AltaDeExhortos/AltaDeExhortos.cshtml", model);
                }

                TempData["MensajeExito"] = "El exhorto se guardó correctamente.";
                TempData["NumeroExhorto"] = respuesta.Valor ?? string.Empty;
                TempData["IdExhorto"] = respuesta.IdGenerado;
                TempData["MostrarModalDocumentos"] = true;

                return RedirectToAction("AltaDeExhortos");
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
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("UsuarioId")))
                {
                    TempData["Error"] = "La sesión expiró. Inicie sesión nuevamente.";
                    return RedirectToAction("Login", "Login");
                }

                if (string.IsNullOrWhiteSpace(noExhorto))
                {
                    TempData["Error"] = "No se recibió el número de exhorto.";
                    return RedirectToAction("AltaDeExhortos");
                }

                AltaDocumentosViewModel model = new AltaDocumentosViewModel
                {
                    NoExhorto = noExhorto
                };

                ViewBag.TiposDocumento = _exhortosRepo.ObtenerTiposDocumento();

                return View("~/Views/AltaDeExhortos/AltaDocumentos.cshtml", model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar la pantalla de documentos: " + ex.Message;

                AltaDocumentosViewModel model = new AltaDocumentosViewModel
                {
                    NoExhorto = noExhorto ?? string.Empty
                };

                ViewBag.TiposDocumento = _exhortosRepo.ObtenerTiposDocumento();

                return View("~/Views/AltaDeExhortos/AltaDocumentos.cshtml", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GuardarDocumentos(AltaDocumentosViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("UsuarioId")))
                {
                    TempData["Error"] = "La sesión expiró. Inicie sesión nuevamente.";
                    return RedirectToAction("Login", "Login");
                }

                if (model == null || string.IsNullOrWhiteSpace(model.NoExhorto))
                {
                    TempData["Error"] = "No se identificó el número de exhorto.";
                    return RedirectToAction("AltaDeExhortos");
                }

                if (model.Documentos == null || !model.Documentos.Any())
                {
                    TempData["Error"] = "Debe agregar al menos un documento.";
                    return RedirectToAction("AltaDocumentos", new
                    {
                        noExhorto = model.NoExhorto
                    });
                }

                foreach (DocumentoModel documento in model.Documentos)
                {
                    documento.NoExhorto = model.NoExhorto;

                    if (documento.TipoDocumentoId <= 0)
                    {
                        TempData["Error"] = "Debe seleccionar el tipo de documento.";
                        return RedirectToAction("AltaDocumentos", new
                        {
                            noExhorto = model.NoExhorto
                        });
                    }

                    if (string.IsNullOrWhiteSpace(documento.Documento))
                    {
                        TempData["Error"] = "Debe capturar el documento Alfresco.";
                        return RedirectToAction("AltaDocumentos", new
                        {
                            noExhorto = model.NoExhorto
                        });
                    }

                    ResponseBd respuesta = _exhortosRepo.InsertarDocumento(documento);

                    if (respuesta.NoError != 0)
                    {
                        TempData["Error"] = string.IsNullOrWhiteSpace(respuesta.Mensaje)
                            ? "No fue posible guardar el documento."
                            : respuesta.Mensaje;

                        return RedirectToAction("AltaDocumentos", new
                        {
                            noExhorto = model.NoExhorto
                        });
                    }
                }

                TempData["Exito"] = "Los documentos se guardaron correctamente.";

                return RedirectToAction("AltaDiligencia", "Diligencias", new
                {
                    noExhorto = model.NoExhorto
                });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al guardar documentos: " + ex.Message;

                return RedirectToAction("AltaDocumentos", new
                {
                    noExhorto = model?.NoExhorto ?? string.Empty
                });
            }
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