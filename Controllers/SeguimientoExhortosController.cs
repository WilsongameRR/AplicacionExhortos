using AplicacionExhortos.Data.Repositories;
using AplicacionExhortos.Models;
using Microsoft.AspNetCore.Mvc;

namespace AplicacionExhortos.Controllers
{
    public class SeguimientoExhortosController : SessionControllerBase
    {
        private readonly ConsultaExhortoRepository _consultaExhortoRepository;
        private readonly DiligenciasRepository _diligenciasRepository;
        private readonly DocumentosRepository _documentosRepository;

        public SeguimientoExhortosController(
            ConsultaExhortoRepository consultaExhortoRepository,
            DiligenciasRepository diligenciasRepository,
            DocumentosRepository documentosRepository)
        {
            _consultaExhortoRepository = consultaExhortoRepository;
            _diligenciasRepository = diligenciasRepository;
            _documentosRepository = documentosRepository;
        }

        [HttpGet]
        public IActionResult SeguimientoExhortos()
        {
            if (!TryObtenerTuaIdSesion(out int tuaIdSession))
            {
                return RedirigirALoginPorSesionExpirada("La sesión expiró o no contiene el TUA del usuario.");
            }

            List<ConsultaExhortos> lista =
                _consultaExhortoRepository.ConsultaSeguimientoExhortos(tuaIdSession);

            return View(lista);
        }

