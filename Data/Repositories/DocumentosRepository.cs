using AplicacionExhortos.Models.Exhortos;
using MySql.Data.MySqlClient;
using System.Data;

namespace AplicacionExhortos.Data.Repositories
{
    public class DocumentosRepository
    {
        private readonly BDConnection _db;

        public DocumentosRepository(BDConnection db)
        {
            _db = db;
        }

        public List<DocumentoAdjuntoModel> ObtenerDocumentosAdjuntosPorNoExhorto(string noExhorto)
        {
            if (string.IsNullOrWhiteSpace(noExhorto))
            {
                return new List<DocumentoAdjuntoModel>();
            }

            using var conexion = _db.GetConnection();
            conexion.Open();

            int exhortoId = ObtenerExhortoIdPorNumero(conexion, noExhorto);

            if (exhortoId <= 0)
            {
                return new List<DocumentoAdjuntoModel>();
            }

            return ObtenerDocumentosAdjuntos(conexion, exhortoId);
        }

        public List<DocumentoAdjuntoModel> ObtenerDocumentosAdjuntos(int exhortoId)
        {
            using var conexion = _db.GetConnection();
            conexion.Open();

            return ObtenerDocumentosAdjuntos(conexion, exhortoId);
        }

        private List<DocumentoAdjuntoModel> ObtenerDocumentosAdjuntos(MySqlConnection conexion, int exhortoId)
        {
            List<DocumentoAdjuntoModel> lista = new();

            using MySqlCommand comando = new("sp_consulta_documentos", conexion);
            comando.CommandType = CommandType.StoredProcedure;
            comando.Parameters.AddWithValue("pExhortoId", exhortoId);

            using MySqlDataReader reader = comando.ExecuteReader();

            while (reader.Read())
            {
                DocumentoAdjuntoModel documento = new()
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

                    TipoDocumentoDesc = reader["TipoDoctoDesc"] != DBNull.Value
                        ? reader["TipoDoctoDesc"].ToString()!
                        : string.Empty,

                    DocumentoAlfresco = reader["DocumentoAlfresco"] != DBNull.Value
                        ? reader["DocumentoAlfresco"].ToString()!
                        : string.Empty,

                    Seccion = reader["Seccion"] != DBNull.Value
                        ? reader["Seccion"].ToString()!
                        : string.Empty
                };

                lista.Add(documento);
            }

            return lista;
        }

        private int ObtenerExhortoIdPorNumero(MySqlConnection conexion, string noExhorto)
        {
            using MySqlCommand comando = new(
                @"SELECT ExhortoId
                  FROM exhortos.exhorto
                  WHERE TRIM(NoExhortoEnviado) = TRIM(@NoExhorto)
                  LIMIT 1",
                conexion);

            comando.Parameters.AddWithValue("@NoExhorto", noExhorto.Trim());

            object? resultado = comando.ExecuteScalar();

            return resultado != null && resultado != DBNull.Value
                ? Convert.ToInt32(resultado)
                : 0;
        }
    }
}
