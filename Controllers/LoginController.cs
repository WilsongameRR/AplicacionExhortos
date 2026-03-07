using AplicacionExhortos.Data.Repositories;
using AplicacionExhortos.Models.Login;
using AplicacionExhortos.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace AplicacionExhortos.Controllers
{
    public class LoginController : Controller
    {
        private readonly LoginRepository _repo;

        public LoginController(LoginRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                // Quitar dominio si el usuario escribe correo
                if (!string.IsNullOrEmpty(model.Usuario) && model.Usuario.Contains("@"))
                {
                    string[] aux = model.Usuario.Split('@');
                    model.Usuario = aux[0];
                }

                // Validar usuario con stored procedure
                var result = _repo.ValidaUsuario(model.Usuario);

                // Códigos de resultado del SP
                if (result.ErrorNum == 1)
                {
                    ModelState.AddModelError("", "El usuario ingresado no existe.");
                    return View(model);
                }

                if (result.ErrorNum == 2)
                {
                    ModelState.AddModelError("", "El usuario ingresado está inactivo.");
                    return View(model);
                }

                if (result.ErrorNum == 99)
                {
                    ModelState.AddModelError("", "Error inesperado en la base de datos.");
                    return View(model);
                }

                // Si no regresó password hash
                if (string.IsNullOrEmpty(result.PasswordHash))
                {
                    ModelState.AddModelError("", "No se pudo validar el usuario.");
                    return View(model);
                }

                // Validar contraseña con BCrypt
                var encripta = new Encripta();
                bool ok = encripta.VerificarPassword(model.Password, result.PasswordHash);

                if (!ok)
                {
                    ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
                    return View(model);
                }

                // Guardar datos en sesión
                HttpContext.Session.SetString("UsuarioId", model.Usuario);
                HttpContext.Session.SetString("UsuarioNombre", result.Nombre ?? "");
                HttpContext.Session.SetInt32("TUAId", result.TuaId);

                // Login correcto
                return RedirectToAction("ExhortosEnviados", "ExhortosEnviados");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Ocurrió un error inesperado al iniciar sesión.");
                return View(model);
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