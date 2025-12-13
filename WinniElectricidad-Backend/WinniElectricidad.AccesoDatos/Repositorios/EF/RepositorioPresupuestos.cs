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


    public Task<Presupuesto?> FindById(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task Update(Presupuesto obj, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task Delete(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Presupuesto>> FindAll(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}