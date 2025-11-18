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

    public Task<Servicio?> FindById(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task Update(Servicio obj, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task Delete(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyList<Servicio>> FindAll(CancellationToken ct = default)
    {
        return await _db.Servicios.ToListAsync(ct);
    }
    
    public async Task<IReadOnlyList<Servicio>> FindAllActive(CancellationToken ct = default)
    {
        return await _db.Servicios.Where(s => s.Activo).ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Servicio>> FindByIds(List<int> ids, CancellationToken ct = default)
    {
        return await _db.Servicios
            .Where(s => ids.Contains(s.Id))
            .ToListAsync(ct);
    }
}