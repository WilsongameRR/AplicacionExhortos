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

        public List<ConsultaExhortos> ConsultaExhorto(string usuarioId)
        {
            var listaExhortos = new List<ConsultaExhortos>();

            using var conn = _db.GetConnection();
            conn.Open();

            using var cmd = new MySqlCommand("exhortos_db.sp_consulta_exhortos_enviados", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("pUsuarioid", MySqlDbType.VarChar, 40).Value = usuarioId;

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var exhorto = new ConsultaExhortos
                {
                    ExhortoId = reader["ExhortoId"] != DBNull.Value ? Convert.ToInt32(reader["ExhortoId"]) : 0,
                    IdOrigen = reader["idOrigen"]?.ToString(),
                    TuaOrigen = reader["tuaOrigen"]?.ToString(),
                    NoExhortoEnviado = reader["NoExhortoEnviado"]?.ToString(),
                    NoExpediente = reader["NoExpediente"]?.ToString(),
                    NoOficio = reader["NoOficio"]?.ToString(),
                    Estado = reader["Estado"]?.ToString(),
                    Municipio = reader["Municipio"]?.ToString(),
                    Poblado = reader["Poblado"]?.ToString(),
                    IdDestino = reader["idDestino"]?.ToString(),
                    TuaDestino = reader["tuaDestino"]?.ToString(),
                    FechaAcuerdo = reader["FechaAcuerdo"]?.ToString(),
                    FechaAudiencia = reader["FechaAudiencia"]?.ToString(),
                    FechaEnvio = reader["FechaEnvio"]?.ToString(),
                    Estatus = reader["Estatus"]?.ToString()
                };

                listaExhortos.Add(exhorto);
            }

            return listaExhortos;
        }

        public List<ConsultaExhortos> ConsultaExhortosRecibidos(int tuaIdDestino)
        {
            var listaExhortos = new List<ConsultaExhortos>();

            try
            {
                using var conn = _db.GetConnection();
                conn.Open();

                using var cmd = new MySqlCommand("exhortos_db.sp_consulta_exhortos_recibidos", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("pTUAIdDestino", MySqlDbType.Int32).Value = tuaIdDestino;

                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var exhorto = new ConsultaExhortos
                    {
                        ExhortoId = reader["ExhortoId"] != DBNull.Value ? Convert.ToInt32(reader["ExhortoId"]) : 0,
                        NoExhortoEnviado = reader["NoExhortoEnviado"]?.ToString(),
                        NoExpediente = reader["NoExpediente"]?.ToString(),
                        TuaOrigen = reader["tuaOrigen"]?.ToString(),
                        Estatus = reader["Estatus"]?.ToString(),
                        NoOficio = reader["NoOficio"]?.ToString(),
                        Estado = reader["Estado"]?.ToString(),
                        Municipio = reader["Municipio"]?.ToString(),
                        Poblado = reader["Poblado"]?.ToString(),
                        IdDestino = reader["idDestino"]?.ToString(),
                        TuaDestino = reader["tuaDestino"]?.ToString(),
                        FechaAcuerdo = reader["FechaAcuerdo"]?.ToString(),
                        FechaAudiencia = reader["FechaAudiencia"]?.ToString(),
                        FechaEnvio = reader["FechaEnvio"]?.ToString(),
                        FechaRecibido = reader["FechaRecibido"]?.ToString(),
                        Folio = reader["NoFolio"]?.ToString(),
                        NoExhortoRecibido = reader["NoExhortoRecibido"]?.ToString(),
                        FechaAcuerdoExhortado = reader["FechaAcuerdoExhortado"]?.ToString(),
                        FechaTurnoActuaria = reader["FechaTurnoActuaria"]?.ToString(),
                        FechaDevolucion = reader["FechaDevolucion"]?.ToString(),
                        Observaciones = reader["Observaciones"]?.ToString()
                    };

                    listaExhortos.Add(exhorto);
                }
            }
            catch (MySqlException)
            {
                return new List<ConsultaExhortos>();
            }
            catch (Exception)
            {
                return new List<ConsultaExhortos>();
            }

            return listaExhortos;
        }
    }
}