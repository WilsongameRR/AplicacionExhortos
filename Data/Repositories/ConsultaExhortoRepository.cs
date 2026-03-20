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
            List<ConsultaExhortos> listaExhortos = new();

            using MySqlConnection conn = _db.GetConnection();
            conn.Open();

            using MySqlCommand cmd = new("exhortos_db.sp_consulta_exhortos_enviados", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("pUsuarioid", MySqlDbType.VarChar, 40).Value = usuarioId;

            using MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ConsultaExhortos exhorto = new()
                {
                    ExhortoId = ObtenerEntero(reader, "ExhortoId"),
                    IdOrigen = ObtenerTexto(reader, "idOrigen"),
                    TuaOrigen = ObtenerTexto(reader, "tuaOrigen"),
                    NoExhortoEnviado = ObtenerTexto(reader, "NoExhortoEnviado"),
                    NoExpediente = ObtenerTexto(reader, "NoExpediente"),
                    NoOficio = ObtenerTexto(reader, "NoOficio"),
                    Estado = ObtenerTexto(reader, "Estado"),
                    Municipio = ObtenerTexto(reader, "Municipio"),
                    Poblado = ObtenerTexto(reader, "Poblado"),
                    IdDestino = ObtenerTexto(reader, "idDestino"),
                    TuaDestino = ObtenerTexto(reader, "tuaDestino"),
                    FechaAcuerdo = FormatearFecha(reader["FechaAcuerdo"]),
                    FechaAudiencia = FormatearFecha(reader["FechaAudiencia"]),
                    FechaEnvio = FormatearFecha(reader["FechaEnvio"]),
                    Estatus = ObtenerTexto(reader, "Estatus")
                };

                listaExhortos.Add(exhorto);
            }

            return listaExhortos;
        }

        public List<ConsultaExhortos> ConsultaExhortosRecibidos(int tuaIdDestino)
        {
            List<ConsultaExhortos> listaExhortos = new();

            using MySqlConnection conn = _db.GetConnection();
            conn.Open();

            using MySqlCommand cmd = new("exhortos_db.sp_consulta_exhortos_recibidos", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("pTUAIdDestino", MySqlDbType.Int32).Value = tuaIdDestino;

            using MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ConsultaExhortos exhorto = new()
                {
                    ExhortoId = ObtenerEntero(reader, "ExhortoId"),
                    NoExhortoEnviado = ObtenerTexto(reader, "NoExhortoEnviado"),
                    NoExpediente = ObtenerTexto(reader, "NoExpediente"),
                    TuaOrigen = ObtenerTexto(reader, "tuaOrigen"),
                    Estatus = ObtenerTexto(reader, "Estatus"),
                    NoOficio = ObtenerTexto(reader, "NoOficio"),
                    Estado = ObtenerTexto(reader, "Estado"),
                    Municipio = ObtenerTexto(reader, "Municipio"),
                    Poblado = ObtenerTexto(reader, "Poblado"),
                    IdDestino = ObtenerTexto(reader, "idDestino"),
                    TuaDestino = ObtenerTexto(reader, "tuaDestino"),
                    FechaAcuerdo = FormatearFecha(reader["FechaAcuerdo"]),
                    FechaAudiencia = FormatearFecha(reader["FechaAudiencia"]),
                    FechaEnvio = FormatearFecha(reader["FechaEnvio"]),
                    FechaRecibido = FormatearFecha(reader["FechaRecibido"]),
                    Folio = ObtenerTexto(reader, "NoFolio"),
                    NoExhortoRecibido = ObtenerTexto(reader, "NoExhortoRecibido"),
                    FechaAcuerdoExhortado = FormatearFecha(reader["FechaAcuerdoExhortado"]),
                    FechaTurnoActuaria = FormatearFecha(reader["FechaTurnoActuaria"]),
                    FechaDevolucion = FormatearFecha(reader["FechaDevolucion"]),
                    Observaciones = ObtenerTexto(reader, "Observaciones")
                };

                listaExhortos.Add(exhorto);
            }

            return listaExhortos;
        }

        public ConsultaExhortos? ObtenerDetalleExhortoRecibido(int exhortoId)
        {
            using MySqlConnection conn = _db.GetConnection();
            conn.Open();

            using MySqlCommand cmd = new("exhortos_db.sp_datos_exhorto", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("pExhortoId", MySqlDbType.Int32).Value = exhortoId;

            using MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new ConsultaExhortos
                {
                    ExhortoId = ObtenerEntero(reader, "ExhortoId"),
                    NoExhortoEnviado = ObtenerTexto(reader, "NoExhortoEnviado"),
                    NoExhortoRecibido = ObtenerTexto(reader, "NoExhortoRecibido"),
                    NoExpediente = ObtenerTexto(reader, "NoExpediente"),
                    TuaOrigen = ObtenerTexto(reader, "tuaOrigen"),
                    Estatus = ObtenerTexto(reader, "Estatus"),
                    NoOficio = ObtenerTexto(reader, "NoOficio"),
                    Folio = ObtenerTexto(reader, "NoFolio"),
                    Estado = ObtenerTexto(reader, "Estado"),
                    Municipio = ObtenerTexto(reader, "Municipio"),
                    Poblado = ObtenerTexto(reader, "Poblado"),
                    IdDestino = ObtenerTexto(reader, "idDestino"),
                    TuaDestino = ObtenerTexto(reader, "tuaDestino"),
                    FechaAcuerdo = FormatearFecha(reader["FechaAcuerdo"]),
                    FechaAudiencia = FormatearFecha(reader["FechaAudiencia"]),
                    FechaEnvio = FormatearFecha(reader["FechaEnvio"]),
                    FechaRecibido = FormatearFecha(reader["FechaRecibido"]),
                    FechaAcuerdoExhortado = FormatearFecha(reader["FechaAcuerdoExhortado"]),
                    FechaTurnoActuaria = FormatearFecha(reader["FechaTurnoActuaria"]),
                    FechaDevolucion = FormatearFecha(reader["FechaDevolucion"]),
                    Observaciones = ObtenerTexto(reader, "Observaciones")
                };
            }

            return null;
        }

        private static string? ObtenerTexto(MySqlDataReader reader, string columna)
        {
            return reader[columna] == DBNull.Value
                ? null
                : reader[columna].ToString();
        }

        private static int ObtenerEntero(MySqlDataReader reader, string columna)
        {
            return reader[columna] == DBNull.Value
                ? 0
                : Convert.ToInt32(reader[columna]);
        }

        private static string? FormatearFecha(object valor)
        {
            if (valor == null || valor == DBNull.Value)
            {
                return null;
            }

            if (DateTime.TryParse(valor.ToString(), out DateTime fecha))
            {
                return fecha.ToString("dd/MM/yyyy");
            }

            return valor.ToString();
        }
    }
}