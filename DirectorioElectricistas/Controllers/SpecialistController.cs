using DirectorioElectricistas.Models;
using DirectorioElectricistas.Repositories;
using DirectorioElectricistas.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using OfficeOpenXml;
using System.Collections.Generic;

namespace DirectorioElectricistas.Controllers
{
    [Authorize]
    public class SpecialistController : Controller
    {
        private ISpecialistCollection db = new SpecialistCollection();

        //THis is the admin versión


        private readonly EmailService _emailService;

        public SpecialistController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> Approve(string id)
        {
            var specialist = db.GetSpecialistById(id);
            if (specialist == null)
            {
                return NotFound();
            }

            try
            {
                // Intentar enviar el correo de aprobación antes de cambiar el estado
                await _emailService.SendEmailAsync(specialist.Email, "Registro Aprobado", "Tu registro en el directorio de electricistas ha sido aprobado.");

                // Si el correo se envía con éxito, aprobar el especialista
                specialist.State = "Aprobado";
                db.UpdateSpecialist(specialist);

                TempData["SuccessMessage"] = "Registro aprobado correctamente.";
            }
            catch (InvalidOperationException ex)
            {
                // Manejar el error de envío de correo
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        //Reject
        [HttpPost]
        public async Task<IActionResult> Reject(string id)
        {
            var specialist = db.GetSpecialistById(id);
            if (specialist == null)
            {
                return NotFound();
            }

            try
            {
                // Intentar enviar el correo de rechazo antes de cambiar el estado
                await _emailService.SendEmailAsync(specialist.Email, "Registro Rechazado", "Tu registro en el directorio de electricistas ha sido rechazado. Para volver a intentarlo, por favor registre sus datos correctamente.");

                // Si el correo se envía con éxito, rechazar el especialista
                specialist.State = "Rechazado";
                db.UpdateSpecialist(specialist);

                TempData["SuccessMessage"] = "Registro rechazado correctamente.";
            }
            catch (InvalidOperationException ex)
            {
                // Manejar el error de envío de correo
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }


        //bool (For image)
        private bool IsBase64String(string base64)
        {
            Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
            return Convert.TryFromBase64String(base64, buffer, out _);
        }


        //Generate report
        public IActionResult GenerateReport()
        {
            // Obtener todos los datos de la colección Specialist
            var specialists = db.GetAllSpecialists();

            // Crear un nuevo paquete de Excel
            using (var package = new ExcelPackage())
            {
                // Crear una nueva hoja de trabajo
                var worksheet = package.Workbook.Worksheets.Add("Specialists");

                // Agregar encabezados a las columnas
                worksheet.Cells[1, 1].Value = "Nombre";
                worksheet.Cells[1, 2].Value = "Número tarjeta profesional";
                worksheet.Cells[1, 3].Value = "Número de contacto";
                worksheet.Cells[1, 4].Value = "Correo electrónico";
                worksheet.Cells[1, 5].Value = "Departamento";
                worksheet.Cells[1, 6].Value = "Municipio";
                worksheet.Cells[1, 7].Value = "Estado";
                worksheet.Cells[1, 8].Value = "Calificación";
                worksheet.Cells[1, 9].Value = "Imágen";

                // Rellenar datos
                for (int i = 0; i < specialists.Count; i++)
                {
                    var specialist = specialists[i];
                    worksheet.Cells[i + 2, 1].Value = specialist.Name;
                    worksheet.Cells[i + 2, 2].Value = specialist.CardId;
                    worksheet.Cells[i + 2, 3].Value = specialist.Number;
                    worksheet.Cells[i + 2, 4].Value = specialist.Email;
                    worksheet.Cells[i + 2, 5].Value = specialist.Place;
                    worksheet.Cells[i + 2, 6].Value = specialist.MainPlace;
                    worksheet.Cells[i + 2, 7].Value = specialist.State;
                    worksheet.Cells[i + 2, 8].Value = specialist.Qualification;

                    // Tamaño predeterminado de la imagen
                    int imageWidth = 50;  // Ancho de la imagen en píxeles
                    int imageHeight = 50; // Altura de la imagen en píxeles

                    // Ajustar la altura de la fila para que coincida con la altura de la imagen
                    worksheet.Row(i + 2).Height = imageHeight * 0.75; // Ajuste basado en píxeles

                    // Convertir la imagen de base64 a binario e insertarla en la celda si es válida
                    if (!string.IsNullOrEmpty(specialist.ImageUrl) && IsBase64String(specialist.ImageUrl))
                    {
                        byte[] imageBytes = Convert.FromBase64String(specialist.ImageUrl);
                        using (var stream = new MemoryStream(imageBytes))
                        {
                            var excelImage = worksheet.Drawings.AddPicture("Image" + i, stream);
                            excelImage.SetPosition(i + 1, 0, 8, 0); // Establecer la posición de la imagen
                            excelImage.SetSize(imageWidth, imageHeight); // Establecer el tamaño de la imagen
                        }
                    }
                }

                // Ajustar el ancho de las columnas
                worksheet.Cells.AutoFitColumns();

                // Convertir el paquete a un arreglo de bytes
                var excelBytes = package.GetAsByteArray();

                // Devolver el archivo de Excel para descargar
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SpecialistsReport.xlsx");
            }
        }




        // GET: SpecialistController
        public ActionResult Index(string searchTerm, string department, string municipality, List<string> states)
        {
            // Obtener todos los especialistas
            List<Specialist> specialists = db.GetAllSpecialists();

            // Filtrar por término de búsqueda si se proporciona
            if (!string.IsNullOrEmpty(searchTerm))
            {
                specialists = specialists
                    .Where(s => s.Name.StartsWith(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Filtrar por departamento si se proporciona
            if (!string.IsNullOrEmpty(department))
            {
                specialists = specialists
                    .Where(s => s.Place.Equals(department, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Filtrar por municipio si se proporciona
            if (!string.IsNullOrEmpty(municipality))
            {
                specialists = specialists
                    .Where(s => s.MainPlace.Equals(municipality, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Filtrar por estados si se proporcionan
            if (states != null && states.Any())
            {
                specialists = specialists
                    .Where(s => states.Contains(s.State))
                    .ToList();
            }

            // Verificar si hay resultados
            bool hasResults = specialists.Any();

            // Pasar el indicador de resultados a la vista
            ViewBag.HasResults = hasResults;

            return View(specialists);
        }




        // GET: SpecialistController/Details/5
        public ActionResult Details(string id)
        {
            var specialist = db.GetSpecialistById(id);
            return View(specialist);
        }

        // GET: SpecialistController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SpecialistController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Specialist specialist, IFormFile imageFile)
        {
            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".jfif" };
                    var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("ImageFile", "Ingrese un tipo de imagen válido.");
                        return View(specialist);
                    }

                    using (var ms = new MemoryStream())
                    {
                        imageFile.CopyTo(ms);
                        var imageBytes = ms.ToArray();
                        specialist.ImageUrl = Convert.ToBase64String(imageBytes);
                    }
                }
                else
                {
                    ModelState.AddModelError("ImageFile", "Ingrese un tipo de imagen válido.");
                    return View(specialist);
                }

                var existingSpecialistApproved = db.GetSpecialistByCardIdAndState(specialist.CardId, "Aprobado");
                var existingSpecialistPending = db.GetSpecialistByCardIdAndState(specialist.CardId, "Pendiente");
                var existingSpecialistRejected = db.GetSpecialistByCardIdAndState(specialist.CardId, "Rechazado");

                if (existingSpecialistApproved != null)
                {
                    ModelState.AddModelError("CardId", "Ya existe un especialista aprobado con este número de tarjeta profesional.");
                    return View(specialist);
                }

                if (existingSpecialistPending != null)
                {
                    ModelState.AddModelError("CardId", "Hay un especialista con esta tarjeta profesional en espera de aprobación.");
                    return View(specialist);
                }

                if (existingSpecialistRejected != null)
                {
                    if (existingSpecialistPending != null)
                    {
                        ModelState.AddModelError("CardId", "No se puede agregar más registros con esta tarjeta profesional, ya hay uno en espera de aprobación.");
                        return View(specialist);
                    }
                }

                db.InsertSpecialist(specialist);

                // Guardar el mensaje de éxito en TempData
                TempData["SuccessMessage"] = "El registro fue realizado con éxito y está pendiente de aprobación. Se le notificará a su correo electrónico una vez aprobado.";

                // Regresar a la vista Create para mostrar el modal
                return View(specialist);
            }
            catch
            {
                return View(specialist);
            }
        }











        // GET: SpecialistController/Edit/5
        public ActionResult Edit(string id)
        {

            var specialist = db.GetSpecialistById(id);
            return View(specialist);
        }

        // POST: SpecialistController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, Specialist specialist, IFormFile imageFile, string currentImageBase64)
        {
            try
            {
                var existingSpecialist = db.GetSpecialistById(id);
                if (existingSpecialist == null)
                {
                    return NotFound();
                }

                bool isCardIdChanged = existingSpecialist.CardId != specialist.CardId;
                bool isImageChanged = imageFile != null && imageFile.Length > 0;

                // Manejo de la imagen
                if (isImageChanged)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".jfif" };
                    var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("ImageFile", "Ingrese un tipo de imagen válido.");
                        return View(specialist);
                    }

                    using (var ms = new MemoryStream())
                    {
                        imageFile.CopyTo(ms);
                        var imageBytes = ms.ToArray();
                        specialist.ImageUrl = Convert.ToBase64String(imageBytes);
                    }
                }
                else
                {
                    // No se ha seleccionado una nueva imagen, conserva la imagen existente
                    specialist.ImageUrl = currentImageBase64;
                }

                // Validaciones de CardId solo si ha cambiado
                if (isCardIdChanged)
                {
                    var existingSpecialistApproved = db.GetSpecialistByCardIdAndState(specialist.CardId, "Aprobado");
                    var existingSpecialistPending = db.GetSpecialistByCardIdAndState(specialist.CardId, "Pendiente");
                    var existingSpecialistRejected = db.GetSpecialistByCardIdAndState(specialist.CardId, "Rechazado");

                    if (existingSpecialistApproved != null && existingSpecialistApproved.Id != existingSpecialist.Id)
                    {
                        ModelState.AddModelError("CardId", "Ya existe un especialista aprobado con este número de tarjeta profesional.");
                        return View(specialist);
                    }

                    if (existingSpecialistPending != null && existingSpecialistPending.Id != existingSpecialist.Id)
                    {
                        ModelState.AddModelError("CardId", "Hay un especialista con esta tarjeta profesional en espera de aprobación.");
                        return View(specialist);
                    }

                    if (existingSpecialistRejected != null && existingSpecialistPending != null)
                    {
                        ModelState.AddModelError("CardId", "No se puede agregar más registros con esta tarjeta profesional, ya hay uno en espera de aprobación.");
                        return View(specialist);
                    }

                    existingSpecialist.State = "Pendiente";
                }

                // Actualizar datos del especialista
                existingSpecialist.Name = specialist.Name;
                existingSpecialist.CardId = specialist.CardId;
                existingSpecialist.Number = specialist.Number;
                existingSpecialist.Email = specialist.Email;
                existingSpecialist.Place = specialist.Place;
                existingSpecialist.MainPlace = specialist.MainPlace;
                existingSpecialist.ImageUrl = specialist.ImageUrl;

                db.UpdateSpecialist(existingSpecialist);

                // Guardar mensaje de éxito y redirigir
                TempData["SuccessMessage"] = "El registro se ha actualizado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                // Manejo de errores
                ModelState.AddModelError("", "Ocurrió un error al actualizar el registro.");
                return View(specialist);
            }
        }




        public ActionResult EditConfirmation(string id)
        {
            var specialist = db.GetSpecialistById(id);
            if (specialist == null)
            {
                return NotFound();
            }

            ViewBag.ShowConfirmationModal = TempData["ShowConfirmationModal"] != null;
            return View("Edit", specialist);
        }



        //No approve
        [HttpPost]
        public async Task<IActionResult> ChangeState(string id, string newState)
        {
            var specialist = db.GetSpecialistById(id);
            if (specialist == null)
            {
                return NotFound();
            }

            // Actualizar el estado del especialista
            specialist.State = newState;
            db.UpdateSpecialist(specialist);

            // No redirigir, simplemente actualizar el estado
            return RedirectToAction(nameof(Edit), new { id = specialist.Id });
        }







        // GET: SpecialistController/Delete/5
        public ActionResult Delete(string id)
        {
            var specialist = db.GetSpecialistById(id);
            return View(specialist);
        }

        // POST: SpecialistController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id, IFormCollection collection)
        {
            try
            {
                db.DeleteSpecialist(id);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


        
    }
}
