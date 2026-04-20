using MySql.Data.MySqlClient;

namespace AplicacionExhortos.Data
{
    public class BDConnection
    {
        private readonly IConfiguration _configuration;

        public BDConnection(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public MySqlConnection GetConnection()
        {
            string connectionString = _configuration.GetConnectionString("MySqlConnection")
                ?? throw new InvalidOperationException(
                    "No se encontró la cadena de conexión 'MySqlConnection'.");

            return new MySqlConnection(connectionString);
        }
    }
}
