using Microsoft.EntityFrameworkCore;
using WinniElectricidad.LogicaNegocio.Entidades;
using WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

namespace WinniElectricidad.AccesoDatos.Repositorios.EF;

public class RepositorioOneTimeTokens : IRepositorioOneTimeToken
{
    private readonly WinniElectricidadContext _db;

    public RepositorioOneTimeTokens(WinniElectricidadContext db)
    {
        _db = db;
    }


    public async Task Add(OneTimeToken token, CancellationToken ct = default)
    {
        await _db.OneTimeTokens.AddAsync(token, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task MarkUsed(string tokenHash, CancellationToken ct = default)
    {
        await _db.OneTimeTokens
            .Where(t => t.TokenHash == tokenHash)
            .ExecuteUpdateAsync(
                s => s.SetProperty(t => t.Usado, true),
                ct
            );
    }

    public async Task<OneTimeToken?> GetActiveByHash(string tokenHash, CancellationToken ct = default)
    {
        var now = DateTimeOffset.UtcNow;
        return await _db.OneTimeTokens.FirstOrDefaultAsync(t => t.TokenHash == tokenHash && t.Tipo == OneTimeTokenTipo.ReseteoDeContrasena && !t.Usado && t.ExpiraEl > now, ct);
    }

    public async Task<OneTimeToken?> GetActiveByUser(int idUsuario, CancellationToken ct = default)
    {
        var now = DateTimeOffset.UtcNow;
        return await _db.OneTimeTokens.FirstOrDefaultAsync(
            t => t.IdUsuario == idUsuario 
                 && t.Tipo == OneTimeTokenTipo.ReseteoDeContrasena
                 && !t.Usado 
                 && t.ExpiraEl > now, ct);
    }
}