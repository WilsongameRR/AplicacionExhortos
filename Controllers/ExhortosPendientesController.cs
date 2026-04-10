using AplicacionExhortos.Data.Repositories;
using AplicacionExhortos.Models;
using Microsoft.AspNetCore.Mvc;

namespace AplicacionExhortos.Controllers
{
    public class ExhortosPendientesController : Controller
    {
        private readonly ExhortosRepository _repository;

        public ExhortosPendientesController(ExhortosRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult ExhortosPendientes()
        {
            string usuarioId = HttpContext.Session.GetString("UsuarioId") ?? string.Empty;

            if (string.IsNullOrWhiteSpace(usuarioId))
            {
                TempData["Error"] = "La sesión expiró. Inicie sesión nuevamente.";
                return RedirectToAction("Login", "Login");
            }

            List<ConsultaExhortos> lista = _repository.ObtenerExhortosPendientes(usuarioId);

            ViewBag.UsuarioId = usuarioId;
            ViewBag.Total = lista.Count;

            return View(lista);
        }

        [HttpGet]
        public IActionResult EnviarExhorto(int id)
        {
            return View();
        }
    }
}