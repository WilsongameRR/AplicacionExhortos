using AplicacionExhortos.Data.Repositories;
using AplicacionExhortos.Models;
using Microsoft.AspNetCore.Mvc;

namespace AplicacionExhortos.Controllers
{
    public class ExhortosConsultaController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ConsultaExhortoRepository _consultaExhortoRepository;

        public ExhortosConsultaController(
            IConfiguration configuration,
            ConsultaExhortoRepository consultaExhortoRepository)
        {
            _configuration = configuration;
            _consultaExhortoRepository = consultaExhortoRepository;
        }

        public IActionResult ExhortosConsulta()
        {
            string? usuarioId=null;

            List<consultaExhortos> consultaExhortos=null;
            usuarioId = HttpContext.Session.GetString("UsuarioId");
            consultaExhortos= _consultaExhortoRepository.ConsultaExhorto(usuarioId);
            return View(consultaExhortos);


        }
    }
}