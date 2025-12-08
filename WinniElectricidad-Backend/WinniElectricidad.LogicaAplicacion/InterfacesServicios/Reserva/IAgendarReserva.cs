using WinniElectricidad.Compartido.DTOs.Reservas;
using WinniElectricidad.Compartido.Reservas;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reserva;

public interface IAgendarReserva
{
    Task<ReservaCreadaDto> Ejecutar(ReservaACrearDto nuevaReserva, int idUsuario, CancellationToken ct = default);
}