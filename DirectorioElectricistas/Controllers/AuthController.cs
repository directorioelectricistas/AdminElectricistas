using Microsoft.AspNetCore.Mvc;
using DirectorioElectricistas.Models; 
using MongoDB.Driver;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using DirectorioElectricistas.Services;

namespace DirectorioElectricistas.Controllers
{
    public class AuthController : Controller
    {
        private readonly IMongoCollection<Users> _usersCollection;
        private readonly PasswordService _passwordService;

        public AuthController(PasswordService passwordService)
        {
            var client = new MongoClient("mongodb+srv://directorioelectricistas:ugXgrCQgxBFXJU17@electricistas.yagbp.mongodb.net/?retryWrites=true&w=majority&appName=Electricistas");
            var database = client.GetDatabase("Directory");
            _usersCollection = database.GetCollection<Users>("Users");
            _passwordService = passwordService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = _usersCollection.Find(u => u.Name == username).FirstOrDefault();

            if (user != null && _passwordService.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name)
            };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                // Redirigir a la página principal
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrorMessage = "Nombre de usuario o contraseña incorrectos.";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Cerrar sesión
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // New User
        [HttpPost]
        public IActionResult Register(string username, string password)
        {
            _passwordService.CreatePasswordHash(password, out string passwordHash, out string passwordSalt);

            var user = new Users
            {
                Name = username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            _usersCollection.InsertOne(user);

            return RedirectToAction("Login");
        }
    }

}
