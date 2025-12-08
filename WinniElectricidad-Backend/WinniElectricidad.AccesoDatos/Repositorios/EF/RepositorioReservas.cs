using Microsoft.EntityFrameworkCore;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.AccesoDatos.Repositorios.EF;

public class RepositorioReservas : IRepositorioReserva
{
    private readonly WinniElectricidadContext _db;

    public RepositorioReservas(WinniElectricidadContext db)
    {
        _db = db;
    }

    public async Task<Reserva?> Add(Reserva reserva, CancellationToken ct = default)
    {
        await _db.Reservas.AddAsync(reserva, ct);
        await _db.SaveChangesAsync(ct);

        return reserva;
    }

    public async Task<Reserva?> FindById(int id, CancellationToken ct = default)
    {
        return await _db.Reservas
            .Include(r => r.Direccion)
            .Include(r => r.Servicios)
            .FirstOrDefaultAsync(r => r.IdReserva == id, ct);
    }

    public async Task Update(Reserva reserva, CancellationToken ct = default)
    {
        _db.Reservas.Update(reserva);
        await _db.SaveChangesAsync(ct);
    }

    public Task Delete(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Reserva>> FindAll(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyList<Reserva>> FindAllBetweenDates(DateTime fechaMinima, DateTime fechaLimite,
        CancellationToken ct = default)
    {
        return await _db.Reservas
            .AsNoTracking()
            .Include(r => r.UsuarioCliente)
            .Include(r => r.Direccion)
            .Include(r => r.Servicios)
            .Where(r => r.FechaReserva >= fechaMinima
                         && r.FechaReserva <= fechaLimite
                         && r.EstadoReserva != EstadoReserva.Cancelada)
            .ToListAsync(ct);
    }
    
    public async Task<IReadOnlyList<Reserva>> FindHistoricoReservas(DateTime fechaMinima, DateTime fechaLimite,
        CancellationToken ct = default)
    {
        return await _db.Reservas
            .AsNoTracking()
            .Include(r => r.UsuarioCliente)
            .Include(r => r.Direccion)
            .Include(r => r.Servicios)
            .Where(r => r.FechaReserva.Date >= fechaMinima
                        && r.FechaReserva.Date <= fechaLimite)
            .ToListAsync(ct);
    }
    public async Task<bool> UsuarioTieneReservaEnHorario(int idUsuario, DateTime fechaReserva,
        CancellationToken ct = default)
    {
        return await _db.Reservas
            .AnyAsync(r =>
                    r.IdUsuarioCliente == idUsuario &&
                    r.FechaReserva == fechaReserva &&
                    r.EstadoReserva != EstadoReserva.Cancelada,
                ct);
    }

    public async Task<bool> UsuarioTieneReservaEnDia(int idUsuario, DateTime fechaReserva,
        CancellationToken ct = default)
    {
        return await _db.Reservas
            .AnyAsync(r =>
                    r.IdUsuarioCliente == idUsuario &&
                    r.FechaReserva.Date == fechaReserva.Date &&
                    r.EstadoReserva != EstadoReserva.Cancelada,
                ct);
    }

    public async Task<bool> HorarioOcupado(DateTime fechaReserva, CancellationToken ct = default)
    {
        return await _db.Reservas
            .AnyAsync(r => r.FechaReserva == fechaReserva &&
                           r.EstadoReserva != EstadoReserva.Cancelada, ct);
    }

    public async Task<IReadOnlyList<Reserva>> FindAllFinalizadas(CancellationToken ct = default)
    {
        var hoy = DateTime.Today;

        return await _db.Reservas
            .AsNoTracking()
            .Include(r => r.UsuarioCliente)
            .Include(r => r.Direccion)
            .Include(r => r.Servicios)
            .Where(r =>
                r.FechaReserva.Date < hoy &&
                r.EstadoReserva != EstadoReserva.Cancelada)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Reserva>> FindAllSegunEstado(EstadoReserva estado, CancellationToken ct = default)
    {
        return await _db.Reservas
            .AsNoTracking()
            .Include(r => r.UsuarioCliente)
            .Include(r => r.Direccion)
            .Include(r => r.Servicios)
            .Where(r => r.EstadoReserva == estado)
            .ToListAsync(ct);
        
    }
    public async Task<Reserva?> ObtenerReservaPorId(int id, CancellationToken ct = default)
    {
        return await _db.Reservas
            .Include(r => r.UsuarioCliente)
            .Include(r => r.Servicios)       
            .Include(r => r.Direccion)      
            .FirstOrDefaultAsync(r => r.IdReserva == id, ct);
    }
}