        [HttpGet]
        public IActionResult DetalleSeguimiento(int exhortoId)
        {
            if (exhortoId <= 0)
            {
                TempData["Error"] = "El identificador del exhorto no es válido.";
                return RedirectToAction(nameof(SeguimientoExhortos));
            }

            if (!TryObtenerUsuarioIdSesion(out string usuario))
            {
                return RedirigirALoginPorSesionExpirada();
            }

            ConsultaExhortos? exhorto =
                _consultaExhortoRepository.ObtenerDetalleExhortoRecibido(exhortoId);

            if (exhorto == null)
            {
                TempData["Error"] = "No se encontró información del exhorto.";
                return RedirectToAction(nameof(SeguimientoExhortos));
            }

            if (!string.IsNullOrWhiteSpace(exhorto.Estatus) &&
                exhorto.Estatus.Trim().ToUpper() == "PENDIENTE")
            {
                _consultaExhortoRepository.AsignarExhortoRecibido(exhortoId, usuario);

                exhorto = _consultaExhortoRepository.ObtenerDetalleExhortoRecibido(exhortoId);

                if (exhorto == null)
                {
                    TempData["Error"] = "No se encontró información del exhorto después de actualizar.";
                    return RedirectToAction(nameof(SeguimientoExhortos));
                }
            }

            DetalleExhortoModel model = new()
            {
                Exhorto = exhorto,
                Diligencias = _diligenciasRepository.ObtenerDiligencias(exhortoId),
                DocumentosAdjuntos = _documentosRepository.ObtenerDocumentosAdjuntos(exhorto.ExhortoId)
            };

            model.Seguimiento.ExhortoId = exhorto.ExhortoId;
            model.Seguimiento.NoFolio = exhorto.Folio;
            model.Seguimiento.Estatus = exhorto.Estatus;
            model.Seguimiento.Observaciones = exhorto.Observaciones;

            if (DateTime.TryParse(exhorto.FechaRecibido, out DateTime fechaRecepcion))
            {
                model.Seguimiento.FechaRecepcion = fechaRecepcion;
            }

            if (DateTime.TryParse(exhorto.FechaAcuerdoExhortado, out DateTime fechaAcuerdoExhortado))
            {
                model.Seguimiento.FechaAcuerdoTuaExhortado = fechaAcuerdoExhortado;
            }

            if (DateTime.TryParse(exhorto.FechaTurnoActuaria, out DateTime fechaTurnoActuaria))
            {
                model.Seguimiento.FechaTurnoActuaria = fechaTurnoActuaria;
            }

            if (DateTime.TryParse(exhorto.FechaAudiencia, out DateTime fechaAudiencia))
            {
                model.Seguimiento.FechaAudiencia = fechaAudiencia;
            }

            if (DateTime.TryParse(exhorto.FechaVencimiento, out DateTime fechaVencimiento))
            {
                model.Seguimiento.FechaVencimiento = fechaVencimiento;
            }

            if (DateTime.TryParse(exhorto.FechaDevolucion, out DateTime fechaDevolucion))
            {
                model.Seguimiento.FechaDevolucion = fechaDevolucion;
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult GuardarSeguimiento(DetalleExhortoModel model)
        {
            if (model == null || model.Seguimiento == null)
            {
                TempData["Error"] = "No se recibió la información del seguimiento.";
                return RedirectToAction(nameof(SeguimientoExhortos));
            }

            SeguimientoModel seguimiento = model.Seguimiento;

            if (!string.IsNullOrWhiteSpace(seguimiento.Observaciones))
            {
                seguimiento.Observaciones =
                    seguimiento.Observaciones.ToUpper().Trim();
            }

            seguimiento.FechaActualizacion = DateTime.Today;

            TryValidateModel(seguimiento, nameof(model.Seguimiento));

            if (!ModelState.IsValid)
            {
                CargarDatosDetalle(model, seguimiento.ExhortoId);
                return View("DetalleSeguimiento", model);
            }

            if (seguimiento.Estatus == "DILIGENCIADO" || seguimiento.Estatus == "PARCIALMENTE ATENDIDO")
            {
                bool diligenciasValidas = _diligenciasRepository.ValidaDiligencias(seguimiento.ExhortoId);

                if (!diligenciasValidas)
                {
                    ModelState.AddModelError(string.Empty,
                        "No es posible guardar porque existen diligencias pendientes o incompletas.");

                    CargarDatosDetalle(model, seguimiento.ExhortoId);
                    return View("DetalleSeguimiento", model);
                }
            }

            bool actualizado = _consultaExhortoRepository.ActualizarSeguimientoExhorto(
                seguimiento.ExhortoId,
                seguimiento.Estatus,
                seguimiento.FechaRecepcion,
                seguimiento.NoFolio,
                seguimiento.FechaAcuerdoTuaExhortado,
                seguimiento.FechaTurnoActuaria,
                seguimiento.FechaVencimiento,
                seguimiento.FechaDevolucion,
                seguimiento.Observaciones,
                UsuarioIdSesion
            );

            if (!actualizado)
            {
                ModelState.AddModelError(string.Empty, "Ocurrió un error al guardar el seguimiento.");
                CargarDatosDetalle(model, seguimiento.ExhortoId);
                return View("DetalleSeguimiento", model);
            }

            TempData["Exito"] = "Seguimiento guardado correctamente.";
            return RedirectToAction(nameof(SeguimientoExhortos));
        }

        private void CargarDatosDetalle(DetalleExhortoModel model, int exhortoId)
        {
            model.Exhorto = _consultaExhortoRepository.ObtenerDetalleExhortoRecibido(exhortoId)
                ?? new ConsultaExhortos();

            model.Diligencias = _diligenciasRepository.ObtenerDiligencias(exhortoId);
            model.DocumentosAdjuntos =
    _documentosRepository.ObtenerDocumentosAdjuntos(model.Exhorto.ExhortoId);

            if (DateTime.TryParse(model.Exhorto.FechaRecibido, out DateTime fechaRecepcion))
            {
                model.Seguimiento.FechaRecepcion = fechaRecepcion;
            }

            if (DateTime.TryParse(model.Exhorto.FechaAcuerdoExhortado, out DateTime fechaAcuerdoExhortado))
            {
                model.Seguimiento.FechaAcuerdoTuaExhortado = fechaAcuerdoExhortado;
            }

            if (DateTime.TryParse(model.Exhorto.FechaTurnoActuaria, out DateTime fechaTurnoActuaria))
            {
                model.Seguimiento.FechaTurnoActuaria = fechaTurnoActuaria;
            }

            if (DateTime.TryParse(model.Exhorto.FechaAudiencia, out DateTime fechaAudiencia))
            {
                model.Seguimiento.FechaAudiencia = fechaAudiencia;
            }

            if (DateTime.TryParse(model.Exhorto.FechaVencimiento, out DateTime fechaVencimiento))
            {
                model.Seguimiento.FechaVencimiento = fechaVencimiento;
            }

            if (DateTime.TryParse(model.Exhorto.FechaDevolucion, out DateTime fechaDevolucion))
            {
                model.Seguimiento.FechaDevolucion = fechaDevolucion;
            }

            model.Seguimiento.ExhortoId = model.Exhorto.ExhortoId;
            model.Seguimiento.NoFolio = model.Exhorto.Folio;
            model.Seguimiento.Estatus = model.Exhorto.Estatus;
            model.Seguimiento.Observaciones = model.Exhorto.Observaciones;
        }
    }
}
