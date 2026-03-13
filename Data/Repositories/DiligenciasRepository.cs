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
                int exhortoId = ObtenerExhortoIdPorNumero(conn, model.NoExhorto);

                if (exhortoId == 0)
                {
                    respuesta.NoError = 1;
                    return respuesta;
                }

                foreach (var diligencia in model.Diligencias)
                {
                    using var cmd = new MySqlCommand("sp_inserta_diligencia", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("pExhortoId", exhortoId);
                    cmd.Parameters.AddWithValue("pTipoDiligenciaId", diligencia.TipoDiligenciaId);
                    cmd.Parameters.AddWithValue("pOtraEspecificar", diligencia.OtraEspecificar ?? string.Empty);
                    cmd.Parameters.AddWithValue("pDestinatario", diligencia.Destinatario ?? string.Empty);
                    cmd.Parameters.AddWithValue(
                        "pFechaDiligencia",
                        string.IsNullOrEmpty(diligencia.FechaDiligencia) ? DBNull.Value : diligencia.FechaDiligencia
                    );
                    cmd.Parameters.AddWithValue(
                        "pFechaAudiencia",
                        string.IsNullOrEmpty(diligencia.FechaAudiencia) ? DBNull.Value : diligencia.FechaAudiencia
                    );

                    cmd.ExecuteNonQuery();
                }

                respuesta.NoError = 0;
            }
            catch
            {
                respuesta.NoError = 99;
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