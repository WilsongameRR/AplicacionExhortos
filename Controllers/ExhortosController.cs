using AplicacionExhortos.Models.Exhortos;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;

namespace AplicacionExhortos.Controllers
{
    public class ExhortosController : Controller
    {
        private readonly IConfiguration _configuration;

        public ExhortosController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult Guardar(AltaExhortoModel model)
        {
            try
            {
                var tuaOrigen = HttpContext.Session.GetInt32("TUAId");
                var usuarioOrigen = HttpContext.Session.GetString("UsuarioId");

                if (tuaOrigen == null || string.IsNullOrEmpty(usuarioOrigen))
                {
                    TempData["Error"] = "La sesión expiró. Inicie sesión nuevamente.";
                    return RedirectToAction("Login", "Login");
                }

                string conexion = _configuration.GetConnectionString("MySqlConnection");

                using (MySqlConnection conn = new MySqlConnection(conexion))
                {
                    conn.Open();

                    int exhortoIdGenerado = 0;
                    string numeroExhortoGenerado = "";

                    
                    // GUARDAR EXHORTO
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
                    }

                    // GUARDAR DILIGENCIA
                    if (model.TipoDiligenciaId.HasValue)
                    {
                        using (MySqlCommand cmdDiligencia = new MySqlCommand("sp_inserta_diligencia", conn))
                        {
                            cmdDiligencia.CommandType = CommandType.StoredProcedure;

                            cmdDiligencia.Parameters.AddWithValue("pExhortoId", exhortoIdGenerado);
                            cmdDiligencia.Parameters.AddWithValue("pTipoDiligenciaId", model.TipoDiligenciaId.Value);

                            if (string.IsNullOrWhiteSpace(model.Otro))
                                cmdDiligencia.Parameters.AddWithValue("pOtro", DBNull.Value);
                            else
                                cmdDiligencia.Parameters.AddWithValue("pOtro", model.Otro);

                            if (string.IsNullOrWhiteSpace(model.Destinatario))
                                cmdDiligencia.Parameters.AddWithValue("pDestinatario", DBNull.Value);
                            else
                                cmdDiligencia.Parameters.AddWithValue("pDestinatario", model.Destinatario);

                            if (model.FechaAudiencia.HasValue)
                                cmdDiligencia.Parameters.AddWithValue("pFechaAudiencia", model.FechaAudiencia.Value);
                            else
                                cmdDiligencia.Parameters.AddWithValue("pFechaAudiencia", DBNull.Value);

                            cmdDiligencia.ExecuteNonQuery();
                        }
                    }

                    TempData["NumeroExhorto"] = numeroExhortoGenerado;
                }

                return RedirectToAction("AltaExhortos");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al guardar: " + ex.Message;
                return RedirectToAction("AltaExhortos");
            }
        }
    }
}