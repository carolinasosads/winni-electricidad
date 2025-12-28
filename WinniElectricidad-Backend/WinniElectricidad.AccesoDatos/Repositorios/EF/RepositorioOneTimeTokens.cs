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
    
    public async Task Update(OneTimeToken token, CancellationToken ct = default)
    {
        _db.OneTimeTokens.Update(token);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<OneTimeToken?> GetActiveByHash(string tokenHash, CancellationToken ct = default)
    {
        var token = await _db.OneTimeTokens
            .Where(t =>
                t.TokenHash == tokenHash &&
                t.Tipo == OneTimeTokenTipo.ReseteoDeContrasena &&
                !t.Usado)
            .OrderByDescending(t => t.Id)
            .FirstOrDefaultAsync(ct);

        if (token is null)
            return null;

        return token.ExpiraEl <= DateTimeOffset.UtcNow ? null : token;
    }

    public async Task<OneTimeToken?> GetActiveByUser(int idUsuario, CancellationToken ct = default)
    {
        var token = await _db.OneTimeTokens
            .Where(t =>
                t.IdUsuario == idUsuario &&
                t.Tipo == OneTimeTokenTipo.ReseteoDeContrasena &&
                !t.Usado)
            .OrderByDescending(t => t.Id)
            .FirstOrDefaultAsync(ct);

        if (token is null)
            return null;

        return token.ExpiraEl <= DateTimeOffset.UtcNow ? null : token;
    }
}