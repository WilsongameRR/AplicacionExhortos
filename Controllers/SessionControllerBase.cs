using Microsoft.AspNetCore.Mvc;

namespace AplicacionExhortos.Controllers
{
    public abstract class SessionControllerBase : Controller
    {
        protected const string MensajeSesionExpirada = "La sesión expiró. Inicie sesión nuevamente.";

        protected string? UsuarioIdSesion => HttpContext.Session.GetString("UsuarioId");

        protected int? TuaIdSesion => HttpContext.Session.GetInt32("TuaId");

        protected bool TryObtenerUsuarioIdSesion(out string usuarioId)
        {
            usuarioId = UsuarioIdSesion ?? string.Empty;
            return !string.IsNullOrWhiteSpace(usuarioId);
        }

        protected bool TryObtenerTuaIdSesion(out int tuaId)
        {
            int? tuaIdSesion = TuaIdSesion;

            if (tuaIdSesion.HasValue && tuaIdSesion.Value > 0)
            {
                tuaId = tuaIdSesion.Value;
                return true;
            }

            tuaId = 0;
            return false;
        }

        protected RedirectToActionResult RedirigirALoginPorSesionExpirada(string? mensaje = null)
        {
            TempData["Error"] = mensaje ?? MensajeSesionExpirada;
            return RedirectToAction("Login", "Login");
        }
    }
}
