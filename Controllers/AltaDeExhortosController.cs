using AplicacionExhortos.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AplicacionExhortos.Controllers
{
    public class AltaDeExhortosController : Controller
    {
        private readonly TuaRepository _tuaRepo;
        private readonly TipoDiligenciaRepository _tipoRepo;

        public AltaDeExhortosController(TuaRepository tuaRepo, TipoDiligenciaRepository tipoRepo)
        {
            _tuaRepo = tuaRepo;
            _tipoRepo = tipoRepo;
        }

        public IActionResult AltaDeExhortos()
        {
            try
            {
                ViewBag.TUAs = _tuaRepo.ObtenerTUAs();
                ViewBag.TiposDiligencia = _tipoRepo.ObtenerTiposDiligencia();

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al cargar los catálogos.";
                Console.WriteLine(ex.Message);
                return View();
            }
        }
    }
}