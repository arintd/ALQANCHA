using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ALQANCHA.Models;
using ALQCANCHA.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ALQANCHA.Controllers
{
    public class ReservaController : Controller
    {
        private readonly AlqanchaDatabaseContext _context;

        public ReservaController(AlqanchaDatabaseContext context)
        {
            _context = context;
        }

        private IEnumerable<Jugador> ObtenerJugadoresDisponibles(DateTime fechaReserva, TimeSpan horaInicio, bool requiereJugador, bool requiereArquero)
        {
            var jugadoresReservados = _context.ReservaJugadores
                .Where(rj => rj.Reserva.FechaReserva == fechaReserva && rj.Reserva.HoraInicio == horaInicio)
                .Select(rj => rj.JugadorId)
                .ToList();

            return _context.Jugadores
                .Where(j => j.FechaDisponible == fechaReserva && j.HoraInicio == horaInicio && !j.EstaSancionado &&
                            ((requiereJugador && j.EsJugador) || (requiereArquero && j.EsArquero)) &&
                            !jugadoresReservados.Contains(j.Id))
                .ToList();
        }

        public async Task<IActionResult> Index()
        {
            var reservas = await _context.Reservas.Include(r => r.Administrador).Include(r => r.Cancha).ToListAsync();
            return View(reservas);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.Administrador)
                .Include(r => r.Cancha)
                .Include(r => r.ReservaJugadores)
                .ThenInclude(rj => rj.Jugador)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        public IActionResult Create()
        {
            var administrador = _context.Administradores.FirstOrDefault();
            if (administrador == null)
            {
                return NotFound("Administrador no encontrado.");
            }

            ViewData["AdministradorId"] = administrador.Id;
            ViewData["CanchaId"] = new SelectList(_context.Canchas, "Id", "Descripcion");
            ViewData["HorasDisponibles"] = new SelectList(Enumerable.Range(10, 15).Select(h => new { Value = new TimeSpan(h, 0, 0), Text = $"{h}:00" }), "Value", "Text");
            ViewData["JugadoresDisponibles"] = new MultiSelectList(new List<Jugador>(), "Id", "Nombre");
            ViewData["TipoReserva"] = new SelectList(Enum.GetValues(typeof(TipoReserva)));

            var reserva = new Reserva
            {
                AdministradorId = administrador.Id,
                CantJugadores = 0,
                FechaReserva = DateTime.Now
            };

            return View(reserva);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AdministradorId,CanchaId,FechaReserva,HoraInicio,Nombre,Apellido,Telefono,Correo,CantJugadores,RequiereJugador,RequiereArquero,EsStream,TipoReserva,Confirmada")] Reserva reserva, int[] JugadoresSeleccionados)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var cancha = await _context.Canchas.FindAsync(reserva.CanchaId);
                    if (cancha == null)
                    {
                        ModelState.AddModelError("CanchaId", "Cancha no encontrada.");
                        return View(reserva);
                    }

                    int minJugadores = 0;
                    if (reserva.RequiereJugador) minJugadores += 1;
                    if (reserva.RequiereArquero) minJugadores += 1;

                    if (reserva.CantJugadores < minJugadores || reserva.CantJugadores > cancha.CantJugadores)
                    {
                        ModelState.AddModelError("CantJugadores", $"La cantidad de jugadores debe estar entre {minJugadores} y {cancha.CantJugadores}.");
                        ViewData["AdministradorId"] = reserva.AdministradorId;
                        ViewData["CanchaId"] = new SelectList(_context.Canchas, "Id", "Descripcion", reserva.CanchaId);
                        ViewData["HorasDisponibles"] = new SelectList(Enumerable.Range(10, 15).Select(h => new { Value = new TimeSpan(h, 0, 0), Text = $"{h}:00" }), "Value", "Text", reserva.HoraInicio);
                        ViewData["JugadoresDisponibles"] = new MultiSelectList(ObtenerJugadoresDisponibles(reserva.FechaReserva, reserva.HoraInicio, reserva.RequiereJugador, reserva.RequiereArquero), "Id", "Nombre");
                        ViewData["TipoReserva"] = new SelectList(Enum.GetValues(typeof(TipoReserva)));
                        return View(reserva);
                    }

                    if (reserva.Confirmada)
                    {
                        // Bloquear la cancha para la fecha y hora seleccionadas
                        cancha.Reservada = true;
                        _context.Update(cancha);

                        // Bloquear los jugadores seleccionados
                        foreach (var jugadorId in JugadoresSeleccionados)
                        {
                            var jugador = await _context.Jugadores.FindAsync(jugadorId);
                            if (jugador != null)
                            {
                                jugador.FechaDisponible = reserva.FechaReserva;
                                jugador.HoraInicio = reserva.HoraInicio;
                                _context.Update(jugador);
                            }
                        }
                    }

                    _context.Add(reserva);
                    await _context.SaveChangesAsync();

                    if (JugadoresSeleccionados != null && JugadoresSeleccionados.Length > 0)
                    {
                        foreach (var jugadorId in JugadoresSeleccionados)
                        {
                            _context.ReservaJugadores.Add(new ReservaJugador { ReservaId = reserva.Id, JugadorId = jugadorId });
                        }
                        await _context.SaveChangesAsync();
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error al crear la reserva: {ex.Message}");
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                ModelState.AddModelError(string.Empty, $"ModelState no es válido. Errores: " + string.Join(", ", errors));
            }

            ViewData["AdministradorId"] = reserva.AdministradorId;
            ViewData["CanchaId"] = new SelectList(_context.Canchas, "Id", "Descripcion", reserva.CanchaId);
            ViewData["HorasDisponibles"] = new SelectList(Enumerable.Range(10, 15).Select(h => new { Value = new TimeSpan(h, 0, 0), Text = $"{h}:00" }), "Value", "Text", reserva.HoraInicio);
            ViewData["JugadoresDisponibles"] = new MultiSelectList(ObtenerJugadoresDisponibles(reserva.FechaReserva, reserva.HoraInicio, reserva.RequiereJugador, reserva.RequiereArquero), "Id", "Nombre");
            ViewData["TipoReserva"] = new SelectList(Enum.GetValues(typeof(TipoReserva)));
            return View(reserva);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.ReservaJugadores)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }

            ViewData["AdministradorId"] = new SelectList(_context.Administradores, "Id", "Nombre", reserva.AdministradorId);
            ViewData["CanchaId"] = new SelectList(_context.Canchas, "Id", "Descripcion", reserva.CanchaId);
            ViewData["HorasDisponibles"] = new SelectList(Enumerable.Range(10, 15).Select(h => new { Value = new TimeSpan(h, 0, 0), Text = $"{h}:00" }), "Value", "Text", reserva.HoraInicio);
            ViewData["JugadoresDisponibles"] = new MultiSelectList(ObtenerJugadoresDisponibles(reserva.FechaReserva, reserva.HoraInicio, reserva.RequiereJugador, reserva.RequiereArquero), "Id", "Nombre");
            ViewData["TipoReserva"] = new SelectList(Enum.GetValues(typeof(TipoReserva)));

            return View(reserva);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AdministradorId,CanchaId,FechaReserva,HoraInicio,Nombre,Apellido,Telefono,Correo,CantJugadores,RequiereJugador,RequiereArquero,EsStream,TipoReserva,Confirmada")] Reserva reserva, int[] JugadoresSeleccionados)
        {
            if (id != reserva.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var cancha = await _context.Canchas.FindAsync(reserva.CanchaId);
                    if (cancha == null)
                    {
                        ModelState.AddModelError("CanchaId", "Cancha no encontrada.");
                        return View(reserva);
                    }

                    if (reserva.Confirmada)
                    {
                        // Bloquear la cancha para la fecha y hora seleccionadas
                        cancha.Reservada = true;
                        _context.Update(cancha);

                        // Bloquear los jugadores seleccionados
                        foreach (var jugadorId in JugadoresSeleccionados)
                        {
                            var jugador = await _context.Jugadores.FindAsync(jugadorId);
                            if (jugador != null)
                            {
                                jugador.FechaDisponible = reserva.FechaReserva;
                                jugador.HoraInicio = reserva.HoraInicio;
                                _context.Update(jugador);
                            }
                        }
                    }

                    _context.Update(reserva);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservaExists(reserva.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["AdministradorId"] = new SelectList(_context.Administradores, "Id", "Nombre", reserva.AdministradorId);
            ViewData["CanchaId"] = new SelectList(_context.Canchas, "Id", "Descripcion", reserva.CanchaId);
            ViewData["HorasDisponibles"] = new SelectList(Enumerable.Range(10, 15).Select(h => new { Value = new TimeSpan(h, 0, 0), Text = $"{h}:00" }), "Value", "Text", reserva.HoraInicio);
            ViewData["JugadoresDisponibles"] = new MultiSelectList(ObtenerJugadoresDisponibles(reserva.FechaReserva, reserva.HoraInicio, reserva.RequiereJugador, reserva.RequiereArquero), "Id", "Nombre");
            ViewData["TipoReserva"] = new SelectList(Enum.GetValues(typeof(TipoReserva)));
            return View(reserva);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.Administrador)
                .Include(r => r.Cancha)
                .Include(r => r.ReservaJugadores)
                .ThenInclude(rj => rj.Jugador)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservaExists(int id)
        {
            return _context.Reservas.Any(e => e.Id == id);
        }

        [HttpGet]
        public IActionResult ObtenerJugadoresDisponiblesAjax(DateTime fechaReserva, TimeSpan horaInicio, bool requiereJugador, bool requiereArquero)
        {
            var jugadoresDisponibles = ObtenerJugadoresDisponibles(fechaReserva, horaInicio, requiereJugador, requiereArquero);
            return Json(jugadoresDisponibles.Select(j => new { j.Id, j.Nombre }));
        }

        [HttpGet]
        public IActionResult GetCanchaDetails(int id)
        {
            var cancha = _context.Canchas.Find(id);
            if (cancha == null)
            {
                return NotFound();
            }
            return Json(new { cancha.CantJugadores });
        }
    }
}
