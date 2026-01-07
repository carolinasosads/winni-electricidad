using Microsoft.EntityFrameworkCore;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.AccesoDatos.Repositorios.EF;

public class RepositorioServicios : IRepositorioServicio
{
    private readonly WinniElectricidadContext _db;

    public RepositorioServicios(WinniElectricidadContext db)
    {
        _db = db;
    }
    
    public async Task<Servicio?> Add(Servicio obj, CancellationToken ct = default)
    {
        await _db.Servicios.AddAsync(obj, ct);
        await _db.SaveChangesAsync(ct);

        return obj;
    }

    public async Task<Servicio?> FindById(int id, CancellationToken ct = default)
    {
        var servicio = await _db.Servicios
            .Include(s => s.Imagenes)
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(ct);
        return servicio;
    }

    public async Task Update(Servicio servicio, CancellationToken ct = default)
    {
        _db.Servicios.Update(servicio);
        await _db.SaveChangesAsync(ct);
    }

    public Task Delete(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyList<Servicio>> FindAll(CancellationToken ct = default)
    {
        return await _db.Servicios
            .Include(s => s.Imagenes)
            .ToListAsync(ct);
    }
    
    public async Task<IReadOnlyList<Servicio>> FindAllSegunEstado(bool activo, CancellationToken ct = default)
    {
        IQueryable<Servicio> query = _db.Servicios;
        
        if (activo)
        {
            query = query.Where(s => s.Activo);
        }
        
        return await query
            .Include(s => s.Imagenes)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Servicio>> FindByIds(List<int> ids, CancellationToken ct = default)
    {
        return await _db.Servicios
            .Where(s => ids.Contains(s.Id))
            .ToListAsync(ct);
    }
}