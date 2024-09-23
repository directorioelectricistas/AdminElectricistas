using System.ComponentModel.DataAnnotations;

namespace DirectorioElectricistas.Models
{
    public class Note
    {
        [Required(ErrorMessage = "El campo Valor es obligatorio.")]
        [Range(0, 5, ErrorMessage = "El valor debe estar entre 0 y 5.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "El valor debe ser un número entero entre 0 y 5.")]
        public int Value { get; set; }

        [Required(ErrorMessage = "Ingrese una observación.")]
        public string Observation { get; set; }
    }
}
