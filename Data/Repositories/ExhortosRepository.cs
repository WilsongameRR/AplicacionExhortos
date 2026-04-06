using AplicacionExhortos.Models;
using AplicacionExhortos.Models.Exhortos;
using MySql.Data.MySqlClient;
using System.Data;

namespace AplicacionExhortos.Data.Repositories
{
    public class ExhortosRepository
    {
        private readonly BDConnection _db;

        public ExhortosRepository(BDConnection db)
        {
            _db = db;
        }

        public ResponseBd GuardarExhorto(AltaExhortoModel model, int tuaOrigen, string usuarioOrigen)
        {
            ResponseBd respuesta = new();

            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                using var cmd = new MySqlCommand("exhortos.sp_inserta_exhorto", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("pOrigen", tuaOrigen);
                cmd.Parameters.AddWithValue("pNoExpediente", model.Expediente ?? string.Empty);
                cmd.Parameters.AddWithValue("pNoOficio", model.NoOficio ?? string.Empty);
                cmd.Parameters.AddWithValue("pEstado", model.Estado ?? string.Empty);
                cmd.Parameters.AddWithValue("pMunicipio", model.Municipio ?? string.Empty);
                cmd.Parameters.AddWithValue("pPoblado", model.Poblado ?? string.Empty);
                cmd.Parameters.AddWithValue("pDestino", model.TuaExhortado);

                cmd.Parameters.AddWithValue(
                    "pFechaAcuerdo",
                    model.FechaGeneral.HasValue ? model.FechaGeneral.Value.Date : DBNull.Value
                );

                cmd.Parameters.AddWithValue(
                    "pFechaAudiencia",
                    model.FechaAudiencia.HasValue ? model.FechaAudiencia.Value.Date : DBNull.Value
                );

                cmd.Parameters.AddWithValue("pUsuarioIdOrigen", usuarioOrigen);

                var pExhortoId = new MySqlParameter("pExhortoId", MySqlDbType.Int32)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(pExhortoId);

                var pExhortoEnviado = new MySqlParameter("pExhortoEnviado", MySqlDbType.VarChar, 40)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(pExhortoEnviado);

                cmd.ExecuteNonQuery();

                respuesta.NoError = 0;
                respuesta.Mensaje = "El exhorto se guardó correctamente.";
                respuesta.IdGenerado = cmd.Parameters["pExhortoId"].Value != DBNull.Value
                    ? Convert.ToInt32(cmd.Parameters["pExhortoId"].Value)
                    : 0;
                respuesta.Valor = cmd.Parameters["pExhortoEnviado"].Value?.ToString();
            }
            catch (Exception ex)
            {
                respuesta.NoError = 99;
                respuesta.Mensaje = ex.Message;
            }

            return respuesta;
        }
    }
}