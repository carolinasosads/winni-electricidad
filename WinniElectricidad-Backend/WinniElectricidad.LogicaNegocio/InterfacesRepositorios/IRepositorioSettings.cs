using WinniElectricidad.LogicaNegocio.Entidades;

namespace WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

public interface IRepositorioSettings
{
    Task<Settings> Obtener(CancellationToken ct = default);
}