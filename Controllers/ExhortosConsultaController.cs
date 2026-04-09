using AplicacionExhortos.Data.Repositories;
using AplicacionExhortos.Models;
using AplicacionExhortos.Models.Exhortos;
using Microsoft.AspNetCore.Mvc;

namespace AplicacionExhortos.Controllers
{
    public class ExhortosConsultaController : Controller
    {
        private readonly ConsultaExhortoRepository _consultaExhortoRepository;
        private readonly DiligenciasRepository _diligenciasRepository;
        private readonly DocumentosRepository _documentosRepository;

        public ExhortosConsultaController(
            ConsultaExhortoRepository consultaExhortoRepository,
            DiligenciasRepository diligenciasRepository,
            DocumentosRepository documentosRepository)
        {
            _consultaExhortoRepository = consultaExhortoRepository;
            _diligenciasRepository = diligenciasRepository;
            _documentosRepository = documentosRepository;
        }

        [HttpGet]
        public IActionResult ExhortosConsulta()
        {
            string? usuarioId = ObtenerUsuarioIdSesion();

            if (string.IsNullOrWhiteSpace(usuarioId))
            {
                TempData["Error"] = "La sesión expiró. Inicie sesión nuevamente.";
                return RedirectToAction("Login", "Login");
            }

            List<ConsultaExhortos> listaExhortos = _consultaExhortoRepository.ConsultaExhorto(usuarioId);

            ViewBag.UsuarioIdSesion = usuarioId;
            ViewBag.TotalRegistros = listaExhortos.Count;

            return View(listaExhortos);
        }

