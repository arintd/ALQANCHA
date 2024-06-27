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
                .Where(rj => rj.Reserva.FechaReserva == fechaReserva && rj.Reserva.HoraInicio == horaInicio && rj.Reserva.Confirmada)
                .Select(rj => rj.JugadorId)
                .ToList();

            return _context.Jugadores
                .Where(j => j.FechaDisponible == fechaReserva && j.HoraInicio == horaInicio && !j.EstaSancionado &&
                            ((requiereJugador && j.EsJugador) || (requiereArquero && j.EsArquero)) &&
                            !jugadoresReservados.Contains(j.Id) && !j.EstaBloqueado) // Añado atributo para asegurarse de que solo se obtengan los jugadores que no están bloqueados.
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
<<<<<<< Updated upstream
        public async Task<IActionResult> Create([Bind("Id,AdministradorId,CanchaId,FechaReserva,HoraInicio,Nombre,Apellido,Telefono,Correo,CantJugadores,RequiereJugador,RequiereArquero,EsStream,TipoReserva,Confirmada")] Reserva reserva, int[] JugadoresSeleccionados)
        {   
=======
        public async Task<IActionResult> Create([Bind("Id,AdministradorId,CanchaId,FechaReserva,HoraInicio,Nombre,Apellido,Telefono,Correo,CantJugadores,RequiereJugador,RequiereArquero,EsStream,TipoReserva,Confirmada")] Reserva reserva, int[] ReservaJugadores)
        {
>>>>>>> Stashed changes
            if (ModelState.IsValid)
            {
                try
                {
                    var cancha = await _context.Canchas.FindAsync(reserva.CanchaId);
                    if (cancha == null)
                    {
                        ModelState.AddModelError("CanchaId", "Cancha no encontrada.");
                        CargarViewDataParaReserva(reserva);
                        return View(reserva);
                    }

<<<<<<< Updated upstream


                    // Validar disponibilidad de la cancha
                    bool canchaReservada = _context.Reservas
                        .Any(r => r.CanchaId == reserva.CanchaId
                               && r.FechaReserva == reserva.FechaReserva
                               && r.HoraInicio == reserva.HoraInicio
                               && r.Confirmada
                               && r.Id != reserva.Id); 

                    if (canchaReservada)
                    {
                        ModelState.AddModelError("CanchaId", "La cancha ya está reservada para la fecha y hora seleccionadas.");
                        return View(reserva);
                    }

                    // Obtener jugadores reservados para la misma fecha y hora
                    var jugadoresReservados = await _context.ReservaJugadores
                        .Where(rj => rj.Reserva.FechaReserva == reserva.FechaReserva && rj.Reserva.HoraInicio == reserva.HoraInicio)
                        .Select(rj => rj.JugadorId)
                        .ToListAsync();

                    // Verificar disponibilidad y bloqueo de los jugadores seleccionados
                    foreach (var jugadorId in JugadoresSeleccionados)
                    {
                        var jugador = await _context.Jugadores.FindAsync(jugadorId);
                        if (jugador == null)
                        {
                            ModelState.AddModelError("JugadoresSeleccionados", $"El jugador con ID {jugadorId} no existe.");
                            return View(reserva);
                        }

                        if (jugadoresReservados.Contains(jugadorId))
                        {
                            ModelState.AddModelError("JugadoresSeleccionados", $"El jugador con ID {jugadorId} ya está reservado para la fecha y hora seleccionadas.");
                            return View(reserva);
                        }

                        if (jugador.EstaBloqueado)
                        {
                            ModelState.AddModelError("JugadoresSeleccionados", $"El jugador con ID {jugadorId} está bloqueado y no puede ser asignado a la reserva.");
                            return View(reserva);
                        }

                        // Marcar jugador como bloqueado
                        jugador.EstaBloqueado = true;

                        // Actualizar disponibilidad del jugador
                        jugador.FechaDisponible = reserva.FechaReserva;
                        jugador.HoraInicio = reserva.HoraInicio;

                        _context.Update(jugador);

                    }
                    int minJugadores = 0;
                    if (reserva.RequiereJugador) minJugadores += 1;
                    if (reserva.RequiereArquero) minJugadores += 1;

                    if (reserva.CantJugadores < minJugadores || reserva.CantJugadores > cancha.CantJugadores)
=======
                    // Verificar si la cancha ya está reservada en la misma fechahora
                    bool canchaOcupada = _context.Reservas.Any(r => r.CanchaId == reserva.CanchaId && r.FechaReserva == reserva.FechaReserva && r.HoraInicio == reserva.HoraInicio && r.Confirmada);
                    if (canchaOcupada)
>>>>>>> Stashed changes
                    {
                        ModelState.AddModelError("CanchaId", "La cancha ya está reservada en esta fecha y hora.");
                        CargarViewDataParaReserva(reserva);
                        return View(reserva);
                    }

<<<<<<< Updated upstream
                    // Incrementar precio si es Stream
                    if (reserva.EsStream)
                    {
                        cancha.PrecioXHora = (int)(cancha.PrecioXHora * 1.15m); // Aumento del 15%
                        _context.Update(cancha);
                        await _context.SaveChangesAsync();
                    }

                    // Agregar la reserva al contexto y guardarla en la base de datos
                    _context.Add(reserva);
                    await _context.SaveChangesAsync();

=======
                    // Verificar si los jugadores seleccionados ya están reservados en la misma fechahora
                    foreach (var jugadorId in ReservaJugadores)
                    {
                        bool jugadorOcupado = _context.ReservaJugadores.Any(rj => rj.JugadorId == jugadorId && rj.Reserva.FechaReserva == reserva.FechaReserva && rj.Reserva.HoraInicio == reserva.HoraInicio && rj.Reserva.Confirmada);
                        if (jugadorOcupado)
                        {
                            ModelState.AddModelError("ReservaJugadores", $"El jugador con ID {jugadorId} ya está reservado en esta fecha y hora.");
                            CargarViewDataParaReserva(reserva);
                            return View(reserva);
                        }
                    }

>>>>>>> Stashed changes
                    if (reserva.Confirmada)
                    {
                        // Bloquear la cancha en esa fechahora seleccionada
                        cancha.Reservada = true;
                        _context.Update(cancha);

<<<<<<< Updated upstream
                            // Asociar jugadores a la reserva
                            if (JugadoresSeleccionados != null && JugadoresSeleccionados.Length > 0)
=======
                        // Bloquear los jugadores seleccionados
                        foreach (var jugadorId in ReservaJugadores)
                        {
                            var jugador = await _context.Jugadores.FindAsync(jugadorId);
                            if (jugador != null)
>>>>>>> Stashed changes
                            {
                                foreach (var jugadorSeleccionadoId in JugadoresSeleccionados)
                                {
                                    reserva.ReservaJugadores.Add(new ReservaJugador { ReservaId = reserva.Id, JugadorId = jugadorSeleccionadoId });
                                }
                                await _context.SaveChangesAsync();
                            }


                        }


                    if (ReservaJugadores != null && ReservaJugadores.Length > 0)
                    {
<<<<<<< Updated upstream
                        foreach (var jugadorSeleccionadoId in JugadoresSeleccionados)
=======
                        foreach (var jugadorId in ReservaJugadores)
>>>>>>> Stashed changes
                        {
                            _context.ReservaJugadores.Add(new ReservaJugador { ReservaId = reserva.Id, JugadorId = jugadorSeleccionadoId });
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

            CargarViewDataParaReserva(reserva);
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
                        ModelState.AddModelError("CanchaId", "Cancha no existe.");
                        CargarViewDataParaReserva(reserva);
                        return View(reserva);
                    }

<<<<<<< Updated upstream


                    // Validar disponibilidad de la cancha
                    bool canchaReservada = _context.Reservas
                        .Any(r => r.CanchaId == reserva.CanchaId
                               && r.FechaReserva == reserva.FechaReserva
                               && r.HoraInicio == reserva.HoraInicio
                               && r.Confirmada
                               && r.Id != reserva.Id);

                    if (canchaReservada)
                    {
                        ModelState.AddModelError("CanchaId", "La cancha ya está reservada para la fecha y hora seleccionadas.");
                        return View(reserva);
                    }

                    // Validar que los jugadores seleccionados no estén asignados a otra reserva para la misma fecha y hora
                    var jugadoresAsignadosOcupados = _context.ReservaJugadores
                        .Include(rj => rj.Reserva)
                        .Where(rj => JugadoresSeleccionados.Contains(rj.JugadorId)
                                  && rj.Reserva.FechaReserva == reserva.FechaReserva
                                  && rj.Reserva.HoraInicio == reserva.HoraInicio)
                        .ToList();

                    if (jugadoresAsignadosOcupados.Any())
                    {
                        var jugadoresOcupados = jugadoresAsignadosOcupados.Select(rj => rj.Jugador.Nombre).Distinct();
                        ModelState.AddModelError(string.Empty, $"Los siguientes jugadores ya están asignados para otra reserva en el mismo horario y fecha: {string.Join(", ", jugadoresOcupados)}");
                        return View(reserva);
                    }

                    int minJugadores = 0;
                    if (reserva.RequiereJugador) minJugadores += 1;
                    if (reserva.RequiereArquero) minJugadores += 1;

                    if (reserva.CantJugadores < minJugadores || reserva.CantJugadores > cancha.CantJugadores)
                    {
                        ModelState.AddModelError("CantJugadores", $"La cantidad de jugadores debe estar entre {minJugadores} y {cancha.CantJugadores}.");
                        return View(reserva);
                    }


=======
                    // Verificar si la cancha ya está reservada en esa misma fechahora
                    bool canchaOcupada = _context.Reservas.Any(r => r.CanchaId == reserva.CanchaId && r.FechaReserva == reserva.FechaReserva && r.HoraInicio == reserva.HoraInicio && r.Confirmada && r.Id != reserva.Id);
                    if (canchaOcupada)
                    {
                        ModelState.AddModelError("CanchaId", "La cancha ya está reservada en esta fecha y hora.");
                        CargarViewDataParaReserva(reserva);
                        return View(reserva);
                    }

                    // Verificar si los jugadores seleccionados ya están reservados en esa fechahora
                    foreach (var jugadorId in JugadoresSeleccionados)
                    {
                        bool jugadorOcupado = _context.ReservaJugadores.Any(rj => rj.JugadorId == jugadorId && rj.Reserva.FechaReserva == reserva.FechaReserva && rj.Reserva.HoraInicio == reserva.HoraInicio && rj.Reserva.Confirmada && rj.ReservaId != reserva.Id);
                        if (jugadorOcupado)
                        {
                            ModelState.AddModelError("JugadoresSeleccionados", $"El jugador con ID {jugadorId} ya está reservado en esta fecha y hora.");
                            CargarViewDataParaReserva(reserva);
                            return View(reserva);
                        }
                    }

>>>>>>> Stashed changes
                    if (reserva.Confirmada)
                    {
                        // Bloquear la cancha para la fechahora seleccionada
                        cancha.Reservada = true;
                        _context.Update(cancha);

<<<<<<< Updated upstream
                        // Incrementar precio si es Stream
                        if (reserva.EsStream)
                        {
                            cancha.PrecioXHora = (int)(cancha.PrecioXHora * 1.15m); // Aumento del 15%
                            _context.Update(cancha);
                            await _context.SaveChangesAsync();
                        }

                        // Bloquear los jugadores seleccionados
=======
                        // Bloquear los jugadores seleccionados, si es que hay
>>>>>>> Stashed changes
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

                    // Actualizar ReservaJugadores
                    var reservaJugadoresExistentes = _context.ReservaJugadores.Where(rj => rj.ReservaId == reserva.Id).ToList();
                    _context.ReservaJugadores.RemoveRange(reservaJugadoresExistentes);

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

            CargarViewDataParaReserva(reserva);
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
        public IActionResult ObtenerDisponiblesAjax(DateTime fechaReserva, TimeSpan horaInicio, bool requiereJugador, bool requiereArquero)
        {
            var canchasDisponibles = _context.Canchas
                .Where(c => !_context.Reservas.Any(r => r.CanchaId == c.Id && r.FechaReserva == fechaReserva && r.HoraInicio == horaInicio && r.Confirmada))
                .Select(c => new { c.Id, c.Descripcion })
                .ToList();

            var jugadoresDisponibles = ObtenerJugadoresDisponibles(fechaReserva, horaInicio, requiereJugador, requiereArquero)
                .Select(j => new { j.Id, j.Nombre })
                .ToList();

            return Json(new { canchasDisponibles, jugadoresDisponibles });
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

        private void CargarViewDataParaReserva(Reserva reserva)
        {
            ViewData["AdministradorId"] = new SelectList(_context.Administradores, "Id", "Nombre", reserva.AdministradorId);
            ViewData["CanchaId"] = new SelectList(_context.Canchas, "Id", "Descripcion", reserva.CanchaId);
            ViewData["HorasDisponibles"] = new SelectList(Enumerable.Range(10, 15).Select(h => new { Value = new TimeSpan(h, 0, 0), Text = $"{h}:00" }), "Value", "Text", reserva.HoraInicio);
            ViewData["JugadoresDisponibles"] = new MultiSelectList(ObtenerJugadoresDisponibles(reserva.FechaReserva, reserva.HoraInicio, reserva.RequiereJugador, reserva.RequiereArquero), "Id", "Nombre");
            ViewData["TipoReserva"] = new SelectList(Enum.GetValues(typeof(TipoReserva)));
        }
    }
}
