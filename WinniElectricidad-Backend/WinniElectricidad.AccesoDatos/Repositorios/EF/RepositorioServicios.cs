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
    
    public Task<Servicio?> Add(Servicio obj, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Servicio?> FindById(int id, CancellationToken ct = default)
    {
        var servicio = await _db.Servicios.Where(x => x.Id == id).FirstOrDefaultAsync(ct);
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
        return await _db.Servicios.ToListAsync(ct);
    }
    
    public async Task<IReadOnlyList<Servicio>> FindAllSegunEstado(bool activo, CancellationToken ct = default)
    {
        var servicios = await _db.Servicios.ToListAsync(ct);

        if (activo)
        {
            servicios = servicios.Where(s => s.Activo).ToList();
        }

        return servicios;
    }

    public async Task<IReadOnlyList<Servicio>> FindByIds(List<int> ids, CancellationToken ct = default)
    {
        return await _db.Servicios
            .Where(s => ids.Contains(s.Id))
            .ToListAsync(ct);
    }
}