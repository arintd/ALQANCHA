﻿// <auto-generated />
using System;
using ALQCANCHA.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ALQANCHA.Migrations
{
    [DbContext(typeof(AlqanchaDatabaseContext))]
    [Migration("20240626062324_CrearReservaJugador")]
    partial class CrearReservaJugador
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ALQANCHA.Models.Administrador", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Apellido")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Telefono")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.HasKey("Id");

                    b.ToTable("Administradores");
                });

            modelBuilder.Entity("ALQANCHA.Models.Cancha", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CantJugadores")
                        .HasColumnType("int");

                    b.Property<bool>("Confirmada")
                        .HasColumnType("bit");

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<int>("PrecioXHora")
                        .HasColumnType("int");

                    b.Property<bool>("Reservada")
                        .HasColumnType("bit");

                    b.Property<string>("Telefono")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.HasKey("Id");

                    b.ToTable("Canchas");
                });

            modelBuilder.Entity("ALQANCHA.Models.Jugador", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Apellido")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("Dni")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("EsArquero")
                        .HasColumnType("bit");

                    b.Property<bool>("EsJugador")
                        .HasColumnType("bit");

                    b.Property<bool>("EstaSancionado")
                        .HasColumnType("bit");

                    b.Property<DateTime>("FechaDisponible")
                        .HasColumnType("datetime2");

                    b.Property<TimeSpan>("HoraInicio")
                        .HasColumnType("time");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("Telefono")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.HasKey("Id");

                    b.ToTable("Jugadores");
                });

            modelBuilder.Entity("ALQANCHA.Models.Reserva", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("AdministradorId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<string>("Apellido")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<int?>("CanchaId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<int>("CantJugadores")
                        .HasColumnType("int");

                    b.Property<bool>("Confirmada")
                        .HasColumnType("bit");

                    b.Property<string>("Correo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("EsStream")
                        .HasColumnType("bit");

                    b.Property<DateTime>("FechaReserva")
                        .HasColumnType("datetime2");

                    b.Property<TimeSpan>("HoraInicio")
                        .HasColumnType("time");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<bool>("RequiereArquero")
                        .HasColumnType("bit");

                    b.Property<bool>("RequiereJugador")
                        .HasColumnType("bit");

                    b.Property<string>("Telefono")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TipoReserva")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AdministradorId");

                    b.HasIndex("CanchaId");

                    b.ToTable("Reservas");
                });

            modelBuilder.Entity("ALQANCHA.Models.ReservaJugador", b =>
                {
                    b.Property<int>("ReservaId")
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    b.Property<int>("JugadorId")
                        .HasColumnType("int")
                        .HasColumnOrder(1);

                    b.HasKey("ReservaId", "JugadorId");

                    b.HasIndex("JugadorId");

                    b.ToTable("ReservaJugadores");
                });

            modelBuilder.Entity("ALQANCHA.Models.Sancion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<DateTime>("FechaImposicion")
                        .HasColumnType("datetime2");

                    b.Property<int>("JugadorId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("JugadorId");

                    b.ToTable("Sanciones");
                });

            modelBuilder.Entity("ALQANCHA.Models.Reserva", b =>
                {
                    b.HasOne("ALQANCHA.Models.Administrador", "Administrador")
                        .WithMany("Reservas")
                        .HasForeignKey("AdministradorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("ALQANCHA.Models.Cancha", "Cancha")
                        .WithMany()
                        .HasForeignKey("CanchaId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Administrador");

                    b.Navigation("Cancha");
                });

            modelBuilder.Entity("ALQANCHA.Models.ReservaJugador", b =>
                {
                    b.HasOne("ALQANCHA.Models.Jugador", "Jugador")
                        .WithMany("ReservaJugadores")
                        .HasForeignKey("JugadorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ALQANCHA.Models.Reserva", "Reserva")
                        .WithMany("ReservaJugadores")
                        .HasForeignKey("ReservaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Jugador");

                    b.Navigation("Reserva");
                });

            modelBuilder.Entity("ALQANCHA.Models.Sancion", b =>
                {
                    b.HasOne("ALQANCHA.Models.Jugador", "Jugador")
                        .WithMany()
                        .HasForeignKey("JugadorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Jugador");
                });

            modelBuilder.Entity("ALQANCHA.Models.Administrador", b =>
                {
                    b.Navigation("Reservas");
                });

            modelBuilder.Entity("ALQANCHA.Models.Jugador", b =>
                {
                    b.Navigation("ReservaJugadores");
                });

            modelBuilder.Entity("ALQANCHA.Models.Reserva", b =>
                {
                    b.Navigation("ReservaJugadores");
                });
#pragma warning restore 612, 618
        }
    }
}
