using WinniElectricidad.Compartido.DTOs.Usuarios.Reserva;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reserva;

public interface IObtenerMisReservasClienteConDetalle
{
    Task<IReadOnlyList<ReservaListadoDto>> Ejecutar(int idCliente, CancellationToken ct);
}