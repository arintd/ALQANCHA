using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ALQANCHA.Models
{
    public class Jugador
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Dni { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Telefono { get; set; }
        public bool EsArquero { get; set; }
        public bool EsJugador { get; set; }
        [Display(Name = "Fecha Disponible")]
        public DateTime FechaDisponible { get; set; }
        [Display(Name = "Hora Disponible")]
        public TimeSpan HoraDisponible { get; set; }
        public bool EstaSancionado { get; set; }
        public Sancion Sancion { get; set; }
    }
}