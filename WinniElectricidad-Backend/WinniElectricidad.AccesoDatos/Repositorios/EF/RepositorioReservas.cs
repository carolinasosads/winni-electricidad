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
    
    public Task Add(Reserva obj, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<Reserva?> FindById(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task Update(Reserva obj, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task Delete(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Reserva>> FindAll(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyList<Reserva>> FindAllBetweenDates(DateTime fechaMinima, DateTime fechaLimite, CancellationToken ct = default)
    {
        return await _db.Reservas
            .AsNoTracking()
            .Where(r => r.FechaReserva.Date >= fechaMinima 
                        && r.FechaReserva.Date <= fechaLimite
                        && r.EstadoReserva != EstadoReserva.Cancelada)
            .ToListAsync(ct);
    }
}