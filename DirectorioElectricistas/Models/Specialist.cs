using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace DirectorioElectricistas.Models
{
    public class Specialist
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [Required(ErrorMessage = "El campo Nombre es obligatorio.")]
        [RegularExpression(@"^[a-zA-Z\sáéíóúÁÉÍÓÚ]+$", ErrorMessage = "El campo Nombre solo debe contener letras.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "El campo Tarjeta profesional es obligatorio.")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "El campo Tarjeta profesional debe tener entre 4 y 20 caracteres.")]

        public string CardId { get; set; }



        [Required(ErrorMessage = "El campo Número de contacto es obligatorio.")]
        [RegularExpression(@"^\d{1,15}$", ErrorMessage = "El campo Número de contacto debe contener solo números y hasta 15 dígitos.")]
        [StringLength(15, ErrorMessage = "El campo Número de contacto debe tener un máximo de 15 dígitos.")]
        public string Number { get; set; }



        [Required(ErrorMessage = "El campo Correo Electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        public string Email { get; set; }




        [Required(ErrorMessage = "El campo Departamento es obligatorio.")]


        public string Place { get; set; }

        [Required(ErrorMessage = "El campo Municipio es obligatorio.")]
        
        
        public string MainPlace { get; set; }


        public string Qualification{ get; set; }
        public string ImageUrl { get; set; }

        public string State { get; set; } = "Pendiente";
        public List<Note> Notes { get; set; }
    }
}
