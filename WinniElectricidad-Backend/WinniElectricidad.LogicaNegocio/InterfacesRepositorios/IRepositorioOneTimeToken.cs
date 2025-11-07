using WinniElectricidad.LogicaNegocio.Entidades;

namespace WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

public interface IRepositorioOneTimeToken
{
    Task Add(OneTimeToken token, CancellationToken ct = default);
    Task MarkUsed(string tokenHash, CancellationToken ct = default);
    Task<OneTimeToken?> GetActiveByHash(string tokenHash, CancellationToken ct = default);
    Task<OneTimeToken?> GetActiveByUser(int idUsuario, CancellationToken ct = default);
}