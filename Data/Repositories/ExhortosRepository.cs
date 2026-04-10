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
                using var cmd = new MySqlCommand("sp_inserta_exhorto", conn);
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
                    model.FechaGeneral.HasValue
                        ? model.FechaGeneral.Value.Date
                        : DBNull.Value
                );

                cmd.Parameters.AddWithValue(
                    "pFechaAudiencia",
                    model.FechaAudiencia.HasValue
                        ? model.FechaAudiencia.Value.Date
                        : DBNull.Value
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

        public List<TipoDocumentoModel> ObtenerTiposDocumento()
        {
            List<TipoDocumentoModel> tiposDocumento = new();

            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                using var cmd = new MySqlCommand("sp_obtienetiposdoctos", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tiposDocumento.Add(new TipoDocumentoModel
                    {
                        TipoDoctoId = reader["TipoDoctoId"] != DBNull.Value
                            ? Convert.ToInt32(reader["TipoDoctoId"])
                            : 0,
                        TipoDoctoDesc = reader["TipoDoctoDesc"]?.ToString() ?? string.Empty
                    });
                }
            }
            catch
            {
                return new List<TipoDocumentoModel>();
            }

            return tiposDocumento;
        }

        public ResponseBd InsertarDocumento(DocumentoModel model)
        {
            ResponseBd respuesta = new();

            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                using var cmd = new MySqlCommand("sp_inserta_documento", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("pExhortoEnviado", model.NoExhorto ?? string.Empty);
                cmd.Parameters.AddWithValue("pTipoDoctoId", model.TipoDocumentoId);
                cmd.Parameters.AddWithValue("pDocumento", model.Documento ?? string.Empty);

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

                respuesta.NoError = cmd.Parameters["p_error_num"].Value != DBNull.Value
                    ? Convert.ToInt32(cmd.Parameters["p_error_num"].Value)
                    : 99;

                respuesta.Mensaje = cmd.Parameters["p_mensaje"].Value?.ToString() ?? string.Empty;
            }
            catch (Exception ex)
            {
                respuesta.NoError = 99;
                respuesta.Mensaje = ex.Message;
            }

            return respuesta;
        }

        public List<DocumentoAdjuntoModel> ObtenerDocumentosAdjuntos(int exhortoId)
        {
            List<DocumentoAdjuntoModel> lista = new();

            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                using var cmd = new MySqlCommand("sp_consulta_documentos", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("pExhortoId", exhortoId);

                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new DocumentoAdjuntoModel
                    {
                        ExhortoId = reader["ExhortoId"] != DBNull.Value
                            ? Convert.ToInt32(reader["ExhortoId"])
                            : 0,

                        DocumentoId = reader["DocumentoId"] != DBNull.Value
                            ? Convert.ToInt32(reader["DocumentoId"])
                            : 0,

                        TipoDocumentoId = reader["TipoDoctoId"] != DBNull.Value
                            ? Convert.ToInt32(reader["TipoDoctoId"])
                            : 0,

                        TipoDocumentoDesc = reader["TipoDoctoDesc"]?.ToString() ?? string.Empty,

                        DocumentoAlfresco = reader["DocumentoAlfresco"]?.ToString() ?? string.Empty
                    });
                }
            }
            catch
            {
                return new List<DocumentoAdjuntoModel>();
            }

            return lista;
        }

        public List<ConsultaExhortos> ObtenerExhortosPendientes(string usuarioId)
        {
            List<ConsultaExhortos> lista = new();

            using var conn = _db.GetConnection();
            conn.Open();

            using var cmd = new MySqlCommand("sp_consulta_exhortos_pendientes", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("pUsuarioid", usuarioId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new ConsultaExhortos
                {
                    ExhortoId = reader["ExhortoId"] != DBNull.Value
                        ? Convert.ToInt32(reader["ExhortoId"])
                        : 0,

                    NoExhortoEnviado = reader["NoExhortoEnviado"]?.ToString() ?? string.Empty,
                    NoExpediente = reader["NoExpediente"]?.ToString() ?? string.Empty,
                    TuaDestino = reader["tuaDestino"]?.ToString() ?? string.Empty,
                    Estatus = reader["Estatus"]?.ToString() ?? string.Empty
                });
            }

            return lista;
        }
    }
}