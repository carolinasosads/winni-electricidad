using WinniElectricidad.Compartido.DTOs.Reservas;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reserva;

public interface IObtenerHistoricoFinalizadas
{
    Task<IEnumerable<HistoricoReservaDto>> Ejecutar(CancellationToken ct = default);
}
