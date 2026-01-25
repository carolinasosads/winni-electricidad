using WinniElectricidad.Compartido.DTOs.Reservas;

namespace WinniElectricidad.LogicaAplicacion.InterfacesServicios.Reserva;

public interface IObtenerHistoricoFinalizadas
{
    Task<IEnumerable<HistoricoReservaDto>> Ejecutar(string? filtro, CancellationToken ct = default);
}
