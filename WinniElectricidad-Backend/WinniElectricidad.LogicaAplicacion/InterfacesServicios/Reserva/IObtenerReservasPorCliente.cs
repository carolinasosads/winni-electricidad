using WinniElectricidad.Compartido.DTOs.Usuarios.Reserva;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reserva;

public interface IObtenerReservasPorCliente
{
    Task<IEnumerable<ReservaListadoDto>> EjecutarAsync(int clienteId, CancellationToken ct = default);
}