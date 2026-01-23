using WinniElectricidad.LogicaNegocio.Entidades;

namespace WinniElectricidad.LogicaNegocio.InterfacesRepositorios;

public interface IRepositorioReserva : IRepositorio<Reserva>
{
    Task<IReadOnlyList<Reserva>> FindAllBetweenDates(DateTime fechaMinima, DateTime fechaLimite, CancellationToken ct = default);
    Task<bool> UsuarioTieneReservaEnHorario(int idUsuario, DateTime fechaReserva, CancellationToken ct = default);
    Task<bool> UsuarioTieneReservaEnDia(int idUsuario, DateTime fechaReserva, CancellationToken ct = default);
    Task<bool> HorarioOcupado(DateTime fechaReserva, CancellationToken ct = default);
    Task<IReadOnlyList<Reserva>> FindAllFinalizadas(string? filtro, CancellationToken ct = default);
    Task<IReadOnlyList<Reserva>> FindAllSegunEstado(EstadoReserva estado, string? filtro,  CancellationToken ct = default);
    Task<Reserva?> ObtenerReservaPorId(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Reserva>> FindHistoricoReservas(DateTime fechaMinima, DateTime fechaLimite, string? filtro, CancellationToken ct = default);
    Task<IEnumerable<Reserva>> GetReservaSegunClienteId(int clienteId, CancellationToken ct = default);
    Task<IEnumerable<Reserva>> ObtenerReservasClienteConDetalle(int idCliente, CancellationToken ct = default);
}