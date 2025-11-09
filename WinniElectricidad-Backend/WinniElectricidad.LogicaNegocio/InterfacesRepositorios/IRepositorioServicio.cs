using WinniElectricidad.LogicaNegocio.Entidades;

namespace WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

public interface IRepositorioServicio : IRepositorio<Servicio>
{
    Task<IReadOnlyList<Servicio>> FindAllActive(CancellationToken ct = default);
}