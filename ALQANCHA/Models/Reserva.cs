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
        public DateTime Fecha { get; set; }
        [Display(Name = "Hora de Reserva")]
        public TimeSpan Hora { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
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