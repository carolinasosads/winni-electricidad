using WinniElectricidad.Compartido.DTOs.Reservas;
using WinniElectricidad.Compartido.Reservas;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reserva;

public interface IRegistrarReservaHistoricaAdmin
{
    Task<ReservaCreadaDto> Ejecutar(ReservaACrearDto dto, int idUsuarioCliente, CancellationToken ct = default);
}