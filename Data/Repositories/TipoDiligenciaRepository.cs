using AplicacionExhortos.Models;
using MySql.Data.MySqlClient;
using System.Data;

namespace AplicacionExhortos.Data.Repositories
{
    public class TipoDiligenciaRepository
    {
        private readonly BDConnection _db;

        public TipoDiligenciaRepository(BDConnection db)
        {
            _db = db;
        }

        public List<Diligencias> ObtenerTiposDiligencia()
        {
            var lista = new List<Diligencias>();

            using var conn = _db.GetConnection();
            conn.Open();

            using var cmd = new MySqlCommand("sp_obtienetiposdiligencia", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new Diligencias
                {
                    TipoDiligenciaId = Convert.ToInt32(reader["TipoDiligenciaId"]),
                    TipoDiligenciaDesc = reader["TipoDiligenciaDesc"].ToString() ?? ""
                });
            }

            return lista;
        }
    }
}