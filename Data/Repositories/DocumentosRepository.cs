using AplicacionExhortos.Models.Exhortos;
using MySql.Data.MySqlClient;
using System.Data;

namespace AplicacionExhortos.Data.Repositories
{
    public class DocumentosRepository
    {
        private readonly string _connectionString;

        public DocumentosRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MySqlConnection")!;
        }

        public List<DocumentoAdjuntoModel> ObtenerDocumentosAdjuntos(int exhortoId)
        {
            List<DocumentoAdjuntoModel> lista = new();

            using MySqlConnection conexion = new(_connectionString);
            using MySqlCommand comando = new("sp_consulta_documentos", conexion);

            comando.CommandType = CommandType.StoredProcedure;
            comando.Parameters.AddWithValue("pExhortoId", exhortoId);

            conexion.Open();

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
    }
}