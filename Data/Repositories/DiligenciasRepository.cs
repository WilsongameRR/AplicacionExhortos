using AplicacionExhortos.Data.Results;
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

        public ReponseBd GuardarDiligencias(AltaDiligenciasModel altaDiligencias)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            ReponseBd reponseBd = null;

            foreach (var diligencia in altaDiligencias.Diligencias)
            {
                using var cmd = new MySqlCommand("sp_inserta_diligencia", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                

                //cmd.Parameters.AddWithValue("pExhortoEnviado", No);
                cmd.Parameters.AddWithValue("pExhortoEnviado", diligencia.NoExhorto);
                cmd.Parameters.AddWithValue("pTipoDiligenciaId", diligencia.TipoDiligenciaId);

                if (string.IsNullOrWhiteSpace(diligencia.Otro))
                    cmd.Parameters.AddWithValue("pOtro", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("pOtro", diligencia.Otro);

                if (string.IsNullOrWhiteSpace(diligencia.Destinatario))
                    cmd.Parameters.AddWithValue("pDestinatario", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("pDestinatario", diligencia.Destinatario);

                if (diligencia.FechaAudiencia.HasValue)
                    cmd.Parameters.AddWithValue("pFechaAudiencia", diligencia.FechaAudiencia.Value.ToString("yyyy-MM-dd"));
                else
                    cmd.Parameters.AddWithValue("pFechaAudiencia", DBNull.Value);
                cmd.Parameters.Add("p_error_num", MySqlDbType.Int32).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("p_mensaje", MySqlDbType.VarChar, 100).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                reponseBd = new ReponseBd
                {
                    NoError = Convert.ToInt32(cmd.Parameters["p_error_num"].Value),

                    Mensaje = cmd.Parameters["p_mensaje"].Value?.ToString()
                };

            }


            return reponseBd;
        }
    }
}