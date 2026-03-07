using AplicacionExhortos.Models;
using MySql.Data.MySqlClient;
using System.Data;

namespace AplicacionExhortos.Data.Repositories
{
    public class TuaRepository
    {
        private readonly BDConnection _db;

        public TuaRepository(BDConnection db)
        {
            _db = db;
        }

        public List<TuaModel> ObtenerTUAs()
        {
            var lista = new List<TuaModel>();

            using var conn = _db.GetConnection();
            conn.Open();

            using var cmd = new MySqlCommand("sp_obtienetua", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new TuaModel
                {
                    TUAId = Convert.ToInt32(reader["TUAId"]),
                    NumTUA = reader["NumTUA"]?.ToString() ?? string.Empty
                });
            }

            return lista;
        }
    }
}