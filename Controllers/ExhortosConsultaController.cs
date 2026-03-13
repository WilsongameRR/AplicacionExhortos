using AplicacionExhortos.Data.Repositories;
using AplicacionExhortos.Models;
using Microsoft.AspNetCore.Mvc;

namespace AplicacionExhortos.Controllers
{
    public class ExhortosConsultaController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ConsultaExhortoRepository _consultaExhortoRepository;
        private readonly DiligenciasRepository _diligenciasRepository;

        public ExhortosConsultaController(
            IConfiguration configuration,
            ConsultaExhortoRepository consultaExhortoRepository,
            DiligenciasRepository diligenciasRepository)
        {
            _configuration = configuration;
            _consultaExhortoRepository = consultaExhortoRepository;
            _diligenciasRepository = diligenciasRepository;
        }

        public IActionResult ExhortosConsulta()
        {
            string? usuarioId = HttpContext.Session.GetString("UsuarioId");

            if (string.IsNullOrEmpty(usuarioId))
            {
                TempData["Error"] = "La sesión expiró. Inicie sesión nuevamente.";
                return RedirectToAction("Login", "Login");
            }

            List<consultaExhortos> listaExhortos = _consultaExhortoRepository.ConsultaExhorto(usuarioId);

            return View(listaExhortos);
        }

        public IActionResult DetalleExhorto(int id)
        {
            string? usuarioId = HttpContext.Session.GetString("UsuarioId");

            if (string.IsNullOrEmpty(usuarioId))
            {
                TempData["Error"] = "La sesión expiró. Inicie sesión nuevamente.";
                return RedirectToAction("Login", "Login");
            }

            List<consultaExhortos> listaExhortos = _consultaExhortoRepository.ConsultaExhorto(usuarioId);

            var exhorto = listaExhortos.FirstOrDefault(x => x.ExhortoId == id);

            if (exhorto == null)
            {
                TempData["Error"] = "No se encontró el exhorto seleccionado.";
                return RedirectToAction("ExhortosConsulta");
            }

            var diligencias = _diligenciasRepository.ObtenerDiligencias(id);

            var model = new DetalleExhortoModel
            {
                Exhorto = exhorto,
                Diligencias = diligencias
            };

            return View(model);
        }

        public IActionResult ExhortoSeguimiento()
        {
            string? usuarioId = HttpContext.Session.GetString("UsuarioId");

            if (string.IsNullOrEmpty(usuarioId))
            {
                TempData["Error"] = "La sesión expiró. Inicie sesión nuevamente.";
                return RedirectToAction("Login", "Login");
            }

            return View();
        }
    }
}
