using AplicacionExhortos.Data.Results;
using MySql.Data.MySqlClient;
using System.Data;

namespace AplicacionExhortos.Data.Repositories
{
    public class LoginRepository
    {
        private readonly BDConnection _db;

        public LoginRepository(BDConnection db)
        {
            _db = db;
        }

        public ValidaUsuarioResult ValidaUsuario(string usuario)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            using var cmd = new MySqlCommand("sp_valida_usuario", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            // Entrada
            cmd.Parameters.AddWithValue("p_usuarioid", usuario);

            // Salidas
            cmd.Parameters.Add("p_password", MySqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("p_nombre", MySqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("p_tuaid", MySqlDbType.Int32).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("p_error_num", MySqlDbType.Int32).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("p_mensaje", MySqlDbType.VarChar, 100).Direction = ParameterDirection.Output;

            cmd.ExecuteNonQuery();

            string? passwordHash = cmd.Parameters["p_password"].Value?.ToString();
            string? nombre = cmd.Parameters["p_nombre"].Value?.ToString();
            string? mensaje = cmd.Parameters["p_mensaje"].Value?.ToString();

            int tuaId = 0;
            if (cmd.Parameters["p_tuaid"].Value != DBNull.Value)
                tuaId = Convert.ToInt32(cmd.Parameters["p_tuaid"].Value);

            int errorNum = 0;
            if (cmd.Parameters["p_error_num"].Value != DBNull.Value)
                errorNum = Convert.ToInt32(cmd.Parameters["p_error_num"].Value);

            return new ValidaUsuarioResult
            {
                PasswordHash = passwordHash,
                Nombre = nombre,
                TuaId = tuaId,
                ErrorNum = errorNum,
                Mensaje = mensaje
            };
        }
    }
}