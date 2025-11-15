using WinniElectricidad.LogicaNegocio.Entidades;

namespace WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

public interface IRepositorioReserva : IRepositorio<Reserva>
{
    Task<IReadOnlyList<Reserva>> FindAllBetweenDates(DateTime fechaMinima, DateTime fechaLimite, CancellationToken ct = default);
}