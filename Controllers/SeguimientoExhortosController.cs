using AplicacionExhortos.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AplicacionExhortos.Controllers
{
    public class SeguimientoExhortosController : Controller
    {
        [HttpGet]
        public IActionResult SeguimientoExhortos()
        {
            var lista = new List<ConsultaExhortos>();
            return View(lista);
        }
    }
}