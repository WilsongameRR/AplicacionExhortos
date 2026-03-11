using AplicacionExhortos.Data.Repositories;
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

                TempData.Remove("Error");

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

                if (!ModelState.IsValid)
                {
                    CargarCatalogos();
                    return View("~/Views/AltaDeExhortos/AltaDeExhortos.cshtml", model);
                }

                string conexion = _configuration.GetConnectionString("MySqlConnection");

                using (MySqlConnection conn = new MySqlConnection(conexion))
                {
                    conn.Open();

                    int exhortoIdGenerado = 0;
                    string numeroExhortoGenerado = "";

                    using (MySqlCommand cmd = new MySqlCommand("sp_inserta_exhorto", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("pTUAIdOrigen", tuaOrigen);
                        cmd.Parameters.AddWithValue("pNoExpediente", model.Expediente ?? "");
                        cmd.Parameters.AddWithValue("pNoOficio", model.NoOficio ?? "");
                        cmd.Parameters.AddWithValue("pEstado", model.Estado ?? "");
                        cmd.Parameters.AddWithValue("pMunicipio", model.Municipio ?? "");
                        cmd.Parameters.AddWithValue("pPoblado", model.Poblado ?? "");
                        cmd.Parameters.AddWithValue("pTUAIdDestino", model.TuaExhortado);

                        if (model.FechaGeneral.HasValue)
                            cmd.Parameters.AddWithValue("pFechaAcuerdo", model.FechaGeneral.Value);
                        else
                            cmd.Parameters.AddWithValue("pFechaAcuerdo", DBNull.Value);

                        cmd.Parameters.AddWithValue("pUsuarioIdOrigen", usuarioOrigen);

                        MySqlParameter pExhortoId = new MySqlParameter("pExhortoId", MySqlDbType.Int32);
                        pExhortoId.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(pExhortoId);

                        MySqlParameter pExhortoEnviado = new MySqlParameter("pExhortoEnviado", MySqlDbType.VarChar, 40);
                        pExhortoEnviado.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(pExhortoEnviado);

                        cmd.ExecuteNonQuery();

                        exhortoIdGenerado = Convert.ToInt32(cmd.Parameters["pExhortoId"].Value);
                        numeroExhortoGenerado = cmd.Parameters["pExhortoEnviado"].Value?.ToString() ?? "";

                        TempData["NumeroExhorto"] = numeroExhortoGenerado;
                        TempData["IdExhorto"] = exhortoIdGenerado;
                    }
                }

                return RedirectToAction("AltaDeExhortos", "Exhortos");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al guardar: " + ex.Message;
                return RedirectToAction("AltaDeExhortos", "Exhortos");
            }
        }

        private void CargarCatalogos()
        {
            ViewBag.TUAs = _tuaRepo.ObtenerTUAs();
            ViewBag.TiposDiligencia = _tipoRepo.ObtenerTiposDiligencia();
        }
    }
}