        [HttpGet]
        public IActionResult DetalleExhorto(int id)
        {
            string? usuarioId = ObtenerUsuarioIdSesion();

            if (string.IsNullOrWhiteSpace(usuarioId))
            {
                TempData["Error"] = "La sesión expiró. Inicie sesión nuevamente.";
                return RedirectToAction("Login", "Login");
            }

            ConsultaExhortos? exhorto = _consultaExhortoRepository.ObtenerDetalleExhortoRecibido(id);

            if (exhorto == null)
            {
                TempData["Error"] = "No se encontró el exhorto seleccionado.";
                return RedirectToAction(nameof(ExhortosConsulta));
            }

            List<DiligenciaModel> diligencias = _diligenciasRepository.ObtenerDiligencias(id);
            List<DocumentoAdjuntoModel> documentosAdjuntos = _documentosRepository.ObtenerDocumentosAdjuntos(id);

            DetalleExhortoModel model = new()
            {
                Exhorto = exhorto,
                Diligencias = diligencias,
                DocumentosAdjuntos = documentosAdjuntos
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult ReiterarExhorto(int id)
        {
            string? usuarioId = ObtenerUsuarioIdSesion();

            if (string.IsNullOrWhiteSpace(usuarioId))
            {
                TempData["Error"] = "La sesión expiró. Inicie sesión nuevamente.";
                return RedirectToAction("Login", "Login");
            }

            ConsultaExhortos? exhorto = _consultaExhortoRepository.ObtenerDetalleExhortoRecibido(id);

            if (exhorto == null)
            {
                TempData["Error"] = "No se encontró el exhorto seleccionado.";
                return RedirectToAction(nameof(ExhortosConsulta));
            }

            List<DiligenciaModel> diligencias = _diligenciasRepository.ObtenerDiligencias(id);
            List<DocumentoAdjuntoModel> documentosAdjuntos = _documentosRepository.ObtenerDocumentosAdjuntos(id);

            ReiterarExhortoModel model = new()
            {
                ExhortoId = exhorto.ExhortoId,
                NoExhortoEnviado = exhorto.NoExhortoEnviado,
                TuaOrigen = exhorto.TuaOrigen,
                NoExpediente = exhorto.NoExpediente,
                NoOficio = exhorto.NoOficio,
                Estado = exhorto.Estado,
                Municipio = exhorto.Municipio,
                Poblado = exhorto.Poblado,
                Estatus = exhorto.Estatus,
                NumeroEnvios = 0,
                Diligencias = diligencias,
                DocumentosAdjuntos = documentosAdjuntos
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReiterarExhorto(ReiterarExhortoModel model)
        {
            string? usuarioId = ObtenerUsuarioIdSesion();

            if (string.IsNullOrWhiteSpace(usuarioId))
            {
                TempData["Error"] = "La sesión expiró. Inicie sesión nuevamente.";
                return RedirectToAction("Login", "Login");
            }

            if (!ModelState.IsValid)
            {
                ConsultaExhortos? exhorto = _consultaExhortoRepository.ObtenerDetalleExhortoRecibido(model.ExhortoId);

                if (exhorto != null)
                {
                    model.NoExhortoEnviado = exhorto.NoExhortoEnviado;
                    model.TuaOrigen = exhorto.TuaOrigen;
                    model.NoExpediente = exhorto.NoExpediente;
                    model.NoOficio = exhorto.NoOficio;
                    model.Estado = exhorto.Estado;
                    model.Municipio = exhorto.Municipio;
                    model.Poblado = exhorto.Poblado;
                    model.Estatus = exhorto.Estatus;
                }

                model.NumeroEnvios = 0;
                model.Diligencias = _diligenciasRepository.ObtenerDiligencias(model.ExhortoId);
                model.DocumentosAdjuntos = _documentosRepository.ObtenerDocumentosAdjuntos(model.ExhortoId);

                return View(model);
            }

            bool reiterado = _consultaExhortoRepository.ReiterarExhorto(model, usuarioId);

            if (!reiterado)
            {
                ModelState.AddModelError(string.Empty, "No fue posible reiterar el exhorto.");

                ConsultaExhortos? exhorto = _consultaExhortoRepository.ObtenerDetalleExhortoRecibido(model.ExhortoId);

                if (exhorto != null)
                {
                    model.NoExhortoEnviado = exhorto.NoExhortoEnviado;
                    model.TuaOrigen = exhorto.TuaOrigen;
                    model.NoExpediente = exhorto.NoExpediente;
                    model.NoOficio = exhorto.NoOficio;
                    model.Estado = exhorto.Estado;
                    model.Municipio = exhorto.Municipio;
                    model.Poblado = exhorto.Poblado;
                    model.Estatus = exhorto.Estatus;
                }

                model.NumeroEnvios = 0;
                model.Diligencias = _diligenciasRepository.ObtenerDiligencias(model.ExhortoId);
                model.DocumentosAdjuntos = _documentosRepository.ObtenerDocumentosAdjuntos(model.ExhortoId);

                return View(model);
            }

            TempData["Success"] = "Exhorto reiterado correctamente.";
            return RedirectToAction(nameof(DetalleExhorto), new { id = model.ExhortoId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarcarAtendido(int id)
        {
            string? usuarioId = ObtenerUsuarioIdSesion();

            if (string.IsNullOrWhiteSpace(usuarioId))
            {
                TempData["Error"] = "La sesión expiró. Inicie sesión nuevamente.";
                return RedirectToAction("Login", "Login");
            }

            bool actualizado = _consultaExhortoRepository.MarcarExhortoAtendido(id, usuarioId);

            if (!actualizado)
            {
                TempData["Error"] = "No fue posible actualizar el exhorto.";
                return RedirectToAction(nameof(DetalleExhorto), new { id });
            }

            TempData["Success"] = "Exhorto actualizado correctamente.";
            return RedirectToAction(nameof(DetalleExhorto), new { id });
        }

        [HttpGet]
        public IActionResult ExhortoSeguimiento()
        {
            string? usuarioId = ObtenerUsuarioIdSesion();

            if (string.IsNullOrWhiteSpace(usuarioId))
            {
                TempData["Error"] = "La sesión expiró. Inicie sesión nuevamente.";
                return RedirectToAction("Login", "Login");
            }

            ViewBag.UsuarioIdSesion = usuarioId;
            return View();
        }

        private string? ObtenerUsuarioIdSesion()
        {
            return HttpContext.Session.GetString("UsuarioId");
        }
    }
}