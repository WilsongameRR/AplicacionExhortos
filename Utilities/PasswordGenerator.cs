using BCrypt.Net;

namespace AplicacionExhortos.Utilities
{
    public class PasswordGenerator
    {
        public static string GenerarHash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}