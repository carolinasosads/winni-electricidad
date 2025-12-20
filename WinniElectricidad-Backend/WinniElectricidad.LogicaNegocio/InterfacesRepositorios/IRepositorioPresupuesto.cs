using WinniElectricidad.LogicaNegocio.Entidades;

namespace WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

public interface IRepositorioPresupuesto : IRepositorio<Presupuesto>
{
    Task<Presupuesto?> FindByReservaId(int idReserva, CancellationToken ct = default);

}