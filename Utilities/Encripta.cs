using BCrypt.Net;

namespace AplicacionExhortos.Utilities
{
    // En esta clase se usara la herrmineta para encriptar
    public class Encripta
    {
        private const int WorkFactor = 13;

        // Genera el encriptado de la contraseña
        public string EncriptarPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: WorkFactor);
        }

        // Verifica contraseña contra el encriptado guardado
        public bool VerificarPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}