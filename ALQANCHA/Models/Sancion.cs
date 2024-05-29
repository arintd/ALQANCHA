using ALQANCHA.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ALQANCHA.Models
{
    public class Sancion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Jugador")]
        public int JugadorId { get; set; }

        public string Descripcion { get; set; }

        [Display(Name = "Fecha de Imposición")]
        public DateTime FechaImposicion { get; set; }

        // Propiedad de navegación
        public virtual Jugador Jugador { get; set; }
    }
}