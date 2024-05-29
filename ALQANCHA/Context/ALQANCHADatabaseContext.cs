using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALQANCHA.Models;

namespace ALQCANCHA.Context
{
    public class AlqanchaDatabaseContext : DbContext
    {
        public AlqanchaDatabaseContext(DbContextOptions<AlqanchaDatabaseContext> options)
            : base(options)
        {
        }

        public DbSet<Administrador> Administradores { get; set; }
        public DbSet<Cancha> Canchas { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Jugador> Jugadores { get; set; }
        public DbSet<Sancion> Sanciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuraciones adicionales si son necesarias, por ejemplo:
            // modelBuilder.Entity<Reserva>().HasIndex(r => new { r.Fecha, r.Hora }).IsUnique();
        }
    }
}