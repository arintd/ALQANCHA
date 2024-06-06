using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ALQANCHA.Models;
using ALQCANCHA.Models;

namespace ALQANCHA.Models
{
    public class Reserva
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Administrador")]
        public int AdministradorId { get; set; }
        [ForeignKey("Cancha")]
        public int CanchaId { get; set; }
        [Display(Name = "Fecha de Reserva")]

        [Required]
        public DateTime Fecha { get; set; }
        [Display(Name = "Hora de Reserva")]

        [Required]
        public TimeSpan Hora { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "El nombre debe tener entre 5 y 30 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "El apellido debe tener entre 5 y 30 caracteres")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "La cantidad de jugadores es obligatoria")]
        [Range(1, 11, ErrorMessage = "La cantidad de jugadores debe estar entre 1 y 11")]
        public int CantJugadores { get; set; }
        public bool RequiereJugador { get; set; }
        public bool RequiereArquero { get; set; }
        public bool EsStream { get; set; }

        [EnumDataType(typeof(TipoReserva))]
        public TipoReserva TipoReserva { get; set; }
        public bool Confirmada { get; set; }

        // Propiedades de navegación
        public virtual Administrador Administrador { get; set; }
        public virtual Cancha Cancha { get; set; }
    }
}