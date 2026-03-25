using AplicacionExhortos.Data.Repositories;
using AplicacionExhortos.Models;
using Microsoft.AspNetCore.Mvc;

namespace AplicacionExhortos.Controllers
{
    public class SeguimientoExhortosController : Controller
    {
        private readonly ConsultaExhortoRepository _consultaExhortoRepository;
        private readonly DiligenciasRepository _diligenciasRepository;

        public SeguimientoExhortosController(
            ConsultaExhortoRepository consultaExhortoRepository,
            DiligenciasRepository diligenciasRepository)
        {
            _consultaExhortoRepository = consultaExhortoRepository;
            _diligenciasRepository = diligenciasRepository;
        }

        [HttpGet]
        public IActionResult SeguimientoExhortos()
        {
            int? tuaIdSession = HttpContext.Session.GetInt32("TuaId");

            if (!tuaIdSession.HasValue || tuaIdSession.Value <= 0)
            {
                TempData["Error"] = "La sesión expiró o no contiene el TUA del usuario.";
                return RedirectToAction("Login", "Login");
            }

            List<ConsultaExhortos> lista =
                _consultaExhortoRepository.ConsultaSeguimientoExhortos(tuaIdSession.Value);

            return View(lista);
        }

        [HttpGet]
        public IActionResult DetalleSeguimiento(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "El identificador del exhorto no es válido.";
                return RedirectToAction(nameof(SeguimientoExhortos));
            }

            string? usuario = HttpContext.Session.GetString("UsuarioId");

            if (string.IsNullOrWhiteSpace(usuario))
            {
                TempData["Error"] = "La sesión expiró. Inicie sesión nuevamente.";
                return RedirectToAction("Login", "Login");
            }

            ConsultaExhortos? exhorto =
                _consultaExhortoRepository.ObtenerDetalleExhortoRecibido(id);

            if (exhorto == null)
            {
                TempData["Error"] = "No se encontró información del exhorto.";
                return RedirectToAction(nameof(SeguimientoExhortos));
            }

            if (!string.IsNullOrWhiteSpace(exhorto.Estatus) &&
                exhorto.Estatus.Trim().ToUpper() == "PENDIENTE")
            {
                _consultaExhortoRepository.AsignarExhortoRecibido(id, usuario);

                exhorto = _consultaExhortoRepository.ObtenerDetalleExhortoRecibido(id);

                if (exhorto == null)
                {
                    TempData["Error"] = "No se encontró información del exhorto después de actualizar.";
                    return RedirectToAction(nameof(SeguimientoExhortos));
                }
            }

            DetalleExhortoModel model = new()
            {
                Exhorto = exhorto,
                Diligencias = _diligenciasRepository.ObtenerDiligencias(id)
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

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GuardarSeguimiento(DetalleExhortoModel model)
        {
            if (model == null || model.Seguimiento == null)
            {
                TempData["Error"] = "No se recibió la información del seguimiento.";
                return RedirectToAction(nameof(SeguimientoExhortos));
            }

            var s = model.Seguimiento;

            if (!ModelState.IsValid)
            {
                model.Exhorto = _consultaExhortoRepository.ObtenerDetalleExhortoRecibido(s.ExhortoId) ?? new ConsultaExhortos();
                model.Diligencias = _diligenciasRepository.ObtenerDiligencias(s.ExhortoId);
                return View("DetalleSeguimiento", model);
            }

            // Validaciones de fechas
            if (s.FechaAcuerdoTuaExhortado < s.FechaRecepcion)
            {
                ModelState.AddModelError("", "La fecha de acuerdo TUA exhortado debe ser mayor o igual a la fecha de recepción.");
                model.Exhorto = _consultaExhortoRepository.ObtenerDetalleExhortoRecibido(s.ExhortoId) ?? new ConsultaExhortos();
                model.Diligencias = _diligenciasRepository.ObtenerDiligencias(s.ExhortoId);
                return View("DetalleSeguimiento", model);
            }

            if (s.FechaTurnoActuaria < s.FechaAcuerdoTuaExhortado)
            {
                ModelState.AddModelError("", "La fecha turno actuaría debe ser mayor o igual a la fecha de acuerdo TUA exhortado.");
                model.Exhorto = _consultaExhortoRepository.ObtenerDetalleExhortoRecibido(s.ExhortoId) ?? new ConsultaExhortos();
                model.Diligencias = _diligenciasRepository.ObtenerDiligencias(s.ExhortoId);
                return View("DetalleSeguimiento", model);
            }

            if (s.FechaAudiencia.HasValue)
            {
                if (s.FechaAcuerdoTuaExhortado >= s.FechaAudiencia)
                {
                    ModelState.AddModelError("", "La fecha de acuerdo TUA exhortado debe ser menor a la fecha de audiencia.");
                    model.Exhorto = _consultaExhortoRepository.ObtenerDetalleExhortoRecibido(s.ExhortoId) ?? new ConsultaExhortos();
                    model.Diligencias = _diligenciasRepository.ObtenerDiligencias(s.ExhortoId);
                    return View("DetalleSeguimiento", model);
                }

                if (s.FechaTurnoActuaria >= s.FechaAudiencia)
                {
                    ModelState.AddModelError("", "La fecha turno actuaría debe ser menor a la fecha de audiencia.");
                    model.Exhorto = _consultaExhortoRepository.ObtenerDetalleExhortoRecibido(s.ExhortoId) ?? new ConsultaExhortos();
                    model.Diligencias = _diligenciasRepository.ObtenerDiligencias(s.ExhortoId);
                    return View("DetalleSeguimiento", model);
                }
            }

            // Si quieres validar diligencias al cambiar a DILIGENCIADO o PARCIALMENTE ATENDIDO
            if (s.Estatus == "DILIGENCIADO" || s.Estatus == "PARCIALMENTE ATENDIDO")
            {
                bool diligenciasValidas = _diligenciasRepository.ValidaDiligencias(s.ExhortoId);

                if (!diligenciasValidas)
                {
                    ModelState.AddModelError("", "No es posible guardar porque existen diligencias pendientes o incompletas.");
                    model.Exhorto = _consultaExhortoRepository.ObtenerDetalleExhortoRecibido(s.ExhortoId) ?? new ConsultaExhortos();
                    model.Diligencias = _diligenciasRepository.ObtenerDiligencias(s.ExhortoId);
                    return View("DetalleSeguimiento", model);
                }
            }

            string? usuarioId = HttpContext.Session.GetString("UsuarioId");

            bool actualizado = _consultaExhortoRepository.ActualizarSeguimientoExhorto(
                s.ExhortoId,
                s.Estatus,
                s.FechaRecepcion,
                s.NoFolio,
                s.FechaAcuerdoTuaExhortado,
                s.FechaTurnoActuaria,
                null,
                s.Observaciones,
                usuarioId
            );

            if (!actualizado)
            {
                ModelState.AddModelError("", "Ocurrió un error al guardar el seguimiento.");
                model.Exhorto = _consultaExhortoRepository.ObtenerDetalleExhortoRecibido(s.ExhortoId) ?? new ConsultaExhortos();
                model.Diligencias = _diligenciasRepository.ObtenerDiligencias(s.ExhortoId);
                return View("DetalleSeguimiento", model);
            }

            TempData["Exito"] = "Seguimiento guardado correctamente.";
            return RedirectToAction(nameof(SeguimientoExhortos));
        }
    }
}