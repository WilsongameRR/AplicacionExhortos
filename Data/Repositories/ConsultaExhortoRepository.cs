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

            using MySqlCommand cmd = new("exhortos.sp_consulta_exhortos_enviados", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("pUsuarioid", MySqlDbType.VarChar, 40).Value = usuarioId;

            using MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ConsultaExhortos exhorto = new()
                {
                    ExhortoId = ObtenerEnteroSeguro(reader, "ExhortoId"),
                    IdOrigen = ObtenerTextoSeguro(reader, "idOrigen"),
                    TuaOrigen = ObtenerTextoSeguro(reader, "tuaOrigen"),
                    NoExhortoEnviado = ObtenerTextoSeguro(reader, "NoExhortoEnviado"),
                    NoExpediente = ObtenerTextoSeguro(reader, "NoExpediente"),
                    NoOficio = ObtenerTextoSeguro(reader, "NoOficio"),
                    Estado = ObtenerTextoSeguro(reader, "Estado"),
                    Municipio = ObtenerTextoSeguro(reader, "Municipio"),
                    Poblado = ObtenerTextoSeguro(reader, "Poblado"),
                    IdDestino = ObtenerTextoSeguro(reader, "idDestino"),
                    TuaDestino = ObtenerTextoSeguro(reader, "tuaDestino"),
                    FechaAcuerdo = ObtenerFechaSegura(reader, "FechaAcuerdo"),
                    FechaAudiencia = ObtenerFechaSegura(reader, "FechaAudiencia"),
                    FechaEnvio = ObtenerFechaSegura(reader, "FechaEnvio"),
                    Estatus = ObtenerTextoSeguro(reader, "Estatus")
                };

                listaExhortos.Add(exhorto);
            }

            return listaExhortos;
        }

        public List<ConsultaExhortos> ConsultaSeguimientoExhortos(int tuaIdDestino)
        {
            List<ConsultaExhortos> listaExhortos = new();

            using MySqlConnection conn = _db.GetConnection();
            conn.Open();

            using MySqlCommand cmd = new("exhortos.sp_seguimiento_exhortos", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("pDestino", MySqlDbType.Int32).Value = tuaIdDestino;

            using MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ConsultaExhortos exhorto = new()
                {
                    ExhortoId = ObtenerEnteroSeguro(reader, "ExhortoId"),
                    TuaOrigen = ObtenerTextoSeguro(reader, "tuaOrigen"),
                    NoExhortoEnviado = ObtenerTextoSeguro(reader, "NoExhortoEnviado"),
                    NoExpediente = ObtenerTextoSeguro(reader, "NoExpediente"),
                    NoOficio = ObtenerTextoSeguro(reader, "NoOficio"),
                    Estado = ObtenerTextoSeguro(reader, "Estado"),
                    Municipio = ObtenerTextoSeguro(reader, "Municipio"),
                    Poblado = ObtenerTextoSeguro(reader, "Poblado"),
                    TuaDestino = ObtenerTextoSeguro(reader, "tuaDestino"),
                    FechaAcuerdo = ObtenerFechaSegura(reader, "FechaAcuerdo"),
                    FechaAudiencia = ObtenerFechaSegura(reader, "FechaAudiencia"),
                    FechaEnvio = ObtenerFechaSegura(reader, "FechaEnvio"),
                    Estatus = ObtenerTextoSeguro(reader, "Estatus")
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

            using MySqlCommand cmd = new("exhortos.sp_consulta_exhortos_recibidos", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("pDestino", MySqlDbType.Int32).Value = tuaIdDestino;

            using MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ConsultaExhortos exhorto = new()
                {
                    ExhortoId = ObtenerEnteroSeguro(reader, "ExhortoId"),
                    NoExhortoEnviado = ObtenerTextoSeguro(reader, "NoExhortoEnviado"),
                    NoExpediente = ObtenerTextoSeguro(reader, "NoExpediente"),
                    TuaOrigen = ObtenerTextoSeguro(reader, "tuaOrigen"),
                    Estatus = ObtenerTextoSeguro(reader, "Estatus"),
                    NoOficio = ObtenerTextoSeguro(reader, "NoOficio"),
                    Estado = ObtenerTextoSeguro(reader, "Estado"),
                    Municipio = ObtenerTextoSeguro(reader, "Municipio"),
                    Poblado = ObtenerTextoSeguro(reader, "Poblado"),
                    IdDestino = ObtenerTextoSeguro(reader, "idDestino"),
                    TuaDestino = ObtenerTextoSeguro(reader, "tuaDestino"),
                    FechaAcuerdo = ObtenerFechaSegura(reader, "FechaAcuerdo"),
                    FechaAudiencia = ObtenerFechaSegura(reader, "FechaAudiencia"),
                    FechaEnvio = ObtenerFechaSegura(reader, "FechaEnvio"),
                    FechaRecibido = ObtenerFechaSegura(reader, "FechaRecibido"),
                    Folio = ObtenerTextoSeguro(reader, "NoFolio"),
                    NoExhortoRecibido = ObtenerTextoSeguro(reader, "NoExhortoRecibido"),
                    FechaAcuerdoExhortado = ObtenerFechaSegura(reader, "FechaAcuerdoExhortado"),
                    FechaTurnoActuaria = ObtenerFechaSegura(reader, "FechaTurnoActuaria"),
                    FechaDevolucion = ObtenerFechaSegura(reader, "FechaDevolucion"),
                    Observaciones = ObtenerTextoSeguro(reader, "Observaciones")
                };

                listaExhortos.Add(exhorto);
            }

            return listaExhortos;
        }

        public string? AsignarExhortoRecibido(int exhortoId, string usuario)
        {
            using MySqlConnection conn = _db.GetConnection();
            conn.Open();

            using MySqlCommand cmd = new("exhortos.sp_asigna_exhorto_recibido", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("pExhortoId", MySqlDbType.Int32).Value = exhortoId;
            cmd.Parameters.Add("vUsuario", MySqlDbType.VarChar, 40).Value = usuario;

            MySqlParameter pExhortoRecibido = new("pExhortoRecibido", MySqlDbType.VarChar, 40)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(pExhortoRecibido);

            cmd.ExecuteNonQuery();

            return cmd.Parameters["pExhortoRecibido"].Value?.ToString();
        }

        public ConsultaExhortos? ObtenerDetalleExhortoRecibido(int exhortoId)
        {
            using MySqlConnection conn = _db.GetConnection();
            conn.Open();

            using MySqlCommand cmd = new("exhortos.sp_datos_exhorto", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("pExhortoId", MySqlDbType.Int32).Value = exhortoId;

            using MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new ConsultaExhortos
                {
                    ExhortoId = ObtenerEnteroSeguro(reader, "ExhortoId"),
                    NoExhortoEnviado = ObtenerTextoSeguro(reader, "NoExhortoEnviado"),
                    NoExhortoRecibido = ObtenerTextoSeguro(reader, "NoExhortoRecibido"),
                    NoExpediente = ObtenerTextoSeguro(reader, "NoExpediente"),
                    TuaOrigen = ObtenerTextoSeguro(reader, "tuaOrigen"),
                    Estatus = ObtenerTextoSeguro(reader, "Estatus"),
                    NoOficio = ObtenerTextoSeguro(reader, "NoOficio"),
                    Folio = ObtenerTextoSeguro(reader, "NoFolio"),
                    Estado = ObtenerTextoSeguro(reader, "Estado"),
                    Municipio = ObtenerTextoSeguro(reader, "Municipio"),
                    Poblado = ObtenerTextoSeguro(reader, "Poblado"),
                    IdDestino = ObtenerTextoSeguro(reader, "idDestino"),
                    TuaDestino = ObtenerTextoSeguro(reader, "tuaDestino"),
                    FechaAcuerdo = ObtenerFechaSegura(reader, "FechaAcuerdo"),
                    FechaAudiencia = ObtenerFechaSegura(reader, "FechaAudiencia"),
                    FechaEnvio = ObtenerFechaSegura(reader, "FechaEnvio"),
                    FechaRecibido = ObtenerFechaSegura(reader, "FechaRecibido"),
                    FechaAcuerdoExhortado = ObtenerFechaSegura(reader, "FechaAcuerdoExhortado"),
                    FechaTurnoActuaria = ObtenerFechaSegura(reader, "FechaTurnoActuaria"),
                    FechaVencimiento = ObtenerFechaSegura(reader, "FechaVencimiento"),
                    FechaDevolucion = ObtenerFechaSegura(reader, "FechaDevolucion"),
                    FechaNuevoAcuerdo = ObtenerFechaSegura(reader, "FechaNuevoAcuerdo"),
                    FechaNuevaAudiencia = ObtenerFechaSegura(reader, "FechaNuevaAudiencia"),
                    EstatusExhorto = ObtenerTextoSeguro(reader, "EstatusExhorto"),
                    Observaciones = ObtenerTextoSeguro(reader, "Observaciones")
                };
            }

            return null;
        }

        public bool ActualizarSeguimientoExhorto(
     int exhortoId,
     string? estatus,
     DateTime? fechaRecibido,
     string? noFolio,
     DateTime? fechaAcuerdoExhortado,
     DateTime? fechaTurnoActuaria,
     DateTime? fechaVencimiento, // 🔥 NUEVO
     DateTime? fechaDevolucion,
     string? observaciones,
     string? usuarioId)
        {
            using MySqlConnection conn = _db.GetConnection();
            conn.Open();

            using MySqlCommand cmd = new("exhortos.sp_actualiza_exhorto", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("pExhortoId", exhortoId);
            cmd.Parameters.AddWithValue("pEstatus", (object?)estatus ?? DBNull.Value);
            cmd.Parameters.AddWithValue("pFechaRecibido", (object?)fechaRecibido ?? DBNull.Value);
            cmd.Parameters.AddWithValue("pNoFolio", (object?)noFolio ?? DBNull.Value);
            cmd.Parameters.AddWithValue("pFechaAcuerdoExhortado", (object?)fechaAcuerdoExhortado ?? DBNull.Value);
            cmd.Parameters.AddWithValue("pFechaTurnoActuaria", (object?)fechaTurnoActuaria ?? DBNull.Value);

            // 🔥 ESTE FALTABA
            cmd.Parameters.AddWithValue("pFechaVencimiento", (object?)fechaVencimiento ?? DBNull.Value);

            cmd.Parameters.AddWithValue("pFechaDevolucion", (object?)fechaDevolucion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("pObservaciones", (object?)observaciones ?? DBNull.Value);
            cmd.Parameters.AddWithValue("pUsuarioId", (object?)usuarioId ?? DBNull.Value);

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
                throw new Exception(mensaje);
            }

            return true;
        }

        private static string? ObtenerTexto(MySqlDataReader reader, string columna)
        {
            return reader[columna] == DBNull.Value ? null : reader[columna].ToString();
        }

        private static int ObtenerEntero(MySqlDataReader reader, string columna)
        {
            return reader[columna] == DBNull.Value ? 0 : Convert.ToInt32(reader[columna]);
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

        private static bool TieneColumna(MySqlDataReader reader, string nombreColumna)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(nombreColumna, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static string? ObtenerTextoSeguro(MySqlDataReader reader, string columna)
        {
            return TieneColumna(reader, columna) ? ObtenerTexto(reader, columna) : null;
        }

        private static int ObtenerEnteroSeguro(MySqlDataReader reader, string columna)
        {
            return TieneColumna(reader, columna) ? ObtenerEntero(reader, columna) : 0;
        }

        private static string? ObtenerFechaSegura(MySqlDataReader reader, string columna)
        {
            return TieneColumna(reader, columna) ? FormatearFecha(reader[columna]) : null;
        }
    }
}