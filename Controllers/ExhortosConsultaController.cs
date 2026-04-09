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

            var model = new DetalleExhortoModel
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

            TempData["Mensaje"] = $"Se seleccionó la opción Reiterar Exhorto para el exhorto con ID {id}.";
            return RedirectToAction(nameof(DetalleExhorto), new { id });
        }

        [HttpGet]
        public IActionResult MarcarAtendido(int id)
        {
            string? usuarioId = ObtenerUsuarioIdSesion();

            if (string.IsNullOrWhiteSpace(usuarioId))
            {
                TempData["Error"] = "La sesión expiró. Inicie sesión nuevamente.";
                return RedirectToAction("Login", "Login");
            }

            TempData["Mensaje"] = $"Se seleccionó la opción Atendido para el exhorto con ID {id}.";
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