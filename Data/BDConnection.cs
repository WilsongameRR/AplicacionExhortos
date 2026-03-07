using MySql.Data.MySqlClient;

namespace AplicacionExhortos.Data
{
    public class BDConnection
    {
        private readonly IConfiguration _config;

        public BDConnection(IConfiguration config)
        {
            _config = config;
        }

        public MySqlConnection GetConnection()
        {
            string connectionString = _config.GetConnectionString("MySqlConnection");
            return new MySqlConnection(connectionString);
        }
    }
}