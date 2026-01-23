using Microsoft.EntityFrameworkCore;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.AccesoDatos.Repositorios.EF;

public class RepositorioPresupuestos : IRepositorioPresupuesto
{
    private readonly WinniElectricidadContext _db;

    public RepositorioPresupuestos(WinniElectricidadContext db)
    {
        _db = db;
    }
    public async Task<Presupuesto?> Add(Presupuesto obj, CancellationToken ct = default)
    {
        _db.Presupuestos.Add(obj);
        await _db.SaveChangesAsync(ct);
        return obj;
    }


    public async Task<Presupuesto?> FindById(int id, CancellationToken ct = default)
    {
        return await _db.Presupuestos
            .Include(p => p.Pagos)
            .FirstOrDefaultAsync(p => p.Id == id, ct);    }

    public async Task Update(Presupuesto presupuesto, CancellationToken ct = default)
    {
        _db.Presupuestos.Update(presupuesto);
        await _db.SaveChangesAsync(ct);
    }

    public async Task Delete(int id, CancellationToken ct = default)
    {
        var presupuesto = await _db.Presupuestos
            .FirstOrDefaultAsync(p => p.Id == id, ct);
        if (presupuesto == null) return; 
        
        _db.Presupuestos.Remove(presupuesto);
        await _db.SaveChangesAsync(ct);
        
    }

    public async Task<IReadOnlyList<Presupuesto>> FindAll(CancellationToken ct = default)
    {
        return await _db.Presupuestos
            .AsNoTracking()
            .OrderByDescending(p => p.FechaPresupuesto)
            .ToListAsync(ct);
    }
    
    public async Task<Presupuesto?> FindByReservaId(int idReserva, CancellationToken ct = default)
    {
        return await _db.Presupuestos
            .Include(p => p.Pagos.Where(pago => pago.EstadoPago == EstadoPago.Confirmado))
            .FirstOrDefaultAsync(p => p.IdReserva == idReserva, ct);
    }
    public async Task<IEnumerable<Presupuesto>> ObtenerPresupuestosConReservaPorUsuario(int idUsuario, CancellationToken ct = default)
    {
        var presupuestos = await _db.Presupuestos
            .AsNoTracking()
            .Include(p => p.Reserva)
            .Include(p => p.Pagos.Where(pago => pago.EstadoPago == EstadoPago.Confirmado))
            .Include(p => p.Reserva.Direccion)
            .Include(p => p.Reserva.Servicios)
            .Where(p => p.IdUsuario == idUsuario)
            .OrderByDescending(p => p.FechaPresupuesto)
            .ToListAsync(ct);

        return presupuestos;
        // TODO, filtrar los que ya se pagaron por completo y que la reserva no este cancealada creo
    }
}