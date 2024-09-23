using DirectorioElectricistas.Models;
using DirectorioElectricistas.Repositories;
using DirectorioElectricistas.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DirectorioElectricistas.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {

       private IUsersCollection db = new UsersCollection();

        // GET: UsersController
        public ActionResult Index()
        {
            var users = db.GetAllUsers();
            return View(users);
        }

        // GET: UsersController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UsersController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UsersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // Instancia del servicio de contraseñas
                var passwordService = new PasswordService();

                // Crear el hash y el salt de la contraseña
                passwordService.CreatePasswordHash(collection["Pass"], out string passwordHash, out string passwordSalt);

                // Crear el objeto de usuario con los datos proporcionados
                var user = new Users()
                {
                    Name = collection["Name"],
                    PasswordHash = passwordHash, // Asignar el hash de la contraseña
                    PasswordSalt = passwordSalt  // Asignar el salt de la contraseña
                };

                // Insertar el usuario en la base de datos
                db.InsertUsers(user);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


        // GET: UsersController/Edit/5
        public ActionResult Edit(string id)
        {
            var user = db.GetUserById(id);
            return View(user);
        }

        // POST: UsersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Edit(string id, IFormCollection collection)
        {
            try
            {
                // Obtener el usuario desde la base de datos usando el id proporcionado
                var user = db.GetUserById(id);

                if (user == null)
                {
                    // Manejar el caso donde el usuario no existe
                    return NotFound();
                }

                // Actualizar los datos del usuario con los valores proporcionados
                user.Name = collection["Name"];

                // Si se intenta cambiar la contraseña
                var currentPassword = collection["CurrentPassword"];
                var newPassword = collection["NewPassword"];
                var confirmPassword = collection["ConfirmPassword"];

                if (!string.IsNullOrEmpty(currentPassword) && !string.IsNullOrEmpty(newPassword) && !string.IsNullOrEmpty(confirmPassword))
                {
                    var passwordService = new PasswordService();

                    // Verificar si la contraseña actual es correcta
                    if (!passwordService.VerifyPasswordHash(currentPassword, user.PasswordHash, user.PasswordSalt))
                    {
                        ModelState.AddModelError(string.Empty, "La contraseña actual es incorrecta.");
                        return View(user);
                    }

                    // Verificar que la nueva contraseña coincida con la confirmación
                    if (newPassword != confirmPassword)
                    {
                        ModelState.AddModelError(string.Empty, "La nueva contraseña y la confirmación no coinciden.");
                        return View(user);
                    }

                    // Crear el hash y el salt de la nueva contraseña
                    passwordService.CreatePasswordHash(newPassword, out string passwordHash, out string passwordSalt);

                    // Actualizar la contraseña del usuario
                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                }

                // Guardar los cambios en la base de datos
                db.UpdateUsers(user);

                // Redirigir al índice de Users con un mensaje de éxito
                TempData["SuccessMessage"] = "El usuario ha sido actualizado exitosamente.";
                return RedirectToAction("Index", "Users");
            }
            catch
            {
                return View();
            }
        }





        // GET: UsersController/Delete/5
        public ActionResult Delete(string id)
        {
            var user = db.GetUserById(id);
            return View(user);
        }

        // POST: UsersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id, IFormCollection collection)
        {
            try
            {
                db.DeleteUsers(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
