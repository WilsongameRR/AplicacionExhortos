using AplicacionExhortos.Data.Repositories;
using AplicacionExhortos.Models;
using AplicacionExhortos.Models.Exhortos;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;

namespace AplicacionExhortos.Controllers
{
    public class ExhortosController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly TuaRepository _tuaRepo;
        private readonly TipoDiligenciaRepository _tipoRepo;

        public ExhortosController(
            IConfiguration configuration,
            TuaRepository tuaRepo,
            TipoDiligenciaRepository tipoRepo)
        {
            _configuration = configuration;
            _tuaRepo = tuaRepo;
            _tipoRepo = tipoRepo;
        }

        [HttpGet]
        public IActionResult AltaDeExhortos()
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("UsuarioId")))
                {
                    TempData["Error"] = "La sesión expiró. Inicie sesión nuevamente.";
                    return RedirectToAction("Login", "Login");
                }

                CargarCatalogos();

                return View("~/Views/AltaDeExhortos/AltaDeExhortos.cshtml");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar los catálogos: " + ex.Message;
                CargarCatalogos();
                return View("~/Views/AltaDeExhortos/AltaDeExhortos.cshtml");
            }
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Guardar(AltaExhortoModel model)
        {
            try
            {
                var tuaOrigen = HttpContext.Session.GetInt32("TuaId");
                var usuarioOrigen = HttpContext.Session.GetString("UsuarioId");

                if (tuaOrigen == null || string.IsNullOrEmpty(usuarioOrigen))
                {
                    TempData["Error"] = "La sesión expiró. Inicie sesión nuevamente.";
                    return RedirectToAction("Login", "Login");
                }

                ValidarFechaAcuerdo(model.FechaGeneral);
                ValidarFechaAudiencia(model.FechaAudiencia);

                if (!ModelState.IsValid)
                {
                    CargarCatalogos();
                    return View("~/Views/AltaDeExhortos/AltaDeExhortos.cshtml", model);
                }

                string conexion = _configuration.GetConnectionString("MySqlConnection");

                int exhortoIdGenerado = 0;
                string numeroExhortoGenerado = string.Empty;

                using (MySqlConnection conn = new MySqlConnection(conexion))
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand("sp_inserta_exhorto", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("pTUAIdOrigen", tuaOrigen);
                        cmd.Parameters.AddWithValue("pNoExpediente", model.Expediente ?? string.Empty);
                        cmd.Parameters.AddWithValue("pNoOficio", model.NoOficio ?? string.Empty);
                        cmd.Parameters.AddWithValue("pEstado", model.Estado ?? string.Empty);
                        cmd.Parameters.AddWithValue("pMunicipio", model.Municipio ?? string.Empty);
                        cmd.Parameters.AddWithValue("pPoblado", model.Poblado ?? string.Empty);
                        cmd.Parameters.AddWithValue("pTUAIdDestino", model.TuaExhortado);

                        if (model.FechaGeneral.HasValue)
                        {
                            cmd.Parameters.AddWithValue("pFechaAcuerdo", model.FechaGeneral.Value.Date);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("pFechaAcuerdo", DBNull.Value);
                        }

                        if (model.FechaAudiencia.HasValue)
                        {
                            cmd.Parameters.AddWithValue("pFechaAudiencia", model.FechaAudiencia.Value.Date);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("pFechaAudiencia", DBNull.Value);
                        }

                        cmd.Parameters.AddWithValue("pUsuarioIdOrigen", usuarioOrigen);

                        MySqlParameter pExhortoId = new MySqlParameter("pExhortoId", MySqlDbType.Int32)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(pExhortoId);

                        MySqlParameter pExhortoEnviado = new MySqlParameter("pExhortoEnviado", MySqlDbType.VarChar, 40)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(pExhortoEnviado);

                        cmd.ExecuteNonQuery();

                        exhortoIdGenerado = Convert.ToInt32(cmd.Parameters["pExhortoId"].Value);
                        numeroExhortoGenerado = cmd.Parameters["pExhortoEnviado"].Value?.ToString() ?? string.Empty;
                    }
                }

                TempData["MensajeExito"] = "El exhorto se guardó correctamente.";
                TempData["NumeroExhorto"] = numeroExhortoGenerado;
                TempData["IdExhorto"] = exhortoIdGenerado;

                return RedirectToAction("AltaDiligencia", "Diligencias", new { noExhorto = numeroExhortoGenerado });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al guardar: " + ex.Message;
                CargarCatalogos();
                return View("~/Views/AltaDeExhortos/AltaDeExhortos.cshtml", model);
            }
        }

        private void ValidarFechaAcuerdo(DateTime? fechaAcuerdo)
        {
            DateTime fechaActual = DateTime.Today;
            DateTime fechaMinima = fechaActual.AddYears(-1);

            if (!fechaAcuerdo.HasValue)
            {
                ModelState.AddModelError("FechaGeneral", "La Fecha de Acuerdo es obligatoria.");
                return;
            }

            DateTime fechaCapturada = fechaAcuerdo.Value.Date;

            if (fechaCapturada < fechaMinima || fechaCapturada > fechaActual)
            {
                ModelState.AddModelError(
                    "FechaGeneral",
                    $"La Fecha de Acuerdo debe estar entre {fechaMinima:dd/MM/yyyy} y {fechaActual:dd/MM/yyyy}."
                );
            }
        }

        private void ValidarFechaAudiencia(DateTime? fechaAudiencia)
        {
            if (!fechaAudiencia.HasValue)
            {
                ModelState.AddModelError("FechaAudiencia", "La Fecha de Audiencia es obligatoria.");
                return;
            }

            DateTime fechaCapturada = fechaAudiencia.Value.Date;
            DateTime fechaMinima = DateTime.Today.AddYears(-1);
            DateTime fechaMaxima = DateTime.Today.AddYears(5);

            if (fechaCapturada < fechaMinima || fechaCapturada > fechaMaxima)
            {
                ModelState.AddModelError(
                    "FechaAudiencia",
                    $"La Fecha de Audiencia debe estar entre {fechaMinima:dd/MM/yyyy} y {fechaMaxima:dd/MM/yyyy}."
                );
            }
        }

        private void CargarCatalogos()
        {
            ViewBag.TUAs = _tuaRepo.ObtenerTUAs();
            ViewBag.TiposDiligencia = _tipoRepo.ObtenerTiposDiligencia();
        }
    }
}