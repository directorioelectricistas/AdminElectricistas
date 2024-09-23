using DirectorioElectricistas.Models;
using DirectorioElectricistas.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DirectorioElectricistas.Controllers
{
    [Authorize]
    public class NoteController : Controller
    {
        private ISpecialistCollection db= new SpecialistCollection();
        public IActionResult Index()
        {
            return View();
        }

        // GET: NoteController/Create
        public ActionResult Create(string id)
        {
            NoteViewModel noteVM = new NoteViewModel()
            {
                SpecialistId = id, Note = new Note()
            };
            return View(noteVM);
        }

        // POST: NoteController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                var specialist = db.GetSpecialistById(collection["SpecialistId"]);
                if (specialist == null)
                {
                    return NotFound();
                }

                if (specialist.Notes == null)
                {
                    specialist.Notes = new List<Note>();
                }

                // Agregar la nueva nota
                var note = new Note
                {
                    Value = int.Parse(collection["Note.Value"]),
                    Observation = collection["Note.Observation"] // Puede ser nulo o vacío
                };
                specialist.Notes.Add(note);

                // Recalcular el promedio de calificación
                double averageQualification = specialist.Notes.Average(n => n.Value);
                specialist.Qualification = Math.Round(averageQualification, 2).ToString("F2");

                // Actualizar el specialist en la base de datos
                db.UpdateSpecialist(specialist);

                return RedirectToAction("Index", "Specialist");
            }
            catch
            {
                return View();
            }
        }




    }
}
