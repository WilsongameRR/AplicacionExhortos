//using AplicacionExhortos.Data.Repositories;
//using AplicacionExhortos.Models;
//using Microsoft.AspNetCore.Mvc;

//namespace AplicacionExhortos.Controllers
//{
//    public class ExhortosRecibidosController : Controller
//    {
//        private readonly ConsultaExhortoRepository _consultaExhortoRepository;

//        public ExhortosRecibidosController(ConsultaExhortoRepository consultaExhortoRepository)
//        {
//            _consultaExhortoRepository = consultaExhortoRepository;
//        }

//        public IActionResult ExhortosRecibidos()
//        {
//            string? usuarioId = HttpContext.Session.GetString("UsuarioId");

//            if (string.IsNullOrEmpty(usuarioId))
//            {
//                TempData["Error"] = "La sesión expiró. Inicie sesión nuevamente.";
//                return RedirectToAction("Login", "Login");
//            }

//            List<consultaExhortos> listaExhortos = _consultaExhortoRepository.ConsultaExhortosRecibidos(usuarioId);

//            return View(listaExhortos);
//        }
//    }
//}

using AplicacionExhortos.Data.Repositories;
using AplicacionExhortos.Models;
using Microsoft.AspNetCore.Mvc;

namespace AplicacionExhortos.Controllers
{
    public class ExhortosRecibidosController : Controller
    {
        private readonly ConsultaExhortoRepository _consultaExhortoRepository;
        private readonly DiligenciasRepository _diligenciasRepository;

        public ExhortosRecibidosController(
            ConsultaExhortoRepository consultaExhortoRepository,
            DiligenciasRepository diligenciasRepository)
        {
            _consultaExhortoRepository = consultaExhortoRepository;
            _diligenciasRepository = diligenciasRepository;
        }

        public IActionResult ExhortosRecibidos()
        {
            string? usuarioId = HttpContext.Session.GetString("UsuarioId");

            if (string.IsNullOrEmpty(usuarioId))
            {
                TempData["Error"] = "La sesión expiró. Inicie sesión nuevamente.";
                return RedirectToAction("Login", "Login");
            }

            List<consultaExhortos> listaExhortos = _consultaExhortoRepository.ConsultaExhortosRecibidos(usuarioId);

            return View(listaExhortos);
        }

        public IActionResult RelacionExhorto(int id)
        {
            var model = new DetalleExhortoModel
            {
                Exhorto = new consultaExhortos(),
                Diligencias = _diligenciasRepository.ObtenerDiligencias(id)
            };

            return View(model);
        }
    }
}