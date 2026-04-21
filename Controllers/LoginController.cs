using AplicacionExhortos.Data.Repositories;
using AplicacionExhortos.Models.Login;
using Microsoft.AspNetCore.Mvc;

namespace AplicacionExhortos.Controllers
{
    public class LoginController : Controller
    {
        private readonly LoginRepository _loginRepository;

        public LoginController(LoginRepository loginRepository)
        {
            _loginRepository = loginRepository;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/Login/Login.cshtml");
        }

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            const string mensajeError = "Usuario o contraseña incorrectos.";

            if (!ModelState.IsValid)
            {
                return View("~/Views/Login/Login.cshtml", model);
            }

            try
            {
                if (string.IsNullOrWhiteSpace(model.Usuario) ||
                    !model.Usuario.EndsWith("@tribunalesagrarios.gob.mx", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("Usuario", "Debe ingresar su correo institucional.");
                    return View("~/Views/Login/Login.cshtml", model);
                }

                string usuarioId = model.Usuario.Split('@')[0];

                var resultado = _loginRepository.ValidaUsuario(usuarioId);

                if (resultado == null || resultado.ErrorNum != 0)
                {
                    ModelState.AddModelError(string.Empty, mensajeError);
                    return View("~/Views/Login/Login.cshtml", model);
                }

                if (string.IsNullOrEmpty(resultado.Password) ||
                    !BCrypt.Net.BCrypt.Verify(model.Password, resultado.Password))
                {
                    ModelState.AddModelError(string.Empty, mensajeError);
                    return View("~/Views/Login/Login.cshtml", model);
                }

                HttpContext.Session.SetString("Usuario", resultado.Nombre ?? usuarioId);
                HttpContext.Session.SetString("UsuarioId", usuarioId);
                HttpContext.Session.SetString("Correo", model.Usuario);
                HttpContext.Session.SetInt32("TuaId", resultado.TuaId);
                HttpContext.Session.SetString("NumTua", resultado.NumTua ?? "");


                TempData.Remove("Error");

                return RedirectToAction("AltaDeExhortos", "Exhortos");
            }
            catch
            {
                ModelState.AddModelError(string.Empty, mensajeError);
                return View("~/Views/Login/Login.cshtml", model);
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Login");
        }
    }
}
