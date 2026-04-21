using AplicacionExhortos.Data.Repositories;
using AplicacionExhortos.Models;
using AplicacionExhortos.Models.Exhortos;
using Microsoft.AspNetCore.Mvc;

namespace AplicacionExhortos.Controllers
{
    public class ExhortosController : SessionControllerBase
    {
        private const string VistaAltaExhortos = "~/Views/AltaDeExhortos/AltaDeExhortos.cshtml";
        private const string VistaAltaDocumentos = "~/Views/AltaDeExhortos/AltaDocumentos.cshtml";
        private const string VistaAltaDocumentosModal = "~/Views/Exhortos/_AltaDocumentosModal.cshtml";

        private readonly TuaRepository _tuaRepo;
        private readonly TipoDiligenciaRepository _tipoRepo;
        private readonly ExhortosRepository _exhortosRepo;
        private readonly DocumentosRepository _documentosRepo;

        public ExhortosController(
            TuaRepository tuaRepo,
            TipoDiligenciaRepository tipoRepo,
            ExhortosRepository exhortosRepo,
            DocumentosRepository documentosRepo)
        {
            _tuaRepo = tuaRepo;
            _tipoRepo = tipoRepo;
            _exhortosRepo = exhortosRepo;
            _documentosRepo = documentosRepo;
        }

        [HttpGet]
        public IActionResult AltaDeExhortos()
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("UsuarioId")))
                {
                    return RedirigirALoginPorSesionExpirada();
                }

                CargarCatalogos();
                return View(VistaAltaExhortos);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar los catálogos: " + ex.Message;
                CargarCatalogos();
                return View(VistaAltaExhortos);
            }
        }

        [HttpPost]
        public IActionResult Guardar(AltaExhortoModel model)
        {
            try
            {
                if (!TryObtenerTuaIdSesion(out int tuaOrigen) ||
                    !TryObtenerUsuarioIdSesion(out string usuarioOrigen))
                {
                    return RedirigirALoginPorSesionExpirada();
                }

                ValidarFechaAcuerdo(model.FechaGeneral);
                ValidarFechaAudiencia(model.FechaAudiencia);

                if (!ModelState.IsValid)
                {
                    CargarCatalogos();
                    return View(VistaAltaExhortos, model);
                }

                ResponseBd respuesta = _exhortosRepo.GuardarExhorto(model, tuaOrigen, usuarioOrigen);

                if (respuesta.NoError != 0)
                {
                    TempData["Error"] = respuesta.Mensaje ?? "No fue posible guardar el exhorto.";
                    CargarCatalogos();
                    return View(VistaAltaExhortos, model);
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
                return View(VistaAltaExhortos, model);
            }
        }

        [HttpGet]
        public IActionResult AltaDocumentos(string noExhorto)
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("UsuarioId")))
                {
                    return RedirigirALoginPorSesionExpirada();
                }

                if (string.IsNullOrWhiteSpace(noExhorto))
                {
                    TempData["Error"] = "No se recibió el número de exhorto.";
                    return RedirectToAction("AltaDeExhortos");
                }

                CargarTiposDocumento();
                return View(VistaAltaDocumentos, CrearModeloAltaDocumentos(noExhorto));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar la pantalla de documentos: " + ex.Message;
                CargarTiposDocumento();
                return View(VistaAltaDocumentos, CrearModeloAltaDocumentos(noExhorto));
            }
        }

        [HttpPost]
        public IActionResult GuardarDocumentosModal(AltaDocumentosViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("UsuarioId")))
                {
                    return Json(new
                    {
                        ok = false,
                        mensaje = MensajeSesionExpirada
                    });
                }

                if (model == null || string.IsNullOrWhiteSpace(model.NoExhorto))
                {
                    return Json(new
                    {
                        ok = false,
                        mensaje = "No se identificó el número de exhorto."
                    });
                }

                if (model.Documentos == null || !model.Documentos.Any())
                {
                    return Json(new
                    {
                        ok = false,
                        mensaje = "Debe agregar al menos un documento."
                    });
                }

                foreach (DocumentoModel documento in model.Documentos)
                {
                    documento.NoExhorto = model.NoExhorto;
                    documento.Seccion = string.IsNullOrWhiteSpace(documento.Seccion)
                        ? "E"
                        : documento.Seccion.Trim().ToUpper();

                    if (documento.TipoDocumentoId <= 0)
                    {
                        return Json(new
                        {
                            ok = false,
                            mensaje = "Debe seleccionar el tipo de documento."
                        });
                    }

                    if (string.IsNullOrWhiteSpace(documento.Documento))
                    {
                        return Json(new
                        {
                            ok = false,
                            mensaje = "Debe capturar el documento Alfresco."
                        });
                    }

                    ResponseBd respuesta = _exhortosRepo.InsertarDocumento(documento);

                    if (respuesta.NoError != 0)
                    {
                        return Json(new
                        {
                            ok = false,
                            mensaje = string.IsNullOrWhiteSpace(respuesta.Mensaje)
                                ? "No fue posible guardar el documento."
                                : respuesta.Mensaje
                        });
                    }
                }

                return Json(new
                {
                    ok = true,
                    mensaje = "Los documentos se guardaron correctamente."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    ok = false,
                    mensaje = "Error al guardar documentos: " + ex.Message
                });
            }
        }

        [HttpGet]
        public IActionResult CargarModalDocumentos(string noExhorto)
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("UsuarioId")))
                {
                    return BadRequest(MensajeSesionExpirada);
                }

                if (string.IsNullOrWhiteSpace(noExhorto))
                {
                    return BadRequest("No se recibió el número de exhorto.");
                }

                CargarTiposDocumento();
                return PartialView(VistaAltaDocumentosModal, CrearModeloAltaDocumentos(noExhorto));
            }
            catch (Exception ex)
            {
                return BadRequest("Error al cargar documentos: " + ex.Message);
            }
        }

        [HttpPost]
        public IActionResult GuardarDocumentos(AltaDocumentosViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("UsuarioId")))
                {
                    return RedirigirALoginPorSesionExpirada();
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
                    documento.Seccion = string.IsNullOrWhiteSpace(documento.Seccion)
                        ? "E"
                        : documento.Seccion.Trim().ToUpper();

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

        private void CargarTiposDocumento()
        {
            ViewBag.TiposDocumento = _exhortosRepo.ObtenerTiposDocumento();
        }

        private AltaDocumentosViewModel CrearModeloAltaDocumentos(string? noExhorto)
        {
            string numeroExhorto = noExhorto ?? string.Empty;

            return new AltaDocumentosViewModel
            {
                NoExhorto = numeroExhorto,
                DocumentosGuardados = _documentosRepo.ObtenerDocumentosAdjuntosPorNoExhorto(numeroExhorto)
            };
        }
    }
}
