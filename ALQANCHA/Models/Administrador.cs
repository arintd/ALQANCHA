using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
namespace ALQANCHA.Models
{
    public class Administrador
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "El nombre debe tener entre 5 y 30 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "El apellido debe tener entre 5 y 30 caracteres")]
        public string Apellido { get; set; }


        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [StringLength(15, MinimumLength = 5, ErrorMessage = "El teléfono debe tener entre 5 y 30 caracteres")]
        public string Telefono { get; set; }

        [Required]
        public bool Activo { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "Correo electrónico no válido")]
        public string Email { get; set; }
    }
}