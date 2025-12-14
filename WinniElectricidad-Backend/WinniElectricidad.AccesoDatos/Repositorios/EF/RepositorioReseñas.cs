using Microsoft.EntityFrameworkCore;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.AccesoDatos.Repositorios.EF;

public class RepositorioReseñas : IRepositorioReseña
{
    private readonly WinniElectricidadContext _db;

    public RepositorioReseñas(WinniElectricidadContext db)
    {
        _db = db;
    }
    public async Task<Reseña?> Add(Reseña reseña, CancellationToken ct = default)
    {
        await _db.Resenias.AddAsync(reseña, ct);
        await _db.SaveChangesAsync(ct);

        return reseña;
    }

    public Task<Reseña?> FindById(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task Update(Reseña obj, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task Delete(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyList<Reseña>> FindAll(CancellationToken ct = default)
    {
        return await _db.Resenias
            .Where(r => r.Estado == EstadoReseña.Aprobada)
            .ToListAsync(ct);
    }
}