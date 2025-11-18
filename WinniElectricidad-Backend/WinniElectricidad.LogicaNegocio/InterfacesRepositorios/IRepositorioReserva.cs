using WinniElectricidad.LogicaNegocio.Entidades;

namespace WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

public interface IRepositorioReserva : IRepositorio<Reserva>
{
    Task<IReadOnlyList<Reserva>> FindAllBetweenDates(DateTime fechaMinima, DateTime fechaLimite, CancellationToken ct = default);
    Task<bool> UsuarioTieneReservaEnHorario(int idUsuario, DateTime fechaReserva, CancellationToken ct = default);
    Task<bool> HorarioOcupado(DateTime fechaReserva, CancellationToken ct = default);
}