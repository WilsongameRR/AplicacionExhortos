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

            // Parámetro de entrada
            cmd.Parameters.AddWithValue("p_usuarioid", usuario);

            // Parámetros de salida
            cmd.Parameters.Add("p_password", MySqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("p_nombre", MySqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("p_tuaid", MySqlDbType.Int32).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("p_error_num", MySqlDbType.Int32).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("p_mensaje", MySqlDbType.VarChar, 100).Direction = ParameterDirection.Output;

            cmd.ExecuteNonQuery();

            return new ValidaUsuarioResult
            {
                Password = cmd.Parameters["p_password"].Value?.ToString(),
                Nombre = cmd.Parameters["p_nombre"].Value?.ToString(),
                TuaId = cmd.Parameters["p_tuaid"].Value != DBNull.Value
                    ? Convert.ToInt32(cmd.Parameters["p_tuaid"].Value)
                    : 0,
                ErrorNum = cmd.Parameters["p_error_num"].Value != DBNull.Value
                    ? Convert.ToInt32(cmd.Parameters["p_error_num"].Value)
                    : 99,
                Mensaje = cmd.Parameters["p_mensaje"].Value?.ToString()
            };
        }
    }
}