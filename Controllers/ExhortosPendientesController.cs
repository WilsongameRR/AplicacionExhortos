using AplicacionExhortos.Data.Repositories;
using AplicacionExhortos.Models;
using Microsoft.AspNetCore.Mvc;

namespace AplicacionExhortos.Controllers
{
    public class ExhortosPendientesController : SessionControllerBase
    {
        private readonly ExhortosRepository _repository;

        public ExhortosPendientesController(ExhortosRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult ExhortosPendientes()
        {
            if (!TryObtenerUsuarioIdSesion(out string usuarioId))
            {
                return RedirigirALoginPorSesionExpirada();
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
