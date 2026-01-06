using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.AccesoDatos.Repositorios.EF;

public class RepositorioPagos : IRepositorioPago
{
    private readonly WinniElectricidadContext _db;

    public RepositorioPagos(WinniElectricidadContext db)
    {
        _db = db;
    }
    public async Task<Pago?> Add(Pago obj, CancellationToken ct = default)
    {
        await _db.Pagos.AddAsync(obj, ct);
        await _db.SaveChangesAsync(ct);
        return obj;
    }

    public Task<Pago?> FindById(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task Update(Pago obj, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task Delete(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Pago>> FindAll(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}