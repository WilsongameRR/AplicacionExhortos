using AplicacionExhortos.Models;
using MySql.Data.MySqlClient;
using System.Data;

namespace AplicacionExhortos.Data.Repositories
{
    public class ConsultaExhortoRepository
    {
        private readonly BDConnection _db;

        public ConsultaExhortoRepository(BDConnection db)
        {
            _db = db;
        }
        public List<consultaExhortos> ConsultaExhorto (string IdUsuario){

            using var conn = _db.GetConnection();
            conn.Open();

            using var cmd = new MySqlCommand("sp_consulta_exhortos_enviados", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            List<consultaExhortos> listaExhortos = new List<consultaExhortos>();

            cmd.Parameters.AddWithValue("pUsuarioid", IdUsuario);
          
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var exhorto = new consultaExhortos
                    {
                        ExhortoId =Convert.ToInt32(reader["ExhortoId"].ToString()),
                        idOrigen = reader["idOrigen"].ToString(),
                        tuaOrigen = reader["tuaOrigen"].ToString(),
                        NoExhortoEnviado = reader["NoExhortoEnviado"].ToString(),
                        NoExpediente = reader["NoExpediente"].ToString(),
                        NoOficio = reader["NoOficio"].ToString(),
                        Estado = reader["Estado"].ToString(),
                        Poblado = reader["Poblado"].ToString(),
                        idDestino =reader["idDestino"].ToString(),
                        tuaDestino = reader["tuaDestino"].ToString(),
                        FechaAcuerdo = reader["FechaAcuerdo"].ToString(),
                        FechaEnvio = reader["FechaEnvio"].ToString(),
                        Estatus = reader["Estatus"].ToString(),

                    };

                    listaExhortos.Add(exhorto);
                }
            }


            return listaExhortos;

        }


    }
}
