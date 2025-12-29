using WinniElectricidad.Compartido.DTOs.Reservas;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reserva;

public interface IModificarReserva
{
    Task Ejecutar(ReservaAModificarDto dto, bool esAdmin, CancellationToken cancellationToken = default);
}