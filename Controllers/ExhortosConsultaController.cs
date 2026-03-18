using AplicacionExhortos.Data.Repositories;
using AplicacionExhortos.Models;
using Microsoft.AspNetCore.Mvc;

namespace AplicacionExhortos.Controllers
{
    public class ExhortosConsultaController : Controller
    {
        private readonly ConsultaExhortoRepository _consultaExhortoRepository;
        private readonly DiligenciasRepository _diligenciasRepository;

        public ExhortosConsultaController(
            ConsultaExhortoRepository consultaExhortoRepository,
            DiligenciasRepository diligenciasRepository)
        {
            _consultaExhortoRepository = consultaExhortoRepository;
            _diligenciasRepository = diligenciasRepository;
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

            List<consultaExhortos> listaExhortos = _consultaExhortoRepository.ConsultaExhorto(usuarioId);

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

            List<consultaExhortos> listaExhortos = _consultaExhortoRepository.ConsultaExhorto(usuarioId);

            consultaExhortos? exhorto = listaExhortos.FirstOrDefault(x => x.ExhortoId == id);

            if (exhorto == null)
            {
                TempData["Error"] = "No se encontró el exhorto seleccionado.";
                return RedirectToAction(nameof(ExhortosConsulta));
            }

            var diligencias = _diligenciasRepository.ObtenerDiligencias(id);

            var model = new DetalleExhortoModel
            {
                Exhorto = exhorto,
                Diligencias = diligencias
            };

            return View(model);
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