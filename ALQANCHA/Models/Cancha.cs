using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ALQANCHA.Models
{
    public class Cancha
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CantJugadores { get; set; }
        public string Descripcion { get; set; }
        public string Telefono { get; set; }
        public int PrecioXHora { get; set; }
        public bool Reservada { get; set; }
        public bool Confirmada { get; set; }
    }
}