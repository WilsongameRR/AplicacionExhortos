using AplicacionExhortos.Models;
using MySql.Data.MySqlClient;
using System.Data;

namespace AplicacionExhortos.Data.Repositories
{
    public class DiligenciasRepository
    {
        private readonly BDConnection _db;

        public DiligenciasRepository(BDConnection db)
        {
            _db = db;
        }

        public ResponseBd GuardarDiligencias(AltaDiligenciasModel model)
        {
            ResponseBd respuesta = new ResponseBd();

            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                if (string.IsNullOrWhiteSpace(model.NoExhorto))
                {
                    respuesta.NoError = 1;
                    respuesta.Mensaje = "Debe capturar el número de exhorto.";
                    return respuesta;
                }

                if (model.Diligencias == null || !model.Diligencias.Any())
                {
                    respuesta.NoError = 1;
                    respuesta.Mensaje = "Debe agregar al menos una diligencia.";
                    return respuesta;
                }

                foreach (var diligencia in model.Diligencias)
                {
                    using var cmd = new MySqlCommand("sp_inserta_diligencia", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("pExhortoEnviado", model.NoExhorto);
                    cmd.Parameters.AddWithValue("pTipoDiligenciaId", diligencia.TipoDiligenciaId);

                    cmd.Parameters.AddWithValue(
                        "pOtro",
                        string.IsNullOrWhiteSpace(diligencia.OtraEspecificar)
                            ? DBNull.Value
                            : diligencia.OtraEspecificar
                    );

                    cmd.Parameters.AddWithValue(
                        "pDestinatario",
                        string.IsNullOrWhiteSpace(diligencia.Destinatario)
                            ? DBNull.Value
                            : diligencia.Destinatario
                    );

                    cmd.Parameters.AddWithValue(
                        "pFechaAudiencia",
                        string.IsNullOrWhiteSpace(diligencia.FechaAudiencia)
                            ? DBNull.Value
                            : Convert.ToDateTime(diligencia.FechaAudiencia)
                    );

                    var pErrorNum = new MySqlParameter("p_error_num", MySqlDbType.Int32)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(pErrorNum);

                    var pMensaje = new MySqlParameter("p_mensaje", MySqlDbType.VarChar, 100)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(pMensaje);

                    cmd.ExecuteNonQuery();

                    int noError = cmd.Parameters["p_error_num"].Value != DBNull.Value
                        ? Convert.ToInt32(cmd.Parameters["p_error_num"].Value)
                        : 99;

                    string mensaje = cmd.Parameters["p_mensaje"].Value?.ToString() ?? "Error no identificado.";

                    if (noError != 0)
                    {
                        respuesta.NoError = noError;
                        respuesta.Mensaje = mensaje;
                        return respuesta;
                    }
                }

                respuesta.NoError = 0;
                respuesta.Mensaje = "Diligencias guardadas correctamente.";
            }
            catch (Exception ex)
            {
                respuesta.NoError = 99;
                respuesta.Mensaje = ex.Message;
            }

            return respuesta;
        }

        private int ObtenerExhortoIdPorNumero(MySqlConnection conn, string? noExhorto)
        {
            if (string.IsNullOrEmpty(noExhorto))
                return 0;

            using var cmd = new MySqlCommand(
                "SELECT ExhortoId FROM exhortos WHERE NoExhortoEnviado = @NoExhorto LIMIT 1",
                conn);

            cmd.Parameters.AddWithValue("@NoExhorto", noExhorto);

            var resultado = cmd.ExecuteScalar();

            return resultado != null && resultado != DBNull.Value
                ? Convert.ToInt32(resultado)
                : 0;
        }

        public List<DiligenciaModel> ObtenerDiligencias(int exhortoId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            using var cmd = new MySqlCommand("sp_consulta_diligencias", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("pExhortoId", exhortoId);

            List<DiligenciaModel> lista = new();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new DiligenciaModel
                {
                    ExhortoId = reader["ExhortoId"] != DBNull.Value ? Convert.ToInt32(reader["ExhortoId"]) : 0,
                    DiligenciaId = reader["DiligenciaId"] != DBNull.Value ? Convert.ToInt32(reader["DiligenciaId"]) : 0,
                    TipoDiligenciaId = reader["TipoDiligenciaId"] != DBNull.Value ? Convert.ToInt32(reader["TipoDiligenciaId"]) : 0,
                    TipoDiligenciaDesc = reader["TipoDiligenciaDesc"]?.ToString(),
                    OtraEspecificar = reader["OtraEspecificar"]?.ToString(),
                    Destinatario = reader["Destinatario"]?.ToString(),
                    FechaDiligencia = reader["FechaDiligencia"] != DBNull.Value
                        ? Convert.ToDateTime(reader["FechaDiligencia"]).ToString("dd/MM/yyyy")
                        : "",
                    EstatusDiligencia = reader["EstatusDiligencia"]?.ToString(),
                    FechaAudiencia = reader["FechaAudiencia"] != DBNull.Value
                        ? Convert.ToDateTime(reader["FechaAudiencia"]).ToString("dd/MM/yyyy")
                        : ""
                });
            }

            return lista;
        }
    }
}